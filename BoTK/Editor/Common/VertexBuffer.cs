using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

using System.Runtime.InteropServices;
using OpenTK;

namespace BoTK.Editor.Common {
  public class VertexBuffer : IDisposable, Bindable {
    private readonly int vboId;
    private readonly int indexBufferId;

    private readonly VertexLayout vertexLayout;

    public readonly int length;

    public static VertexBuffer Create<T>(T[] vertices, uint[] indices)
      where T : struct {
      var vertexBuffer = new VertexBuffer(indices.Length, typeof(T));

      GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer.vboId);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, vertexBuffer.indexBufferId);

      GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertices.Length * vertexBuffer.vertexLayout.Size), vertices,
        BufferUsageHint.StaticDraw);
      GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(indices.Length * 4), indices,
        BufferUsageHint.StaticDraw);

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

      GLUtil.ThrowIfGLError();

      return vertexBuffer;
    }

    private VertexBuffer(int length, Type layoutType) {
      vboId = GL.GenBuffer();
      indexBufferId = GL.GenBuffer();

      GLUtil.ThrowIfGLError();

      this.length = length;

      vertexLayout = new VertexLayout(layoutType);
    }

    public void Bind(Shader shader) {
    }

    public void Dispose() {
      GL.DeleteBuffers(2, new []{vboId, indexBufferId});
    }

    public void Bind(DrawCall call) {
      GL.BindBuffer(BufferTarget.ArrayBuffer, vboId);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferId);

      vertexLayout.Bind(call);
    }

    public void Unbind(DrawCall call) {
      GL.BindBuffer(BufferTarget.ArrayBuffer, vboId);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferId);

      vertexLayout.Unbind(call);
    }
  }
}