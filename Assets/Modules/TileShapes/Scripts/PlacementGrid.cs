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
            gizmoPrint = true;
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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D)) { debugTwoStep = !debugTwoStep; gizmoPrint = true; } 
        }

        public bool debugTwoStep;
        bool gizmoPrint = true;/*
        private void OnDrawGizmos()
        {
            if (grid == null) return;

            bool shouldPrint = gizmoPrint;
            gizmoPrint = false;

            Gizmos.color = Color.blue;
            var wSize = new Vector2(size.x, size.y) * Tile.rScale;
            //Gizmos.DrawWireCube((Vector2)transform.position + wSize / 2f, wSize);


            var preCubes = new List<Vector2Int>();
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    // Determine the position of the current cell in the 2D space
                    Vector3 position = new Vector3(x * Tile.rScale, y * Tile.rScale, 0);

                    // Set Gizmo color based on whether the cell is filled or empty
                    Gizmos.color = grid[x, y] ? Color.blue : Color.gray;
                    if (grid[x, y]) preCubes.Add(new(x, y));

                    // Draw the cell as a filled or wire square in 2D space
                    Gizmos.DrawCube(transform.position + position, new Vector3(Tile.rScale, Tile.rScale, .1f));
                }
            }

            if (shouldPrint)
            {
            print("--- greid --");
            print(string.Join(", ", preCubes.OrderBy(p => p.x).ThenBy(p => p.y).ToList()));
                print(preCubes.Count);
                preCubes.Clear();
            }
            if (!debugTwoStep) return;

            void DrawCube(Shape.Info shape, Vector2Int pos, Color color)
            {
                Gizmos.color = color;
                foreach (var p in shape.GetTilePositions())
                {
                    preCubes.Add(new((pos.x + p.x), (pos.y + p.y)));
                    Vector3 position = new Vector3((pos.x + p.x) * Tile.rScale, (pos.y + p.y) * Tile.rScale, 0);
                    Gizmos.DrawCube(transform.position + position, new Vector3(Tile.rScale, Tile.rScale, .1f));
                }
            }

            var slist = GetStackList();
            DrawCube(shapes[0], slist[0], Color.red);
            DrawCube(shapes[1], slist[1], Color.green);
            DrawCube(shapes[2], slist[2], Color.yellow);

            if (shouldPrint)
            {
                print("--- speas --");
                print(string.Join(", ", preCubes.OrderBy(p => p.x).ThenBy(p => p.y).ToList()));
                print(preCubes.Count);
                preCubes.Clear();
            }
        }*/
    }
}
