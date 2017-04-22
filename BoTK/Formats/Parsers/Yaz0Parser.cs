using System.IO;
using BotWLib;

namespace BoTK.Formats.Parsers {
  /// <summary>
  /// Decodes Yaz0 data
  /// </summary>
  public class Yaz0Parser : Parser {
    public static string GetName() {
      return "yaz0";
    }

    public void Parse(Stream inputStream, Stream outputStream) {
      var output = new MemoryStream();

      BotWLib.Formats.Yaz0.Decompress(inputStream, output);

      output.Seek(0, SeekOrigin.Begin);
      output.CopyTo(outputStream);
    }


  }
}