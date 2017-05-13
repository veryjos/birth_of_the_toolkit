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

    private int texUnit;

    public Texture(int width, int height, PixelFormat pixelFormat, PixelInternalFormat internalPixelFormat, PixelType pixelType) {
      TextureBufferId = GL.GenTexture();

      Width = width;
      Height = height;

      Format = pixelFormat;
      InternalFormat = internalPixelFormat;

      PixType = pixelType;
    }

    public void Bind(int textureUnit) {
      texUnit = textureUnit;

      GL.BindTextureUnit(textureUnit, TextureBufferId);

      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

      GL.Enable(EnableCap.Texture2D);
    }

    public void Unbind() {
      GL.BindTextureUnit(texUnit, TextureBufferId);

      GL.Disable(EnableCap.Texture2D);
    }

    public void SetData<T>(T[] data)
      where T: struct
    {
      GL.BindTexture(TextureTarget.Texture2D, TextureBufferId);
      GL.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat, Width, Height, 0, Format, PixType, data);

      GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Dispose() {
      GL.DeleteTexture(TextureBufferId);
    }
  }
}