using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace BoTK.Editor.Common {
  public class GLUtil {
    public static void ThrowIfGLError() {
      var error = GL.GetError();

      if (error != ErrorCode.NoError)
        throw new GraphicsErrorException(error.ToString());
    }
  }
}