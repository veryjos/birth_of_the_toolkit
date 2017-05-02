using CommandLine;

namespace BoTK {
  public class ProgOptions {
    [Option(
      HelpText = "Decoded input folder")]
    public string dataFolder { get; set; }
  }
}