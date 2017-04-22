using System;
using System.IO;
using BoTK.Entities.Formats;

namespace BoTK.Entities {
  public class EntityRepository {
    public static Entity Decode(Stream stream) {
      while (true) {
        var magic = new byte[4];
        stream.Read(magic, 0, 4);
        stream.Seek(0, SeekOrigin.Begin);

        var entity = CreateFromMagic(new Magic(magic));
        entity.LoadFromNativeStream(stream);

        if (entity.IsTerminal())
          return entity;
        else {
          stream = new MemoryStream();

          entity.WriteDecodedToStream(stream);
          stream.Seek(0, SeekOrigin.Begin);
        }
      }
    }

    public static Entity CreateFromMagic(Magic magic) {
      if (magic == SARCEntityCollection.GetMagic())
        return new SARCEntityCollection();

      if (magic == BYMLEntity.GetMagic())
        return new BYMLEntity();

      if (magic == YAZ0Entity.GetMagic())
        return new YAZ0Entity();

      return new BinaryEntity();
    }
  }
}