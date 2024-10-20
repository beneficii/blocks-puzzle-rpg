using FancyToolkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace GridBoard
{
    public class LineClearer : MonoBehaviour, Board.IModule
    {
        [SerializeField] AudioClip soundMatchOne;
        [SerializeField] AudioClip soundMatchMultiple;

        int[] rows;
        int[] columns;

        int width, height;

        Board board;

        static HashSet<ILineClearHandler> lineClearHandlers = new();

        public void InitBoard(Board board)
        {
            if (this.board) return;
            this.board = board;

            width = board.width;
            height = board.height;
            columns = new int[width];
            rows = new int[height];
        }

        private void OnEnable()
        {
            if (!board) InitBoard(GetComponent<Board>());

            board.OnTilePlaced += HandleTilePlaced;
            board.OnTileRemoved += HandleTileRemoved;
            board.OnChanged += HandleBoardChanged;
        }

        private void OnDisable()
        {
            board.OnTilePlaced -= HandleTilePlaced;
            board.OnTileRemoved -= HandleTileRemoved;
            board.OnChanged -= HandleBoardChanged;
        }

        public static void AddHandler(ILineClearHandler handler)
        {
            lineClearHandlers.Add(handler);
        }

        public static void RemoveHandler(ILineClearHandler handler)
        {
            lineClearHandlers.Remove(handler);
        }

        void HandleBoardChanged()
        {
            var removeColumns = new List<int>();
            var removeRows = new List<int>();

            var tilesRemoved = new List<Tile>();

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
                    var tile = board.PopOutAt(x, y);
                    if (tile) tilesRemoved.Add(tile);
                }
            }

            foreach (var y in removeRows)
            {
                for (int x = 0; x < width; x++)
                {
                    var tile = board.PopOutAt(x, y);
                    if (tile) tilesRemoved.Add(tile);
                }
            }

            int totalCleared = removeColumns.Count + removeRows.Count;

            if (totalCleared > 0)
            {
                // ToDo: maybe move sound to tiles themselves
                if (totalCleared < 3)
                {
                    soundMatchOne?.PlayWithRandomPitch(0.2f);
                }
                else
                {
                    soundMatchMultiple?.PlayWithRandomPitch(0.1f);
                }

                StartCoroutine(ClearRoutine(tilesRemoved, removeRows.Count, removeColumns.Count));
            }
        }

        IEnumerator ClearRoutine(List<Tile> tilesRemoved, int rowsCount, int columnsCount)
        {
            var tileSet = new HashSet<Tile>();
            foreach (var item in tilesRemoved)
            {
                if (!item.data.isEmpty)
                {
                    tileSet.Add(item);
                }
            }

            var clearData = new LineClearData(tileSet, rowsCount, columnsCount);

            foreach (var handler in lineClearHandlers)
            {
                yield return handler.HandleLinesCleared(clearData);
            }

            yield return new WaitForSeconds(.2f);
            tileSet = new HashSet<Tile>(tilesRemoved);
            foreach (var item in tileSet)
            {
                yield return item.FadeOut(10f);
                item.OnRemoved();
                Destroy(item.gameObject);
            }
        }
        

        void HandleTilePlaced(Tile tile)
        {
            var pos = tile.position;
            rows[pos.y]++;
            columns[pos.x]++;
            Assert.IsTrue(rows[pos.y] <= height);
            Assert.IsTrue(columns[pos.x] <= width);
        }

        void HandleTileRemoved(Tile tile)
        {
            var pos = tile.position;
            rows[pos.y]--;
            columns[pos.x]--;

            Assert.IsTrue(rows[pos.y] >= 0);
            Assert.IsTrue(columns[pos.x] >= 0);
        }
    }

    public interface ILineClearHandler
    {
        public IEnumerator HandleLinesCleared(LineClearData clearData);
    }

    public class LineClearData
    {
        public List<TileData> list;
        public HashSet<Tile> tiles;
        Queue<Tile> queue;

        public int rowsMatched;
        public int columnsMatched;

        public LineClearData(HashSet<Tile> blocks, int rowsMatched, int columnsMatched)
        {
            this.tiles = blocks;
            this.list = blocks.Select(x=>x.data).ToList();
            queue = new Queue<Tile>(blocks.OrderByDescending(x => x.data.priority));
            this.rowsMatched = rowsMatched;
            this.columnsMatched = columnsMatched;
        }

        public Tile PickNextTile()
        {
            while (queue.Count > 0)
            {
                var tile = queue.Dequeue();
                if (tiles.Contains(tile)) return tile;
            }

            return null;
        }
    }
}
