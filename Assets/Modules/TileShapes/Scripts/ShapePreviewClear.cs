using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using FancyToolkit;
using GridBoard;
using UnityEngine.Assertions;

namespace TileShapes
{
    public class ShapePreviewClear : MonoBehaviour
    {
        Shape currentShape;

        Vector2Int? cachedGridPos = null;
        int? cachedRotation = null;

        int[] rows;
        int[] columns;

        List<Vector2Int> cachedBlocks = new();

        private void OnEnable()
        {
            Shape.OnDragState += HandleShapeDragState;
        }

        private void OnDisable()
        {
            Shape.OnDragState -= HandleShapeDragState;
        }


        void CalculateTempBoard(Board board)
        {
            columns = new int[board.width];
            rows = new int[board.height];

            for (int x = 0; x < board.width; x++)
            {
                for (int y = 0; y < board.height; y++)
                {
                    if (board.IsOcupied(x, y))
                    {
                        columns[x]++;
                        rows[y]++;
                    }
                }
            }
        }

        void HandleShapeDragState(Shape shape, bool state)
        {
            if (state)
            {
                currentShape = shape;
                CalculateTempBoard(shape.board);
            }
            else
            {
                SetBlockPositions(null);
                currentShape = null;
            }
            cachedBlocks = new();
        }

        void SetBlockPositions(List<Vector2Int> list)
        {
            if (!currentShape) return;

            var board = currentShape.board;
            int tmp;
            foreach (var block in cachedBlocks)
            {
                tmp = --columns[block.x];
                Assert.IsTrue(tmp >= 0);

                tmp = --rows[block.y];
                Assert.IsTrue(tmp >= 0);
            }

            cachedBlocks = list ?? new();

            foreach (var block in cachedBlocks)
            {
                tmp = ++columns[block.x];
                Assert.IsTrue(tmp <= board.height);

                tmp = ++rows[block.y];
                Assert.IsTrue(tmp <= board.width);
            }

            foreach (var item in board.GetAllTiles())
            {
                item.SetPreviewColor(columns[item.position.x] == board.width || rows[item.position.y] == board.height);
            }

            if (currentShape.tiles.Count == cachedBlocks.Count)
            {
                for (int i = 0; i < cachedBlocks.Count; i++)
                {
                    var pos = cachedBlocks[i];
                    var tile = currentShape.tiles[i];

                    tile.SetPreviewColor(columns[pos.x] == board.width || rows[pos.y] == board.height);
                }
            }
            else if (cachedBlocks.Count == 0)
            {
                foreach (var tile in currentShape.tiles)
                {
                    tile.SetPreviewColor(false);
                }
            }
            else
            {
                Debug.LogError("Shape and list count dont match");
            }
        }

        private void Update()
        {
            if (!currentShape) return;
            var board = currentShape.board;

            var gridPos = board.CheckGridPos(currentShape.transform.position);
            if (gridPos == cachedGridPos && currentShape.rotation == cachedRotation) return;
            cachedGridPos = gridPos;
            cachedRotation = currentShape.rotation;

            if (gridPos.HasValue)
            {
                var list = board.GetTileGridPositions(gridPos.Value, currentShape.GetInfo().GetTiles(), true);
                SetBlockPositions(list);
            }
            else
            {
                SetBlockPositions(null);
            }
        }
    }
}