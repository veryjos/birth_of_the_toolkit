using System.Collections.Generic;
using BotWLib.Common;
using BotWWorldViewer.Resource;

namespace BoTK.Editor.World.Terrain {
  public class TerrainHeightmapBank {
    public static Dictionary<string, TerrainHeightmap> heightmaps = new Dictionary<string, TerrainHeightmap>();

    public static TerrainHeightmap GetOrLoadHeightmap(string name) {
      // If we already have this heightmap loaded, just return it
      if (heightmaps.ContainsKey(name))
        return heightmaps[name];

      // Otherwise, load it
      var dataStream = ResourceManager.ReadFile(name);

      // Convert the byte array into an unsigned short array
      var binaryReader = new EndianBinaryReader(dataStream, Endian.Little);

      var ushorts = new ushort[256, 256];
      for (int i = 0; i < 256 * 256; ++i)
        ushorts[i % 256, i / 256] = binaryReader.ReadUInt16();

      // Create and return our heightmap
      TerrainHeightmap heightmap = new TerrainHeightmap(ushorts);
      heightmaps[name] = heightmap;

      return heightmap;
    }
  }
}