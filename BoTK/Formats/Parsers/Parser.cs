using System.IO;

namespace BoTK.Formats.Parsers {
  public interface Parser {
    /// <summary>
    /// Parses the format
    /// </summary>
    /// <param name="input">Input stream</param>
    /// <param name="output">Output stream</param>
    void Parse(Stream input, Stream output);
  }
}