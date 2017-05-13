using System;
using OpenTK;

namespace BoTK.Editor {
  public class Camera3D {
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }

    private Matrix4 viewMatrix = new Matrix4();
    public Matrix4 ViewMatrix {
      get { return viewMatrix; }
    }


    private Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(1.0f, 4.0f / 3.0f, 0.01f, 1000.0f);
    public Matrix4 ProjectionMatrix {
      get { return projectionMatrix; }
    }

    public void Translate(Vector3 delta) {
      Position.Add(delta);

      UpdateViewMatrix();
    }

    public void TranslateViewRelative(Vector2 delta) {
      var invRot = viewMatrix.ExtractRotation().Inverted();

      var forward = invRot * -Vector3.UnitZ;
      Position += forward * delta.Y;

      var left = invRot * -Vector3.UnitX;
      Position += left * delta.X;

      UpdateViewMatrix();
    }

    public void Rotate(Vector2 delta) {
      Rotation = new Vector3 {
        X = (Rotation.X - delta.X) % ((float) Math.PI * 2.0f),
        Y = Math.Max(Math.Min(Rotation.Y + delta.Y, (float) Math.PI / 2.0f - 0.1f), (float) -Math.PI / 2.0f + 0.1f),
        Z = 0.0f
      };

      UpdateViewMatrix();
    }

    private void UpdateViewMatrix() {
      var lookat = Quaternion.FromEulerAngles(0.0f, Rotation.X, Rotation.Y) * Vector3.UnitZ;

      viewMatrix = Matrix4.LookAt(Position, Position + lookat, Vector3.UnitY);
    }
  }
}