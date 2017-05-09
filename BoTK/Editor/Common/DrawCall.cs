using OpenTK.Graphics.OpenGL4;

namespace BoTK.Editor.Common {
  public class DrawCall {
    public readonly Shader shader;
    public readonly Material material;
    public readonly VertexBuffer vertexBuffer;

    public DrawCall(Shader shader, Material material, VertexBuffer vertexBuffer) {
      this.shader = shader;
      this.material = material;
      this.vertexBuffer = vertexBuffer;
    }

    public void Submit() {
      // For now, just render in place and reset state changes
      shader.Bind(this);

      material.Bind(this);
      vertexBuffer.Bind(this);

      GL.DrawElements(PrimitiveType.Triangles, vertexBuffer.length, DrawElementsType.UnsignedInt, 0);

      shader.Unbind(this);

      material.Unbind(this);
      vertexBuffer.Unbind(this);
    }
  }
}