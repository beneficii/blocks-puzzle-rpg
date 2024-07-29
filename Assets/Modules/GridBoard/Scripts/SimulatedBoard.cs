using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

namespace GridBoard
{
    public class SimulatedBoard
    {
        public bool[,] ocupied;
        int width;
        int height;

        int[] rows;
        int[] columns;

        List<Vector2Int> searchPoints;

        public Vector2Int hint { get; private set; }

        void MakeRandomSearchPoints()
        {
            searchPoints = new List<Vector2Int>();
            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    searchPoints.Add(new Vector2Int(x, y));
                }
            }

            searchPoints.Shuffle();
        }

        bool InBounds(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return false;

            return true;
        }

        public SimulatedBoard(Board board)
        {
            width = board.width;
            height = board.height;
            ocupied = new bool[width, height];
            columns = new int[width];
            rows = new int[height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (board.IsOcupied(x, y))
                    {
                        ocupied[x, y] = true;
                        columns[x]++;
                        rows[y]++;
                    }
                }
            }

            MakeRandomSearchPoints();
        }

        void CheckLines()
        {
            var removeColumns = new List<int>();
            var removeRows = new List<int>();

            for (int i = 0; i < width; i++)
            {
                if (columns[i] >= height)
                {
                    removeColumns.Add(i);
                }
            }

            for (int i = 0; i < height; i++)
            {
                if (rows[i] >= width)
                {
                    removeRows.Add(i);
                }
            }

            foreach (var x in removeColumns)
            {
                for (int y = 0; y < height; y++)
                {
                    if (ocupied[x, y])
                    {
                        ocupied[x, y] = false;
                        columns[x]--;
                        rows[y]--;
                    }
                }
            }

            foreach (var y in removeRows)
            {
                for (int x = 0; x < width; x++)
                {
                    if (ocupied[x, y])
                    {
                        ocupied[x, y] = false;
                        columns[x]--;
                        rows[y]--;
                    }
                }
            }
        }

        bool CanFit(Vector2Int origin, List<Vector2Int> tiles)
        {
            foreach (var item in tiles)
            {
                var pos = origin + item;

                if (!InBounds(pos.x, pos.y)) return false;
                if (ocupied[pos.x, pos.y]) return false;
            }

            return true;
        }

        public bool AddPiece(List<Vector2Int> tiles)
        {
            foreach (var pos in searchPoints)
            {
                if (CanFit(pos, tiles))
                {
                    foreach (var block in tiles)
                    {
                        var bpos = pos + block;

                        ocupied[bpos.x, bpos.y] = true;
                        columns[bpos.x]++;
                        rows[bpos.y]++;
                    }
                    CheckLines();
                    hint = pos;
                    return true;
                }
            }

            return false;
        }
    }
}