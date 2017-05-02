using System.Linq;
using BotWLib.Formats;

using OpenTK;

namespace BoTK.Editor.World.Terrain {
  public class TerrainMap : Renderable {
    private TerrainTile RootTile;

    public TerrainMap(TSCB data) {
      LoadFrom(data);
    }

    private void LoadFrom(TSCB data) {
      RootTile = new TerrainTile(data.TileTableList[0]);

      foreach (var tileMeta in data.TileTableList.Skip(1).ToArray()) {
        var terrainTile = new TerrainTile(tileMeta);

        var parent = RootTile.GetDeepestChild(terrainTile.CenterPosition);
        parent.SetChild(terrainTile.CenterPosition, terrainTile);
      }
    }

    private Renderable LazyGenerateRenderData(Camera3D camera) {

    }

    public void Render(Camera3D camera) {
      let terrainRenderable = LazyGenerateRenderData(camera);
    }
  }
}