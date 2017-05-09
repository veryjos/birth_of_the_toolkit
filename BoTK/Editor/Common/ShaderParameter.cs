using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace BoTK.Editor.Common {
  public class ShaderParameter {
    private readonly ShaderParameterValueBase value;

    public ShaderParameter(float value) {
      this.value = new ShaderParameterValueFloat(value);
    }

    public ShaderParameter(Vector2 value) {
      this.value = new ShaderParameterValueVector2(value);
    }

    public ShaderParameter(Vector3 value) {
      this.value = new ShaderParameterValueVector3(value);
    }

    public ShaderParameter(Matrix3 value) {
      this.value = new ShaderParameterValueMatrix3(value);
    }

    public ShaderParameter(Matrix4 value) {
      this.value = new ShaderParameterValueMatrix4(value);
    }

    public void SetValue<T>(T innerValue) {
      value.SetValue(innerValue);
    }

    public void Apply(int location) {
      value.Apply(location);
    }
  }

  public abstract class ShaderParameterValueBase {
    public abstract void Apply(int location);
    public abstract void SetValue(object value);
  }

  class ShaderParameterValueFloat : ShaderParameterValueBase {
    private float value;

    public ShaderParameterValueFloat(float value) {
      SetValue(value);
    }

    public override void SetValue(object value) {
      this.value = (float)value;
    }

    public override void Apply(int location) {
      GL.Uniform1(location, value);
    }
  }

  class ShaderParameterValueVector2 : ShaderParameterValueBase {
    private Vector2 value;

    public ShaderParameterValueVector2(Vector2 value) {
      SetValue(value);
    }

    public override void SetValue(object value) {
      this.value = (Vector2)value;
    }

    public override void Apply(int location) {
      GL.Uniform2(location, value);
    }
  }

  class ShaderParameterValueVector3 : ShaderParameterValueBase {
    private Vector3 value;

    public ShaderParameterValueVector3(Vector3 value) {
      SetValue(value);
    }

    public override void SetValue(object value) {
      this.value = (Vector3)value;
    }

    public override void Apply(int location) {
      GL.Uniform3(location, value);
    }
  }

  class ShaderParameterValueMatrix3 : ShaderParameterValueBase {
    private Matrix3 value;

    public ShaderParameterValueMatrix3(Matrix3 value) {
      SetValue(value);
    }

    public override void SetValue(object value) {
      this.value = (Matrix3)value;
    }

    public override void Apply(int location) {
      GL.UniformMatrix3(location, false, ref value);
    }
  }

  class ShaderParameterValueMatrix4 : ShaderParameterValueBase {
    private Matrix4 value;

    public ShaderParameterValueMatrix4(Matrix4 value) {
      SetValue(value);
    }

    public override void SetValue(object value) {
      this.value = (Matrix4)value;
    }

    public override void Apply(int location) {
      GL.UniformMatrix4(location, false, ref value);
    }
  }
}