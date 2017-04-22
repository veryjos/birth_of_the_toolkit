using System;
using System.IO;
using CommandLine;

using BoTK.Formats;

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
      // Get the input stream that the options describe
      var inputStream = GetInputStream(options);
      var outputStream = GetOutputStream(options);

      var parseChain = new ParseChain(inputStream);

      foreach (var parserName in options.parserChainArray) {
        parseChain.Parse(ParserFactory.CreateFromName(parserName));
      }

      parseChain.GetResult(outputStream);

      inputStream.Close();

      outputStream.Flush();
      outputStream.Close();

      return 0;
    }

    public static Stream GetOutputStream(ProgOptions options) {
      if (options.outputFile == null)
        return Console.OpenStandardOutput();

      return File.OpenWrite(options.outputFile);
    }

    public static Stream GetInputStream(ProgOptions options) {
      if (options.inputFile == null) {
        var inStream = new MemoryStream();

        Console.OpenStandardInput().CopyTo(inStream);
        inStream.Seek(0, SeekOrigin.Begin);

        return inStream;
      }

      if (!File.Exists(options.inputFile))
        throw new FileNotFoundException("Input file not found");

      return File.OpenRead(options.inputFile);
    }
  }
}
