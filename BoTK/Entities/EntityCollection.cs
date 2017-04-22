using System.Collections.Generic;
using System.IO;

namespace BoTK.Entities {
  public abstract class EntityCollection : Entity {
    public List<Entity> entities = new List<Entity>();

    public override bool IsCollection() {
      return true;
    }

    public abstract void LoadFromStream(Stream inputStream);

    public void WriteDecodedToFolder(string path) {
      Directory.CreateDirectory(path);

      foreach (var entity in entities) {
        var outPath = Path.Combine(path, entity.Name);

        var outStream = File.OpenWrite(outPath);

        entity.WriteDecodedToStream(outStream);

        outStream.Close();
      }
    }
  }
}