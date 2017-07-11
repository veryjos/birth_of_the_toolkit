using System.Collections.Generic;
using System.Threading.Tasks;
using BotWLib.Common;
using BotWWorldViewer.Resource;
using CommandLine;

namespace BoTK.Editor.World.Terrain {
  public class TerrainHeightmapBank {
    public static Dictionary<string, Task<TerrainHeightmap>> heightmaps = new Dictionary<string, Task<TerrainHeightmap>>();

    public static TerrainHeightmap GetOrLoadHeightmap(string name) {
      // If we already have this heightmap loaded, just return it
      if (heightmaps.ContainsKey(name)) {
        var value = heightmaps[name];

        value.Wait();
        return heightmaps[name].Result;
      }

      // Otherwise, load it
      var dataStream = ResourceManager.ReadFile(name);

      // Convert the byte array into an unsigned short array
      var binaryReader = new EndianBinaryReader(dataStream, Endian.Little);

      var ushorts = new ushort[256, 256];
      for (int i = 0; i < 256 * 256; ++i)
        ushorts[i % 256, i / 256] = binaryReader.ReadUInt16();

      // Create and return our heightmap
      TerrainHeightmap heightmap = new TerrainHeightmap(ushorts);
      heightmaps[name] = Task.FromResult(heightmap);

      return heightmap;
    }

    public static Task<TerrainHeightmap> GetOrLoadHeightmapAsync(string name) {
      // If we already have this heightmap loaded, just return it
      if (heightmaps.ContainsKey(name))
        return heightmaps[name];

      var rValue = Task.Run(() => {
        // Otherwise, load it
        var dataStream = ResourceManager.ReadFile(name);

        // Convert the byte array into an unsigned short array
        var binaryReader = new EndianBinaryReader(dataStream, Endian.Little);

        var ushorts = new ushort[256, 256];
        for (int i = 0; i < 256 * 256; ++i)
          ushorts[i % 256, i / 256] = binaryReader.ReadUInt16();

        // Create and return our heightmap
        TerrainHeightmap heightmap = new TerrainHeightmap(ushorts);
        return heightmap;
      });

      heightmaps[name] = rValue;
      return rValue;
    }
  }
}