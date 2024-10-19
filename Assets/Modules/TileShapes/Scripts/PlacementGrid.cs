using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using UnityEngine.Assertions;
using System.Linq;

namespace TileShapes
{
    public class PlacementGrid : MonoBehaviour
    {
        public Vector2Int size;

        Stack<Vector2Int> stack;
        List<Shape.Info> shapes;

        bool[,] grid;

        List<Vector2Int> GetStackList()
        {
            var list = stack.ToList();
            list.Reverse();
            return list;
        }

        bool IsFree(int x, int y)
        {
            if (x < 0 || y < 0 || x >= size.x || y >= size.y) return false;
            return !grid[x,y];
        }

        bool IsFreeOrEmpty(int x, int y)
        {
            if (x < 0 || y < 0 || x >= size.x || y >= size.y) return true;
            return !grid[x, y];
        }

        bool IsFree(Vector2Int pos) => IsFree(pos.x, pos.y);

        bool TryPlace(Shape.Info shape, int x, int y)
        {
            var deltas = shape.GetTilePositions()
                .Select(p => new Vector2Int(x + p.x, y + p.y))
                .ToList();
            foreach (var p in deltas)
            {
                if (!IsFree(p.x, p.y)) return false;

                if (!IsFreeOrEmpty(p.x + 1, p.y)) return false;  // Right
                if (!IsFreeOrEmpty(p.x - 1, p.y)) return false;  // Left
                if (!IsFreeOrEmpty(p.x, p.y + 1)) return false;  // Up
                if (!IsFreeOrEmpty(p.x, p.y - 1)) return false;  // Down

                if (!IsFreeOrEmpty(p.x + 1, p.y + 1)) return false;  // Upper-right diagonal
                if (!IsFreeOrEmpty(p.x - 1, p.y + 1)) return false;  // Upper-left diagonal
                if (!IsFreeOrEmpty(p.x + 1, p.y - 1)) return false;  // Lower-right diagonal
                if (!IsFreeOrEmpty(p.x - 1, p.y - 1)) return false;  // Lower-left diagonal
            }

            foreach (var p in deltas)
            {
                grid[p.x, p.y] = true;
            }

            stack.Push(new(x, y));
            return true;
        }

        private void UndoPlace(Shape.Info shape, int x, int y)
        {
            foreach (var item in shape.GetTilePositions())
            {
                grid[x + item.x, y + item.y] = false;
            }
            stack.Pop();
        }

        public List<Vector2Int> Solve(List<Shape.Info> shapes)
        {
            grid = new bool[size.x, size.y];
            stack = new();
            this.shapes = shapes;
            if (Backtrack(0))
            {
                return GetStackList();
            }

            return null;
        }

        bool Backtrack(int itemIndex)
        {
            if (itemIndex == shapes.Count) return true;

            var currentItem = shapes[itemIndex];

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    if (!TryPlace(currentItem, x, y)) continue;
                    if (Backtrack(itemIndex + 1)) return true;
                    UndoPlace(currentItem, x, y);
                }
            }

            return false; // no valid placement found
        }
    }
}
