using System.Collections.Generic;
using System.Security.Permissions;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace BoTK.Editor.Common {
  public class Material : Bindable {
    public Dictionary<string, ShaderParameter> ShaderParameters = new Dictionary<string, ShaderParameter>();
    public Dictionary<string, Texture> Textures = new Dictionary<string, Texture>();

    public void SetShaderParameter(string name, dynamic value) {
      ShaderParameters[name] = new ShaderParameter(value);
    }

    public void UnsetShaderParameter(string name) {
      ShaderParameters.Remove(name);
    }

    public void SetTexture(string name, Texture value) {
      Textures[name] = value;
    }

    public void UnsetTexture(string name) {
      Textures.Remove(name);
    }

    public void Bind(DrawCall call) {
      // Bind each shader parameter
      foreach (var entry in ShaderParameters) {
        int location = call.shader.GetUniformLocation(entry.Key);

        entry.Value.Apply(location);
      }

      // Bind each texture
      int textureUnit = 0;
      foreach (var entry in Textures) {
        int location = call.shader.GetUniformLocation(entry.Key);

        entry.Value.Bind(textureUnit);

        textureUnit++;
      }
    }

    public void Unbind(DrawCall call) {
      // Unbind each texture
      int textureUnit = 0;
      foreach (var entry in Textures) {
        entry.Value.Unbind();
      }
    }
  }
}