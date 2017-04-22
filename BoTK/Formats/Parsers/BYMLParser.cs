using System.IO;
using System.Text;
using BotWLib;

namespace BoTK.Formats.Parsers {
  public class BYMLParser : Parser {
    public static string GetName() {
      return "byml";
    }

    public void Parse(Stream inputStream, Stream outputStream) {
      var binary = new byte[inputStream.Length];
      inputStream.Read(binary, 0, (int)inputStream.Length);

      var bymlDecoder = new BotWLib.Formats.BYML(binary);
      var resultStr = bymlDecoder.ToYAML();
      var resultBytes = Encoding.UTF8.GetBytes(resultStr);

      outputStream.Write(resultBytes, 0, resultBytes.Length);
    }
  }
}