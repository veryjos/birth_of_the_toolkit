using BotWLib.Formats;
using BoTK.Editor.World.Terrain;

namespace BoTK.Editor.World {
  public class WorldData {
    public TerrainMap Terrain { get; }

    public WorldData(TSCB tscb) {
      Terrain = new TerrainMap(tscb);
    }
  }
}