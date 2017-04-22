using System;
using System.IO;
using BoTK.Entities;
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
      var inputStream = GetInputStream(options);

      // Parse the input stream
      var entity = EntityRepository.Decode(inputStream);

      if (entity.IsCollection()) {
        (entity as EntityCollection).WriteDecodedToFolder(options.output);
      }
      else {
        var outputStream = GetOutputStream(options);

        entity.WriteDecodedToStream(outputStream);

        outputStream.Close();
      }

      inputStream.Close();

      return 0;
    }

    public static Stream GetInputStream(ProgOptions options) {
      if (options.input == null) {
        var inStream = new MemoryStream();

        Console.OpenStandardInput().CopyTo(inStream);
        inStream.Seek(0, SeekOrigin.Begin);

        return inStream;
      }

      if (!File.Exists(options.input))
        throw new FileNotFoundException("Input file not found");

      return File.OpenRead(options.input);
    }

    public static Stream GetOutputStream(ProgOptions options) {
      if (options.output == null)
        return Console.OpenStandardOutput();

      return File.OpenWrite(options.output);
    }
  }
}
