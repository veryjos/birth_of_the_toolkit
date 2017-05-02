using System;
using OpenTK.Graphics.OpenGL4;

namespace BoTK.Editor.Common {
  public class Shader : IDisposable {
    private readonly int vertexShaderId;
    private readonly int fragmentShaderId;

    private readonly int programId;

    public Shader(string vertexSource, string fragmentSource) {
      this.vertexShaderId = CreateShader(vertexSource, ShaderType.VertexShader);
      this.fragmentShaderId = CreateShader(fragmentSource, ShaderType.FragmentShader);

      this.programId = GL.CreateProgram();

      GL.AttachShader(this.programId, this.vertexShaderId);
      GL.AttachShader(this.programId, this.fragmentShaderId);

      GL.LinkProgram(this.programId);

      GLUtil.ThrowIfGLError();
    }

    private static int CreateShader(string source, ShaderType shaderType) {
      int shaderId;

      switch (shaderType) {
        case ShaderType.FragmentShader:
        case ShaderType.VertexShader:
          shaderId = GL.CreateShader(shaderType);
          break;

        default:
          throw new NotImplementedException("Shader type is not implemented");
      }

      GL.ShaderSource(shaderId, source);
      GL.CompileShader(shaderId);

      GLUtil.ThrowIfGLError();

      return shaderId;
    }

    public void Dispose() {
      GL.DeleteShader(vertexShaderId);
      GL.DeleteShader(fragmentShaderId);

      GL.DeleteProgram(programId);
    }
  }
}