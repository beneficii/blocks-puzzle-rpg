using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FancyToolkit
{
    public partial class SpatialHashGrid<TObj>
        where TObj : MonoBehaviour
    {
        public readonly int width = 20;
        public readonly int height = 20;
        public readonly Vector2Int cellSize;

        LinkedList<Container> objList = new();
        Dictionary<TObj, LinkedListNode<Container>> objDict = new();

        readonly Cell[,] cells;

        public SpatialHashGrid(int width, int height, Vector2Int cellSize)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;

            cells = new Cell[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var worldPos = new Vector2((x + 0.5f) * cellSize.x, (y + 0.5f) * cellSize.y);
                    var cell = new Cell(this, x, y, cellSize, worldPos);
                    cells[x, y] = cell;
                }
            }
        }

        public Container Add(TObj obj)
        {
            var container = new Container(this, obj);
            container.RefreshPosition();
            var node = objList.AddLast(container);
            objDict.Add(obj, node);

            return container;
        }

        public bool Remove(TObj obj)
        {
            if (objDict.TryGetValue(obj, out var node))
            {
                objList.Remove(node);
                var container = node.Value;
                objDict.Remove(obj);
                container.Remove();
                return true;
            }
            return false;
        }

        public Cell GetCell(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return null;

            var cell = cells[x, y];

            return cell;
        }

        public Cell GetCell(Vector2 pos)
            => GetCell((int)(pos.x / cellSize.x), (int)(pos.y / cellSize.y));

        public IEnumerable<Cell> GetCells() 
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    yield return cells[x, y];
                }
            }
        }


        public void Update()
        {
            List<TObj> toRemove = new();
            foreach (var item in objList)
            {
                if (!item.obj)
                {
                    toRemove.Add(item.obj);
                    return;
                }

                item.Update();
            }

            foreach (var item in toRemove)
            {
                Remove(item);
            }
        }
    }
}
