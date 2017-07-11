using System;
using System.Collections.Generic;
using OpenTK;

namespace BoTK.Editor.World.Terrain {
  /// <summary>
  /// Responsible for laying out TerrainLodTileView views to scale
  /// LOD down with distance
  /// </summary>
  public class TerrainLodLayout {
    public TerrainLodTileView[] TileViews { get; private set; }
    public TerrainMap Map { get; }

    private float lastOriginX = -1.0f;
    private float lastOriginY = -1.0f;

    public TerrainLodLayout(TerrainMap map) {
      Map = map;
    }

    public void SetCameraPos(float x, float y) {
      var tiles = new List<TerrainLodTileView>();

      // Create the base 4 highest level of detail tile views in the center
      // The smallest tile has an edge length of 0.125, and they're grid aligned.
      var originX = (float)Math.Round(x / 0.125f) * 0.125f - 0.125f;
      var originY = (float)Math.Round(y / 0.125f) * 0.125f - 0.125f;

      if (originX == lastOriginX &&
          originY == lastOriginY)
        return;

      lastOriginX = originX;
      lastOriginY = originY;

      for (int ty = 0; ty < 3; ++ty) {
        for (int tx = 0; tx < 3; ++tx) {
          var view = new TerrainLodTileView(Map,
            new Vector2(originX + tx * 0.125f, originY + ty * 0.125f),
            0.125f,
            8,
            256
          );

          tiles.Add(view);
        }
      }

      // Create the less-detailed LODs
      var offset = 0.0f;
      var lastSize = 0.125f;
      for (int level = 7; level >= 6; --level) {
        // Get the world tile size
        var worldTileSize = lastSize * 3.0f;
        lastSize = worldTileSize;

        offset -= worldTileSize;

        // Create a ring of tile views around the center.
        // The center was filled by the previous LOD.
        for (int ty = 0; ty < 3; ++ty) {
          for (int tx = 0; tx < 3; ++tx) {
            if (tx == 1 && ty == 1)
              continue;

            var view = new TerrainLodTileView(Map,
              new Vector2(
                originX + offset + tx * worldTileSize,
                originY + offset + ty * worldTileSize
              ),
              worldTileSize,
              level,
              256
            );

            tiles.Add(view);
          }
        }
      }

      TileViews = tiles.ToArray();
    }
  }
}