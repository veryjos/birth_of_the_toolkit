using System.IO;
using System.Linq;
using BotWLib.Formats;
using BotWWorldViewer.Resource;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace BoTK.Editor.World {
  public class WorldEditor : Editor {
    public WorldData WorldData { get; }
    public Camera3D Camera { get; }

    public WorldEditor(Stream tscbInputStream, string mapDirectory) {
      // Walk the map directory and load every archive
      var dir = new DirectoryInfo(mapDirectory);
      var heightmaps = dir.GetFiles().Where(f => f.Name.Contains("hght.sstera"));

      foreach (var heightmap in heightmaps) {
        // Load each heightmap archive
        ResourceManager.LoadArchive(heightmap.FullName);
      }

      var tscb = new TSCB(tscbInputStream);

      WorldData = new WorldData(tscb);
    }

    protected override void OnRenderFrame(FrameEventArgs e) {
      GL.ClearColor(Color4.White);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      // Render terrain
      WorldData.Terrain.Render(Camera);

      SwapBuffers();
    }

    public override void Start() {
      Run(60.0);
    }
  }
}