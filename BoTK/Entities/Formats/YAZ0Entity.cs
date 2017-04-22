using System.IO;

namespace BoTK.Entities.Formats {
  public class YAZ0Entity : Entity {
    private MemoryStream m_decodedStream;

    public static Magic GetMagic() {
      return new Magic("Yaz0");
    }

    public override bool IsTerminal() {
      return false;
    }

    public override string GetTypeAsString() {
      return "YAZ0";
    }

    public override void LoadFromNativeStream(Stream inputStream) {
      var output = new MemoryStream();

      BotWLib.Formats.Yaz0.Decompress(inputStream, output);

      m_decodedStream = new MemoryStream();

      output.Seek(0, SeekOrigin.Begin);
      output.CopyTo(m_decodedStream);
    }

    public override void WriteDecodedToStream(Stream outputStream) {
      m_decodedStream.Seek(0, SeekOrigin.Begin);
      m_decodedStream.CopyTo(outputStream);

      m_decodedStream.Seek(0, SeekOrigin.Begin);
    }
  }
}