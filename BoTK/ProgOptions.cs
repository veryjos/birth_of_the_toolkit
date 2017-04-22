using CommandLine;

namespace BoTK {
  public class ProgOptions {
    [Option(
      HelpText = "Enables verbose error logging")]
    public bool verbose { get; set; }

    [Option(
      HelpText = "Input file or folder (stdin by default)")]
    public string input { get; set; }

    [Option(
      HelpText = "Output file or folder (stdout by default)")]
    public string output { get; set; }
  }
}