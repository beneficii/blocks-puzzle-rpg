using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using FancyToolkit;
using GridBoard;

namespace TileShapes
{
    public class ShapeShadowCtrl : MonoBehaviour
    {
        [SerializeField] Sprite spriteBlock;
        [SerializeField] Color color;
        [SerializeField] int sortingOrder = 21;

        [SerializeField] Board board;

        List<Transform> blocks = new();
        int blocksUsed = 0;

        Shape currentShape;

        Vector2Int? cachedGridPos = null;

        List<Transform> GetBlocks(int count)
        {
            // Create more blocks if needed
            while (count > blocks.Count)
            {
                var obj = new GameObject();
                var render = obj.AddComponent<SpriteRenderer>();
                render.sprite = spriteBlock;
                render.color = color;
                render.sortingOrder = sortingOrder;

                blocks.Add(obj.transform);
            }

            while (count > blocksUsed)
            {
                blocks[blocksUsed].gameObject.SetActive(true);
                blocksUsed++;
            }

            while (blocksUsed > count)
            {
                blocks[--blocksUsed].gameObject.SetActive(false);
            }

            return blocks.Take(count).ToList();
        }


        private void OnEnable()
        {
            Shape.OnDragState += HandleShapeDragState;
        }

        private void OnDisable()
        {
            Shape.OnDragState -= HandleShapeDragState;
        }

        void Clear()
        {
            GetBlocks(0);
        }

        void HandleShapeDragState(Shape shape, bool state)
        {
            if (state)
            {
                currentShape = shape;
            }
            else
            {
                currentShape = null;
                Clear();
            }
        }

        private void Start()
        {
            //board = FindAnyObjectByType<Board>();
        }

        private void Update()
        {
            if (!currentShape) return;

            var gridPos = board.CheckGridPos(currentShape.transform.position);
            if (gridPos == cachedGridPos) return;
            cachedGridPos = gridPos;

            if (gridPos.HasValue)
            {
                var list = board.GetTilePositions(gridPos.Value, currentShape.GetInfo().GetTiles(), true);
                if (list != null)
                {
                    int idx = 0;
                    foreach (var item in GetBlocks(list.Count))
                    {
                        item.position = list[idx++];
                    }
                }
                else
                {
                    Clear();
                }
            }
            else
            {
                Clear();
            }
        }
    }
}