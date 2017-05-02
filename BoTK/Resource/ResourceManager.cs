using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;

namespace BotWWorldViewer.Resource {
  internal static class ResourceManager {
    private static readonly List<Archive> loadedArchives = new List<Archive>();
    private static readonly Dictionary<string, string> fileMap = new Dictionary<string, string>();

    public static Dictionary<string, string> FileMap { get; }

    public static void LoadArchive(string filePath) {
      var archive = Archive.Load(filePath);
      loadedArchives.Add(archive);

      foreach (var file in archive.FileList)
        fileMap.Add(file.Key, filePath);
    }

    public static bool FileExists(String name) {
      return loadedArchives.Any(arch => arch.ContainsFile(name));
    }

    public static FramedStream ReadFile(String name) {
      return (from arch in loadedArchives where arch.ContainsFile(name) select arch.ReadFile(name)).FirstOrDefault();
    }
  }
}