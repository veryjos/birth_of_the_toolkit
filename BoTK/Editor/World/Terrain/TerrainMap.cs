using System.Linq;
using BotWLib.Formats;
using OpenTK;

namespace BoTK.Editor.World.Terrain {
  public class TerrainMap : Renderable {
    public TerrainTile RootTile { get; }

    private TerrainLodLayout lodLayout;

    public TerrainMap(TSCB data) {
      RootTile = LoadRootTileFrom(data);
      lodLayout = new TerrainLodLayout(this);
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
      lodLayout.SetCameraPos(camera.Position.X, camera.Position.Z);

      foreach (var tileView in lodLayout.TileViews)
        tileView.Render(camera);
    }
  }
}