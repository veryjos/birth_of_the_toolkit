using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using BoTK.Editor.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace BoTK.Editor.World.Terrain {
  /// <summary>
  /// A tile for a LOD.
  ///
  /// Will basically be the most direct mapping from texture to vertex height.
  /// Consumes multiple tiles for a certain LOD (or the closest one that satisfies it)
  /// to generate a bindable texture
  /// </summary>
  public class TerrainLodTileView : IDisposable, Renderable {
    public TerrainMap Map { get; }
    public Vector2 Position { get; }
    public Vector2 Bounds { get; }

    public uint VboSize { get; }

    public int LodLevel { get; }

    private VertexBuffer vertexBuffer;

    private Material material;
    private Shader shader;

    private bool vertexBufferInvalid = true;

    /// <summary>
    /// Creates a new TerrainLodTileView.
    /// </summary>
    /// <param name="position">The world position of the tile</param>
    /// <param name="worldEdgeLength">The world edge length of the tile to use</param>
    /// <param name="lodLevel">The minimum LOD level to use for this tile view</param>
    public TerrainLodTileView(TerrainMap map, Vector2 position, float worldEdgeLength, int lodLevel, uint vboSize) {
      Map = map;

      Position = position;
      Bounds = new Vector2(worldEdgeLength, worldEdgeLength);

      LodLevel = lodLevel;
      VboSize = vboSize;

      material = new Material();
      shader = new Shader(File.ReadAllText("redist/shaders/test.vert"),
        File.ReadAllText("redist/shaders/test.frag"));
    }

    // TODO: Reduce overdraw, other optimizations, bilinear filtering..
    private void RebuildVertexBuffer() {
      // Destroy the old vertex buffer
      vertexBuffer?.Dispose();

      // Create index buffer
      uint numQuads = VboSize - 1;
      var indices = new uint[(numQuads * numQuads) * 6];

      uint vertNum = 0;
      uint vertStride = VboSize;

      for (int i = 0; i < indices.Length; i += 6) {
        // Generates triangle indices in CW order

        // TL triangle
        // 0, 1, 2
        indices[i + 0] = vertNum;
        indices[i + 1] = vertNum + 1;
        indices[i + 2] = vertNum + vertStride;

        // BR triangle
        // 2, 1, 3
        indices[i + 3] = vertNum + vertStride;
        indices[i + 4] = vertNum + 1;
        indices[i + 5] = vertNum + vertStride + 1;

        vertNum++;

        // We need to skip the last vert of each row
        // so we don't make some weird wrapping-around tris
        if ((vertNum + 1) % vertStride == 0)
          vertNum++;
      }

      // Start generating new VBO data
      ColoredVertex[] vboData = new ColoredVertex[VboSize * VboSize];

      {
        // Create a flat grid of vertices
        float x = 0.0f;
        float z = 0.0f;

        int i = 0;
        for (int iy = 0; iy < VboSize; iy++) {
          x = 0.0f;

          for (int ix = 0; ix < VboSize; ix++) {
            vboData[i++] = new ColoredVertex() {
              R = 0.0f,
              G = 0.0f,
              B = 0.0f,
              A = 1.0f,

              X = x,
              Y = 0.0f,
              Z = z,

              NX = 0.0f,
              NY = 0.0f,
              NZ = 0.0f
            };

            x += Bounds.X / (VboSize - 1);
          }

          z += Bounds.Y / (VboSize - 1);
        }
      }

      // Get each contributing tile for this tile view
      var rootTile = Map.RootTile;
      var contributors = rootTile.GetChildrenForTileView(Position, Bounds, LodLevel);

      foreach (var tile in contributors) {
        // Get the heightmap array for this tile
        var heightmap = tile.GetHeightmap();

        // Blit the heightmap onto our VBO

        // Figure out how far we'll have to step for nearest-neighbor filtering
        // TODO: We're assuming square from this point on :~)
        var iBlitStep = (tile.EdgeLength / Bounds.X) * (VboSize / 256);

        // Convert the tile's position into the grid position we'll blit into the VBO
        var blitX = tile.CenterPosition.X - tile.EdgeLength / 2; // blitX is now world-space top left corner
        blitX = blitX - Position.X; // blitX is now world-space, but relative to our top left corner
        blitX /= Bounds.X; // blitX is now normalized from 0-width to 0-1
        var iBlitX = (int) (blitX * VboSize); // Convert to VBO-tile space

        var blitY = tile.CenterPosition.Y - tile.EdgeLength / 2;
        blitY = blitY - Position.Y;
        blitY /= Bounds.Y;
        var iBlitY = (int) (blitY * VboSize);

        // Blit this data onto our VBO
        for (int x = 0; x < 256; ++x) {
          for (int y = 0; y < 256; ++y) {
            var px = (int)(iBlitX + x * iBlitStep);
            var py = (int)(iBlitY + y * iBlitStep);

            var height = heightmap.Data[x, y];

            for (int ex = 0; ex < iBlitStep; ex++) {
              for (int ey = 0; ey < iBlitStep; ey++) {
                  if (px + ex < 0 || py + ey < 0 ||
                    px + ex >= VboSize || py + ey >= VboSize)
                    continue;

                vboData[px + ex + (py + ey) * VboSize].Y = (height / 65535.0f);
                vboData[px + ex + (py + ey) * VboSize].R = (height / 65535.0f);
              }
            }
          }
        }
      }

      // Calculate vertex normals
      var vertDivisors = new int[VboSize * VboSize];
      var vertNormals = new Vector3[VboSize * VboSize];
      for (int i = 0; i < indices.Length; i += 3) {
        var p1 = vboData[indices[i + 0]];
        var p2 = vboData[indices[i + 1]];
        var p3 = vboData[indices[i + 2]];

        var edge1 = Vector3.Subtract(
                      new Vector3(p2.X, p2.Y, p2.Z),
                      new Vector3(p1.X, p1.Y, p1.Z)
                    );

        var edge2 = Vector3.Subtract(
                      new Vector3(p3.X, p3.Y, p3.Z),
                      new Vector3(p1.X, p1.Y, p1.Z)
                    );

        var normal = Vector3.Cross(edge1, edge2).Normalized();

        vertNormals[indices[i + 0]] += normal;
        vertDivisors[indices[i + 0]]++;

        vertNormals[indices[i + 1]] += normal;
        vertDivisors[indices[i + 1]]++;

        vertNormals[indices[i + 2]] += normal;
        vertDivisors[indices[i + 2]]++;
      }

      for (int i = 0; i < vertNormals.Length; ++i) {
        vertNormals[i] /= vertDivisors[i];

        vboData[i].NX = vertNormals[i].X;
        vboData[i].NY = vertNormals[i].Y;
        vboData[i].NZ = vertNormals[i].Z;
      }

      // Create and validate the vertex buffer
      vertexBuffer = VertexBuffer.Create(vboData, indices);
      vertexBufferInvalid = false;
    }

    public void Render(Camera3D camera) {
      // Get distance to camera to select LOD
      if (vertexBufferInvalid) {
        RebuildVertexBuffer();
      }

      var model = Matrix4.CreateTranslation(Position.X, 0.0f, Position.Y);
      material.SetShaderParameter("modelView", model * camera.ViewMatrix * camera.ProjectionMatrix);

      DrawCall drawCall = new DrawCall(shader, material, vertexBuffer);
      drawCall.Submit();
    }

    public void Dispose() {
      vertexBuffer?.Dispose();
    }
  }
}