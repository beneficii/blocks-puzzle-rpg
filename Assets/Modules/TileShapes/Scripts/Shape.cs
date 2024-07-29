using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using FancyToolkit;
using System.Linq;

namespace TileShapes
{
    public class Shape : MonoBehaviour
    {
        public static System.Action<Shape, bool> OnDragState;

        public System.Action<Shape, Vector2Int> OnDropped;
        public const float offsetDrag = 0f;

        public ShapeData data { get; private set; }
        public int rotation { get; private set; }

        [SerializeField] Tile prefabTile;
        [SerializeField] AudioClip soundPickup;
        [SerializeField] AudioClip soundPlace;

        public Info GetInfo() => new Info(data, rotation);

        List<Tile> blocks;

        Board board;

        bool isDragging;
        Vector3 prevDragPosition;

        void Clear()
        {
            if (blocks == null) return;

            foreach (var item in blocks)
            {
                Destroy(item.gameObject);
            }
            blocks.Clear();
        }

        public void Init(ShapeData data, int rotation, Board board)
        {
            this.board = board;
            Clear();
            blocks = new List<Tile>();
            this.data = data;
            this.rotation = rotation;
            foreach (var item in data.GetTiles(rotation))
            {
                var instance = Instantiate(board.prefabTile, transform);
                instance.SetRenderLayer(Tile.RenderLayer.Inventory);
                instance.transform.localPosition = item.pos.ToFloatVector();
                instance.Init(item);
                blocks.Add(instance);
            }

            SetDragState(false);
        }

        public void Init(Info info, Board board)
        {
            Init(info.data, info.rotation, board);
        }

        Vector2 moveSpeed;

        void CalculateMouseSpeed()
        {
            float height = Helpers.Camera.orthographicSize * 2.0f;
            float width = height * Helpers.Camera.aspect;

            float moveSpeedX = width / Screen.width;
            float moveSpeedY = height / Screen.height;
            moveSpeed = new Vector2(moveSpeedX, moveSpeedY) * 1.32f;
        }

        void SetDragState(bool value)
        {
            isDragging = value;
            OnDragState?.Invoke(this, value);

            if (value)
            {
                CalculateMouseSpeed();
                prevDragPosition = Input.mousePosition;
                transform.localScale = Vector3.one;
                transform.position = Helpers.MouseToWorldPosition() + Vector2.up * offsetDrag;
                soundPickup?.PlayNow();
            }
            else
            {
                transform.localScale = Vector3.one * 0.75f;
            }
        }

        public void OnMouseDown()
        {
            if (FancyInputCtrl.IsMouseOverUI()) return;

            SetDragState(true);
        }

        public Vector2 DragPosition()
            //=> Helpers.MouseToWorldPosition() + Vector2.up * offsetDrag;
            => transform.position + Vector3.up * offsetDrag;


        public void OnMouseUp()
        {
            if (!isDragging) return;

            //DropAt(DragPosition());
            DropAt(transform.position);
            SetDragState(false);
        }

        private void LateUpdate()
        {
            if (!isDragging) return;
            var delta = Input.mousePosition - prevDragPosition;
            //transform.position += DragPosition();
            transform.position += new Vector3(delta.x * moveSpeed.x, delta.y * moveSpeed.y);
            prevDragPosition = Input.mousePosition;
        }

        public bool DropAt(Vector2Int pos)
        {
            var placed = board.PlaceTiles(pos.x, pos.y, data.GetTiles(rotation));
            if (placed != null)
            {
                /*
                foreach (var block in placed)
                {
                    block.SetBg(data.spriteIdx);
                }*/
                OnDropped?.Invoke(this, pos);
                soundPlace?.PlayWithRandomPitch(0.2f);
                Destroy(gameObject);
                return true;
            }
            else
            {
                transform.localPosition = Vector3.zero;
                return false;
            }
        }

        public bool DropAt(Vector2 worldPos)
        {
            var gridPos = board.GetGridPos(worldPos);
            return DropAt(gridPos);
        }


        public class Info
        {
            public ShapeData data;
            public int rotation;

            public Info(ShapeData data, int rotation = 0)
            {
                this.data = data;
                this.rotation = rotation;
            }

            public List<Tile.Info> GetTiles()
            {
                return data.GetTiles(rotation);
            }

            public List<Vector2Int> GetTilePositions()
            {
                return data.GetTiles(rotation)
                    .Select(x => x.pos)
                    .ToList();
            }
        }

    }
}
