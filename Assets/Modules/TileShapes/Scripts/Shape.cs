using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using FancyToolkit;
using System.Linq;
using UnityEditor.Build;

namespace TileShapes
{
    public class Shape : MonoBehaviour
    {
        public static System.Action<Shape, bool> OnDragState;

        public System.Action<Shape, Vector2Int> OnDropped;
        public const float offsetDrag = 0f;

        public ShapeData data { get; private set; }
        public int rotation { get; private set; }

        [SerializeField] AudioClip soundPickup;
        [SerializeField] AudioClip soundPlace;

        public Info GetInfo() => new Info(data, rotation);

        List<Tile> tiles;

        Board board;
        ShapePanel parent;

        bool shouldDestroy;

        bool isDragging;
        Vector3 prevDragPosition;

        void Clear()
        {
            if (tiles == null) return;

            foreach (var item in tiles)
            {
                Destroy(item.gameObject);
            }
            tiles.Clear();
        }

        public void Init(ShapeData data, int rotation, Board board, ShapePanel parent)
        {
            this.board = board;
            this.parent = parent;
            Clear();
            tiles = new List<Tile>();
            this.data = data;
            this.rotation = rotation;
            foreach (var item in data.GetTiles(rotation))
            {
                var instance = Instantiate(board.prefabTile, transform);
                instance.SetRenderLayer(Tile.RenderLayer.Inventory);
                instance.transform.localPosition = Tile.IndexToPos(item.pos);
                instance.position = item.pos;
                instance.Init(item.data);
                tiles.Add(instance);
            }

            SetDragState(false);
            StartCoroutine(UpdateShapeCollider());
        }

        public void Init(Info info, Board board, ShapePanel parent)
        {
            Init(info.data, info.rotation, board, parent);
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
                //CalculateMouseSpeed();
                //prevDragPosition = Input.mousePosition;
                transform.localScale = Vector3.one;
                transform.position = Helpers.MouseToWorldPosition(); //+ Vector2.up * offsetDrag;
                soundPickup?.PlayNow();
            }
            else
            {
                transform.localScale = Vector3.one;// * 0.75f;
            }
        }

        public void SetHighlight(bool value)
        {
            foreach (var item in tiles)
            {
                item.SetHighlight(value);
            }
        }

        public void MouseDown()
        {
            if (parent.IsLocked) return;
            if (FancyInputCtrl.IsMouseOverUI()) return;

            SetDragState(true);
        }

        public Vector2 DragPosition()
            //=> Helpers.MouseToWorldPosition() + Vector2.up * offsetDrag;
            => transform.position + Vector3.up * offsetDrag;

        public void MouseUp()
        {
            if (!isDragging) return;

            SetDragState(false);
            StartCoroutine(MouseDrop(transform.position));
        }

        IEnumerator MouseDrop(Vector2 position)
        {
            //DropAt(DragPosition());
            yield return DropAt(position);
            if (shouldDestroy) Destroy(gameObject);
        }


        IEnumerator UpdateShapeCollider()
        {
            yield return new WaitForSeconds(.1f); // ToDo: figure out why shapes not intializing sometimes
            var shapeCollider = GetComponent<BoxCollider2D>();
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
            bool boundsSet = false;

            foreach (var child in tiles)
            {
                Collider2D tileCollider = child.GetComponent<Collider2D>();
                var prevBounds = bounds;
                if (!boundsSet)
                {
                    bounds = tileCollider.bounds;
                    boundsSet = true;
                }
                else
                {
                    bounds.Encapsulate(tileCollider.bounds);
                }
                boundsSet = true;
            }

            shapeCollider.offset = bounds.center - transform.position;
            shapeCollider.size = bounds.size;
        }

        private void LateUpdate()
        {
            if (!isDragging) return;
            //var delta = Input.mousePosition - prevDragPosition;
            //transform.position += DragPosition();
            //transform.position += new Vector3(delta.x * moveSpeed.x, delta.y * moveSpeed.y);
            //prevDragPosition = Input.mousePosition;
            transform.position = Helpers.MouseToWorldPosition();
        }

        public IEnumerator DropAt(Vector2Int pos)
        {
            var result = board.PlaceTiles(pos.x, pos.y, data.GetTiles(rotation));
            if (result != null)
            {
                // prevent calculating until OnPlace Actions execute
                board.SetShouldCalculate(false);
                board.GenerateEmptyTileQueue();
                //foreach (var block in placed) block.SetBg(data.spriteIdx);
                OnDropped?.Invoke(this, pos);
                soundPlace?.PlayWithRandomPitch(0.2f);
                transform.localScale = Vector3.zero;
                shouldDestroy = true;

                board.UnlockAllTileActions();

                // make sure on placed effects don't affect this shape
                foreach (var tile in result) tile.isBeingPlaced = true;
                foreach (var tile in result) yield return tile.Place();
                foreach (var tile in result) tile.isBeingPlaced = false;
                board.SetShouldCalculate(true);
            }
            else
            {
                transform.localPosition = Vector3.zero;
            }
        }

        public IEnumerator DropAt(Vector2 worldPos)
        {
            var gridPos = board.GetGridPos(worldPos);
            yield return DropAt(gridPos);
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
