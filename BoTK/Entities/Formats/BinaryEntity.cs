using System.IO;

namespace BoTK.Entities.Formats {
  public class BinaryEntity : Entity {
    private byte[] m_binary;

    public override bool IsTerminal() {
      return true;
    }

    public override string GetTypeAsString() {
      return "BIN";
    }

    public override void LoadFromNativeStream(Stream inputStream) {
      m_binary = new byte[inputStream.Length];
      inputStream.Read(m_binary, 0, (int)inputStream.Length);
    }

    public override void WriteDecodedToStream(Stream outputStream) {
      outputStream.Write(m_binary, 0, m_binary.Length);
    }
  }
}
