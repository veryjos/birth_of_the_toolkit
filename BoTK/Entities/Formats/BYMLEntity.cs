using System.IO;
using System.Text;

namespace BoTK.Entities.Formats {
  public class BYMLEntity : Entity {
    private string m_yaml;

    public static Magic GetMagic() {
      return new Magic("BY");
    }

    public override bool IsTerminal() {
      return true;
    }

    public override string GetTypeAsString() {
      return "BYML";
    }

    public override void LoadFromNativeStream(Stream inputStream) {
      var binary = new byte[inputStream.Length];
      inputStream.Read(binary, 0, (int)inputStream.Length);

      var bymlDecoder = new BotWLib.Formats.BYML(binary);
      m_yaml = bymlDecoder.ToYAML();
    }

    public override void WriteDecodedToStream(Stream outputStream) {
      var resultBytes = Encoding.UTF8.GetBytes(m_yaml);

      outputStream.Write(resultBytes, 0, resultBytes.Length);
    }
  }
}