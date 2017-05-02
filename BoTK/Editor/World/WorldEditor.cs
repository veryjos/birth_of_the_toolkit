using System.IO;
using BotWLib.Formats;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace BoTK.Editor.World {
  public class WorldEditor : Editor {
    public WorldData WorldData { get; }
    public Camera3D Camera { get; }

    public WorldEditor(Stream tscbInputStream) {
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