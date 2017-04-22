using System;
using System.IO;
using System.Text;
using BotWLib;

namespace BoTK.Formats.Parsers {
  public class SARCParser : Parser {
    public static string GetName() {
      return "sarc";
    }

    public void Parse(Stream inputStream, Stream outputStream) {
      var input = new MemoryStream();
      inputStream.CopyTo(input);
      input.Seek(0, SeekOrigin.Begin);

      var sarc = new BotWLib.Formats.SARC(input);

      string output = String.Format("NumNodes: {0}, First Name: {1}", sarc.SfatHeader.NodeCount, sarc.SfatStringTable[0]);

      outputStream.Write(Encoding.ASCII.GetBytes(output), 0, output.Length);
    }
  }
}