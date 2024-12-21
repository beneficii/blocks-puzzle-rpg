using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;

namespace TileShapes
{
    public class TetrisShapes
    {
        // Define Tetris shapes as human-readable byte arrays (1 = block, 0 = empty space)
        private static readonly byte[,] IShape =
        {
        { 1, 1, 1, 1 }
    };

        private static readonly byte[,] JShape =
        {
        { 1, 0, 0 },
        { 1, 1, 1 }
    };

        private static readonly byte[,] LShape =
        {
        { 0, 0, 1 },
        { 1, 1, 1 }
    };

        private static readonly byte[,] OShape =
        {
        { 1, 1 },
        { 1, 1 }
    };

        private static readonly byte[,] SShape =
        {
        { 0, 1, 1 },
        { 1, 1, 0 }
    };

        private static readonly byte[,] TShape =
        {
        { 0, 1, 0 },
        { 1, 1, 1 }
    };

        private static readonly byte[,] ZShape =
        {
        { 1, 1, 0 },
        { 0, 1, 1 }
    };

        private static readonly byte[,] TwoTileLine =
        {
        { 1, 1 }
    };

        private static readonly byte[,] ThreeTileLine =
        {
        { 1, 1, 1 }
    };

        private static readonly byte[,] ThreeByThreeSquare =
        {
        { 1, 1, 1 },
        { 1, 1, 1 },
        { 1, 1, 1 }
    };

        private static readonly byte[,] TwoTileDiagonal =
        {
        { 1, 0 },
        { 0, 1 }
    };

        private static readonly byte[,] ThreeTileDiagonal =
        {
        { 1, 0, 0 },
        { 0, 1, 0 },
        { 0, 0, 1 }
    };

        // Method to convert a byte array to List<Vector2Int> for tile positions relative to (0,0)
        private static List<Vector2Int> ConvertToVectorList(byte[,] shape)
        {
            var result = new List<Vector2Int>();

            for (int y = 0; y < shape.GetLength(0); y++)
            {
                for (int x = 0; x < shape.GetLength(1); x++)
                {
                    if (shape[y, x] == 1)
                    {
                        result.Add(new Vector2Int(x, -y)); // Note: -y to align grid origin with Tetris's top-down convention
                    }
                }
            }

            return result;
        }

        public static Shape.Info GetShape(byte[,] shape, TileData tileData = null)
        {
            tileData ??= TileCtrl.current.emptyTile;
            var tiles = ConvertToVectorList(shape)
                .Select(x => new Tile.Info(tileData, x))
                .ToList();

            return new Shape.Info(new(tiles));
        }
    }
}