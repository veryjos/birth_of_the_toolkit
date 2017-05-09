using System.Collections.Generic;
using System.Security.Permissions;
using OpenTK;

namespace BoTK.Editor.Common {
  public class Material : Bindable {
    public Dictionary<string, ShaderParameter> ShaderParameters = new Dictionary<string, ShaderParameter>();

    public void SetShaderParameter(string name, dynamic value) {
      ShaderParameters[name] = new ShaderParameter(value);
    }

    public void UnsetShaderParameter(string name) {
      ShaderParameters.Remove(name);
    }

    public void Bind(DrawCall call) {
      // Bind each shader parameter
      foreach (var entry in ShaderParameters) {
        int location = call.shader.GetUniformLocation(entry.Key);

        entry.Value.Apply(location);
      }
    }

    public void Unbind(DrawCall call) {
    }
  }
}