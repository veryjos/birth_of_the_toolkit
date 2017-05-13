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
      Camera = new Camera3D();

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

      MainTerrainMap.Render(Camera);

      SwapBuffers();
    }

    public override void Start() {
      Run(60.0);
    }
  }
}