using System.Linq;
using BotWLib.Formats;
using OpenTK;

namespace BoTK.Editor.World.Terrain {
  public class TerrainMap : Renderable {
    public TerrainTile RootTile { get; }

    private TerrainLodTileView tileView = null;

    public TerrainMap(TSCB data) {
      RootTile = LoadRootTileFrom(data);
    }

    private TerrainTile LoadRootTileFrom(TSCB data) {
      TerrainTile rootTile = null;
      rootTile = new TerrainTile(data.TileTableList[0]);

      foreach (var tileMeta in data.TileTableList.Skip(1).ToArray()) {
        var terrainTile = new TerrainTile(tileMeta);

        var parent = rootTile.GetDeepestChild(terrainTile.CenterPosition);
        parent.SetChild(terrainTile.CenterPosition, terrainTile);
      }

      return rootTile;
    }

    public void Render(Camera3D camera) {
      tileView = new TerrainLodTileView(this, Vector2.Subtract(camera.Position.Xy, new Vector2(1.0f, 1.0f)), 2.0f, 0, 256);

      tileView.Render(camera);
    }
  }
}