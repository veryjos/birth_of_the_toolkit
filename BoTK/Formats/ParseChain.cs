using System.IO;
using BoTK.Formats.Parsers;

namespace BoTK.Formats {
  /// <summary>
  /// A helper class to chain parsers together.
  /// </summary>
  public class ParseChain {
    private readonly Stream m_outputStream;

    private MemoryStream m_intermediateStream = new MemoryStream();

    /// <summary>
    /// Creates a new ParseChain
    /// </summary>
    /// <param name="inputStream">The stream ot parse from</param>
    public ParseChain(Stream inputStream) {
      // Copy the input stream to the stream being worked on
      inputStream.Seek(0, SeekOrigin.Begin);
      inputStream.CopyTo(m_intermediateStream);
    }

    /// <summary>
    /// Continues parsing the input stream with the specified parser.
    /// </summary>
    /// <param name="parser">The parser to parse with</param>
    /// <returns>The same ParseChain called on for daisy chaining</returns>
    public ParseChain Parse(Parser parser) {
      var outStream = new MemoryStream();

      // Parse our current data into an ouput stream
      m_intermediateStream.Seek(0, SeekOrigin.Begin);
      parser.Parse(m_intermediateStream, outStream);

      // Clear out the intermediate stream and copy the result
      m_intermediateStream = new MemoryStream();

      outStream.Seek(0, SeekOrigin.Begin);
      outStream.CopyTo(m_intermediateStream);

      return this;
    }

    /// <summary>
    /// Copies the result into an output stream
    /// </summary>
    /// <param name="outStream">The stream to copy into</param>
    public ParseChain GetResult(Stream outStream) {
      m_intermediateStream.Seek(0, SeekOrigin.Begin);
      m_intermediateStream.CopyTo(outStream);

      return this;
    }
  }
}