using System;
using BoTK.Editor.Common;
using OpenTK;
using OpenTK.Input;

namespace BoTK.Editor.World.Terrain {
  /// <summary>
  /// A tile for a LOD.
  ///
  /// Will basically be the most direct mapping from texture to vertex height.
  /// Consumes multiple tiles for a certain LOD (or the closest one that satisfies it)
  /// to generate a bindable texture
  /// </summary>
  public class TerrainLodTileView : IDisposable {
    public Vector2 Position { get; }
    public Vector2 Bounds { get; }

    public int LodLevel { get; }

    private Texture depthTexture;

    /// <summary>
    /// Creates a new TerrainLodTileView.
    /// </summary>
    /// <param name="position">The world position of the tile</param>
    /// <param name="worldEdgeLength">The world edge length of the tile to use</param>
    /// <param name="lodLevel">The minimum LOD level to use for this tile view</param>
    public TerrainLodTileView(Vector2 position, float worldEdgeLength, int lodLevel) {
      Position = position;
      Bounds = new Vector2(worldEdgeLength, worldEdgeLength);

      LodLevel = lodLevel;
    }

    /// <summary>
    /// Builds the texture used for this TerrainLodTileView
    /// </summary>
    private void BuildTexture(TerrainMap terrainMap) {
      depthTexture?.Dispose();

      var rootTile = terrainMap.RootTile;
      var contributors = rootTile.GetChildrenForTileView(Position, Bounds, LodLevel);

      foreach (var tile in contributors) {
        // Load the tile's texture data
        var fileName = tile.Name + ".hght";
      }
    }

    public void Dispose() {
      depthTexture?.Dispose();
    }
  }
}