using System;
using OpenTK.Graphics.OpenGL4;

namespace BoTK.Editor.Common {
  public class Texture : IDisposable {
    public readonly int TextureBufferId;

    public readonly int Width;
    public readonly int Height;

    public readonly PixelFormat Format;
    public readonly PixelInternalFormat InternalFormat;

    public readonly PixelType PixType;

    public Texture(int width, int height, byte[] data, PixelFormat pixelFormat, PixelInternalFormat internalPixelFormat, PixelType pixelType) {
      TextureBufferId = GL.GenTexture();

      Width = width;
      Height = height;

      Format = pixelFormat;
      InternalFormat = internalPixelFormat;

      PixType = pixelType;

      GL.BindTexture(TextureTarget.Texture2D, TextureBufferId);

      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

      GL.TexImage2D(TextureTarget.Texture2D, 0, internalPixelFormat, width, height, 0, pixelFormat, PixelType.UnsignedByte, data);

      GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Dispose() {
      GL.DeleteTexture(TextureBufferId);
    }
  }
}