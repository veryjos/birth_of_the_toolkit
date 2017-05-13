using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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
      shader = new Shader(
@"#version 130

in vec3 Position;
in vec4 Color;

out Vec4 vColor;

void main() {
  vColor = Color;

  gl_Position = vec4(Position, 1.0);
}
",
@"#version 130

in Vec4 vColor;
out vec4 frag_color;

void main() {
  frag_color = vColor;
}
");
    }

    private void RebuildVertexBuffer() {
      // Destroy the old vertex buffer
      vertexBuffer?.Dispose();

      // Create index buffer
      uint numQuads = VboSize - 1;
      var indices = new uint[(numQuads * numQuads) * 6];

      uint vertNum = 0;
      uint vertStride = numQuads;

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

        // We need to skip the last vert of each row
        // so we don't make some weird wrapping-around tris
        if (vertNum % vertStride == 0)
          vertNum++;

        vertNum++;
      }

      // Start generating new VBO data
      ColoredVertex[] vboData = new ColoredVertex[VboSize * VboSize];

      {
        // Create a flat grid of vertices
        float x = 0.0f;
        float z = 0.0f;

        int i = 0;
        for (int ix = 0; ix < VboSize; ix++) {
          x = 0.0f;

          for (int iy = 0; iy < VboSize; iy++) {
            vboData[i++] = new ColoredVertex() {
              R = 1.0f,
              G = 1.0f,
              B = 1.0f,
              A = 1.0f,

              X = x,
              Y = 0.0f,
              Z = z
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
        var iBlitStep = (int)(tile.EdgeLength / Bounds.X);

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
            var px = iBlitX + x * iBlitStep;
            var py = iBlitY + y * iBlitStep;

            if (px < 0 || py < 0 ||
                px >= VboSize || py >= VboSize)
              continue;

            var height = heightmap.Data[x, y];
            vboData[px + py * VboSize].Z = height;

            for (int ex = 0; ex < iBlitStep; ex++) {
              for (int ey = 0; ey < iBlitStep; ey++) {
                // vboData[px + ex + (py + ey) * VboSize].Z = height;
                vboData[px + ex + (py + ey) * VboSize].R = (height / 65535.0f);
              }
            }
          }
        }
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

      DrawCall drawCall = new DrawCall(shader, material, vertexBuffer);
      drawCall.Submit();
    }

    public void Dispose() {
      vertexBuffer?.Dispose();
    }
  }
}