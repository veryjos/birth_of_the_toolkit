using CommandLine;

namespace BoTK {
  public class ProgOptions {
    [Option(
      HelpText = "Enables verbose error logging")]
    public bool verbose { get; set; }

    [Option(
      HelpText = "Input file (stdin by default)")]
    public string inputFile { get; set; }

    [Option(
      HelpText = "Output file (stdout by default)")]
    public string outputFile { get; set; }

    [Option(
      HelpText = "Comma-delimited list of parsers to chain when decoding input")]
    public string parserChain { get; set; } = "";
    public string[] parserChainArray => parserChain.Split(',');
  }
}