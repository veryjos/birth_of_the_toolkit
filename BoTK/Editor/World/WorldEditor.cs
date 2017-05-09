using System.IO;
using System.Linq;
using BotWLib.Formats;
using BotWWorldViewer.Resource;
using BoTK.Editor.Common;
using BoTK.Editor.World.Terrain;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace BoTK.Editor.World {
  public class WorldEditor : Editor {
    public TerrainMap MainTerrainMap { get; }
    public Camera3D Camera { get; }

    public Shader testShader;
    public VertexBuffer testVertexBuffer;
    public Material testMaterial;

    public WorldEditor(Stream tscbInputStream, string mapDirectory) {

      var testVert = File.ReadAllText("redist/shaders/test.vert");
      var testFrag = File.ReadAllText("redist/shaders/test.frag");
      testShader = new Shader(testVert, testFrag);

      testMaterial = new Material();

      ColoredVertex[] vertices = new ColoredVertex[] {
        ColoredVertex.Create(
          new Vector3(0.0f, 0.0f, 0.0f),
          new Vector4(1.0f, 0.0f, 0.0f, 1.0f)
        ),

        ColoredVertex.Create(
          new Vector3(1.0f, 0.0f, 0.0f),
          new Vector4(1.0f, 0.0f, 0.0f, 1.0f)
        ),

        ColoredVertex.Create(
          new Vector3(1.0f, 1.0f, 0.0f),
          new Vector4(1.0f, 0.0f, 0.0f, 1.0f)
        ),
      };

      testVertexBuffer = VertexBuffer.Create(vertices, new uint[]{
        0, 1, 2
      });

      // Walk the map directory and load every archive
      var dir = new DirectoryInfo(mapDirectory);
      var heightmaps = dir.GetFiles().Where(f => f.Name.Contains("hght.sstera"));

      // Load each heightmap archive
      foreach (var heightmap in heightmaps)
        ResourceManager.LoadArchive(heightmap.FullName);

      // Load the TSCB file passed in from stream and pass to terrain map
      var tscb = new TSCB(tscbInputStream);
      MainTerrainMap = new TerrainMap(tscb);
    }

    protected override void OnRenderFrame(FrameEventArgs e) {
      GL.ClearColor(Color4.White);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      DrawCall drawCall = new DrawCall(testShader, testMaterial, testVertexBuffer);
      drawCall.Submit();

      SwapBuffers();
    }

    public override void Start() {
      Run(60.0);
    }
  }
}