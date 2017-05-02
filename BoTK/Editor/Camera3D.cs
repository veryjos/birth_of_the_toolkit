using System;
using OpenTK;

namespace BoTK.Editor {
  public class Camera3D {
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }

    public Matrix4 ViewMatrix {
      get {
        var lookat = new Vector3 {
          X = (float) (Math.Sin(Rotation.X) * Math.Cos(Rotation.Y)),
          Y = (float) (Math.Sin(Rotation.Y)),
          Z = (float) (Math.Cos(Rotation.X) * Math.Cos(Rotation.Y))
        };

        return Matrix4.LookAt(Position, Position + lookat, Vector3.UnitZ);
      }
    }

    public Matrix4 ProjectionMatrix {
      get {
        return Matrix4.CreatePerspectiveFieldOfView(90.0f, 1.0f, 0.01f, 1.0f);
      }
    }

    public void Translate(Vector3 delta) {
      Position.Add(delta);
    }

    public void TranslateViewRelative(Vector2 delta) {
      var forward = new Vector3(ViewMatrix.ExtractRotation().ToAxisAngle());

      Position.Add(Vector3.Multiply(forward, delta.Y));
    }

    public void Rotate(Vector2 delta) {
      Rotation = new Vector3 {
        X = (Rotation.X + delta.X) % ((float) Math.PI * 2.0f),
        Y = Math.Max(Math.Min(Rotation.Y + delta.Y, (float)Math.PI / 2.0f - 0.1f), (float)-Math.PI / 2.0f + 0.1f),
        Z = 0.0f
      };
    }
  }
}