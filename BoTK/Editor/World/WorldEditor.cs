using System;
using System.IO;
using System.Linq;
using BotWLib.Formats;
using BotWWorldViewer.Resource;
using BoTK.Editor.Common;
using BoTK.Editor.World.Terrain;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace BoTK.Editor.World {
  public class WorldEditor : Editor {
    public TerrainMap MainTerrainMap { get; }
    public Camera3D Camera { get; }

    public Shader testShader;
    public VertexBuffer testVertexBuffer;
    public Material testMaterial;

    private Vector2 lastMousePos;

    public WorldEditor(Stream tscbInputStream, string mapDirectory) {
      Camera = new Camera3D();
      Camera.Position = new Vector3(0.0f, 5.0f, 0.0f);

      // Walk the map directory and load every archive
      var dir = new DirectoryInfo(mapDirectory);
      var heightmaps = dir.GetFiles().Where(f => f.Name.Contains("hght.sstera"));

      // Load each heightmap archive
      foreach (var heightmap in heightmaps)
        ResourceManager.LoadArchive(heightmap.FullName);

      // Load the TSCB file passed in from stream and pass to terrain map
      var tscb = new TSCB(tscbInputStream);
      MainTerrainMap = new TerrainMap(tscb);

      Title = "Birth of the Toolkit";
    }

    protected override void OnUpdateFrame(FrameEventArgs e) {
      // Translate the camera
      var keyState = Keyboard.GetState();

      const float speed = 0.05f;
      if (keyState.IsKeyDown(Key.W)) {
        Camera.TranslateViewRelative(new Vector2(0.0f, 1.0f) * speed);
      } else if (keyState.IsKeyDown(Key.S)) {
        Camera.TranslateViewRelative(new Vector2(0.0f, -1.0f) * speed);
      }

      if (keyState.IsKeyDown(Key.A)) {
        Camera.TranslateViewRelative(new Vector2(1.0f, 0.0f) * speed);
      } else if (keyState.IsKeyDown(Key.D)) {
        Camera.TranslateViewRelative(new Vector2(-1.0f, 0.0f) * speed);
      }

      // Rotate the camera

      var centerPos = new Vector2(
        Bounds.Left + Bounds.Width / 2,
        Bounds.Top + Bounds.Height / 2
      );

      var cursorState = OpenTK.Input.Mouse.GetCursorState();
      var rotationDelta = new Vector2((cursorState.X - lastMousePos.X) * 0.005f, (cursorState.Y - lastMousePos.Y) * 0.005f);
      Camera.Rotate(rotationDelta);

      lastMousePos = new Vector2(cursorState.X, cursorState.Y);
    }

    protected override void OnRenderFrame(FrameEventArgs e) {
      GL.ClearColor(Color4.CornflowerBlue);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      MainTerrainMap.Render(Camera);

      SwapBuffers();
    }

    public override void Start() {
      Run(60.0);
    }
  }
}