using System;
using OpenTK.Graphics.OpenGL4;

using System.Runtime.InteropServices;

namespace BoTK.Editor.Common {
  public struct ColoredVertex {
    public float X, Y, Z;
    public float R, G, B, A;

    public const int Stride =
      3 * 4 + // Position (3 floats)
      4 * 4;  // Color (4 floats)
  }

  public class VertexBuffer : IDisposable {
    private readonly int vboId;
    private readonly int indexBufferId;

    private readonly int length;

    public static VertexBuffer Create<T>(T[] vertices, uint[] indices)
      where T : struct {
      var vertexBuffer = new VertexBuffer(indices.Length);

      GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer.vboId);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, vertexBuffer.indexBufferId);

      GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertices.Length * Marshal.SizeOf(typeof(T))), vertices,
        BufferUsageHint.StaticDraw);
      GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(indices.Length * Marshal.SizeOf(typeof(T))), indices,
        BufferUsageHint.StaticDraw);

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

      GLUtil.ThrowIfGLError();

      return vertexBuffer;
    }

    private VertexBuffer(int length) {
      vboId = GL.GenBuffer();
      indexBufferId = GL.GenBuffer();

      this.length = length;

      GL.BindBuffer(BufferTarget.ArrayBuffer, vboId);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferId);

      GLUtil.ThrowIfGLError();
    }

    public void Dispose() {
      GL.DeleteBuffers(2, new []{vboId, indexBufferId});
    }

  }
}