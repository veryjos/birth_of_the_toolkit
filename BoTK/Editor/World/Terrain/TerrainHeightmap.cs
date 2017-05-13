namespace BoTK.Editor.World.Terrain {
  public class TerrainHeightmap {
    public ushort[,] Data { get; }

    public TerrainHeightmap(ushort[,] data) {
      Data = data;
    }
  }
}