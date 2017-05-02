using System;
using BotWLib.Formats;
using OpenTK;

namespace BoTK.Editor.World.Terrain {
  public class TerrainTile {
    public Vector2 CenterPosition { get; }
    public float EdgeLength { get; }
    public string Name { get; }

    public readonly TerrainTile[] Children = new TerrainTile[4];

    public TerrainTile(TSCB.TileTableEntry tileMetadata) {
      CenterPosition = new Vector2{ X = tileMetadata.CenterX, Y = tileMetadata.CenterY };
      EdgeLength = tileMetadata.EdgeLength;
      Name = tileMetadata.Name;
    }

    public TerrainTile GetChildAtLevel(Vector2 pos, int depth = 0) {
      var child = Children[(pos.X < CenterPosition.X ? 0 : 1) + (pos.Y < CenterPosition.Y ? 0 : 2)];

      if (depth == 0)
        return child;

      return child?.GetChildAtLevel(pos, depth - 1);
    }

    public TerrainTile GetDeepestChild(Vector2 pos) {
      var child = Children[(pos.X < CenterPosition.X ? 0 : 1) + (pos.Y < CenterPosition.Y ? 0 : 2)];

      return child != null ? child.GetDeepestChild(pos) : this;
    }

    public void SetChild(Vector2 pos, TerrainTile tile) {
      Children[(pos.X < CenterPosition.X ? 0 : 1) + (pos.Y < CenterPosition.Y ? 0 : 2)] = tile;
    }
  }
}