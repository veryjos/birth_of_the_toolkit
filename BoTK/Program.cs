using System.IO;
using BoTK.Editor.World;
using CommandLine;

namespace BoTK {

  internal class Program {
    public static int Main(string[] args) {
      // Parse options passed via shell
      return CommandLine.Parser.Default.ParseArguments<ProgOptions>(args)
        .MapResult(
          MainWithOpts,
          errs => 1
        );
    }

    public static int MainWithOpts(ProgOptions options) {
      var tscbStream = File.OpenRead(options.dataFolder + "/Terrain/A/MainField.tscb");

      var editor = new WorldEditor(tscbStream);
      editor.Start();

      return 0;
    }
  }
}
