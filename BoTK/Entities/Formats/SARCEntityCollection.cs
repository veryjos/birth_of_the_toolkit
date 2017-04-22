using System.IO;

namespace BoTK.Entities.Formats {
  public class SARCEntityCollection : EntityCollection {

    public static Magic GetMagic() {
      return new Magic("SARC");
    }

    public override bool IsTerminal() {
      return true;
    }

    public override string GetTypeAsString() {
      return "SARC";
    }

    public override void LoadFromStream(Stream inputStream) {
      var input = new MemoryStream();
      inputStream.CopyTo(input);
      input.Seek(0, SeekOrigin.Begin);

      var sarc = new BotWLib.Formats.SARC(input);
      foreach (var node in sarc.SfatNodes) {
        Entity entity = new BinaryEntity();

        entity.Name = node.copied_name;

        var dataStream = new MemoryStream(node.copied_data);
        entity.LoadFromNativeStream(dataStream);

        entities.Add(entity);
      }
    }

    public override void LoadFromNativeStream(Stream inputStream) {
      LoadFromStream(inputStream);
    }

    public override void WriteDecodedToStream(Stream outputStream) {
      throw new System.NotImplementedException();
    }
  }
}