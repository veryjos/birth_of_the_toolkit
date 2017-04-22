using System.IO;

namespace BoTK.Entities {
  public abstract class Entity {
    public string Name { get; set; }

    public virtual bool IsCollection() {
      return false;
    }

    public abstract bool IsTerminal();

    public abstract string GetTypeAsString();

    public abstract void LoadFromNativeStream(Stream inputStream);
    public abstract void WriteDecodedToStream(Stream outputStream);
  }
}