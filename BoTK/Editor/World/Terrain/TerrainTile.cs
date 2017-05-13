using System;
using System.Collections.Generic;
using System.Linq;
using BotWLib.Formats;
using OpenTK;

namespace BoTK.Editor.World.Terrain {
  /// <summary>
  /// A unit of terrain.
  ///
  /// For all intents and purposes, this is basically a really shitty quadtree
  /// that holds data for rendering a tile of terrain.
  /// </summary>
  public class TerrainTile {
    public Vector2 CenterPosition { get; }
    public float EdgeLength { get; }
    public string Name { get; }

    public int LodLevel { get; }

    public readonly TerrainTile[] Children = { null, null, null, null };

    public TerrainTile(TSCB.TileTableEntry tileMetadata) {
      CenterPosition = new Vector2{ X = tileMetadata.CenterX, Y = tileMetadata.CenterY };
      EdgeLength = tileMetadata.EdgeLength;
      Name = tileMetadata.Name;

      LodLevel = GetLodLevelFromEdgeLength(EdgeLength);
    }

    public static int GetLodLevelFromEdgeLength(float edgeLength) {
      for (int i = 0; i < 10; ++i) {
        if (edgeLength == Math.Pow(2, 9))
          return i;
      }

      return -1;
    }

    /// <summary>
    /// Gets the heightmap for this terrain tile.
    /// </summary>
    /// <returns>Heightmap for this terrain tile</returns>
    public TerrainHeightmap GetHeightmap() {
      return TerrainHeightmapBank.GetOrLoadHeightmap(Name + ".hght");
    }

    /// <summary>
    /// Gets a child at a world position and a certain depth.
    /// </summary>
    /// <param name="pos">World position</param>
    /// <param name="depth">LOD depth</param>
    /// <returns>The tile found, or null if none</returns>
    public TerrainTile GetChildAtLevel(Vector2 pos, int depth = 0) {
      var child = Children[(pos.X < CenterPosition.X ? 0 : 1) + (pos.Y < CenterPosition.Y ? 0 : 2)];

      if (depth == 0)
        return child;

      return child?.GetChildAtLevel(pos, depth - 1);
    }

    /// <summary>
    /// Finds the depest child that satisfies a certain world position, with an optional maximum depth.
    /// </summary>
    /// <param name="pos">The world position to check for a child at</param>
    /// <param name="maxDepth"></param>
    /// <param name="currentDepth"></param>
    /// <returns></returns>
    public TerrainTile GetDeepestChild(Vector2 pos, int maxDepth = -1, int currentDepth = -1) {
      var child = Children[(pos.X < CenterPosition.X ? 0 : 1) + (pos.Y < CenterPosition.Y ? 0 : 2)];

      return child != null ? child.GetDeepestChild(pos) : this;
    }

    /// <summary>
    /// Sets the child at a world position.
    /// </summary>
    /// <param name="pos">World position</param>
    /// <param name="tile">The tile to set</param>
    public void SetChild(Vector2 pos, TerrainTile tile) {
      Children[(pos.X < CenterPosition.X ? 0 : 1) + (pos.Y < CenterPosition.Y ? 0 : 2)] = tile;
    }

    /// <summary>
    /// Gets a child for a tile view, sorted for painters algorithm.
    /// </summary>
    /// <param name="pos">The world position of the origin of the rectangle</param>
    /// <param name="bounds">The bounds for the rectangle</param>
    /// <param name="maxDepth">Maximum depth of the LOD to source</param>
    /// <param name="currentDepth">Used iternally for recursion, don't set</param>
    /// <returns>List of terrain tiles in the rectangle, sorted for painters algorithm</returns>
    public IEnumerable<TerrainTile> GetChildrenForTileView(Vector2 pos, Vector2 bounds, int maxDepth = -1, int currentDepth = 0) {
      // Check if the current depth is too deep
      if (currentDepth > maxDepth)
        yield break;

      // First, check if we're intersecting with the region
      var halfEdge = EdgeLength / 2.0f;
      if (!(CenterPosition.X > pos.X - halfEdge && CenterPosition.Y > pos.Y - halfEdge &&
          CenterPosition.X < pos.X + bounds.X + halfEdge && CenterPosition.Y < pos.Y + bounds.Y + halfEdge))
          yield break;

      // Because the tile view will use painters algorithm, we need to make sure that we add ourselves
      // before the children so that way they will be drawn on top of us.

      // However, if we have full children, then there's no reason to return us.
      if (!(Children.Count(p => p != null) == 4 && currentDepth != maxDepth))
        yield return this;

      // If we have no children, we can safely early-out and return ourselves because
      // we're the best this region is going to get :)
      if (Children.Count(p => p == null) == 4)
        yield break;

      // Otherwise, iterate over each child and recursively call this method.
      // Check if each of the children are in the region.
      foreach (var child in Children) {
        if (child == null)
          continue;

        var recursiveChildren = child.GetChildrenForTileView(pos, bounds, maxDepth, currentDepth + 1);

        foreach (var recursiveChild in recursiveChildren)
          yield return recursiveChild;
      }
    }
  }
}