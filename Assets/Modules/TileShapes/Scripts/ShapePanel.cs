﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using GridBoard;
using System.Linq;
using FancyToolkit;

namespace TileShapes
{
    public class ShapePanel : MonoBehaviour, IWorldUpdateListener
    {
        public System.Action<bool> OnShapesGenerated;
        public CoroutineEvent OnOutOfShapes = new();
        public System.Action OnDeadEnd;
        public UnityEvent<bool> OnHintsAviable;

        [SerializeField] PlacementGrid testGrid;

        [SerializeField] Shape prefabShape;
        [SerializeField] List<Transform> slots;
        [SerializeField] Board board;
        [SerializeField] Transform anchor;

        [SerializeField] LayerMask layerMask;

        Transform currentColider;
        Shape currentShapeUnderMouse;
        Shape currentDraggedShape;

        List<Shape> shapes = new();

        List<ShapeData> pool;
        int poolIdx = 0;

        bool shouldCheckSlots;

        Queue<Hint> hints;
        Queue<TileData> bonuseTileQueue = new();
        

        public bool NeedsUpdate => shouldCheckSlots;

        Shape.Info GetWispShape()
        {
            var tiles = new List<Tile.Info>() { new (TileCtrl.current.emptyTile, Vector2Int.zero) };
            return new Shape.Info(new(tiles));
        }

        Vector2 AnchoredPosition(Vector2Int pos)
        {
            return (Vector2)anchor.transform.position + new Vector2(pos.x * Tile.rScale, pos.y * Tile.rScale);
        }

        public Vector2 GetShapeMidPos(int index = 0)
        {
            return slots[index].transform.position;
        }

        public ShapeData GetShapeFromPool()
        {
            if (pool == null) GeneratePool();
            var shape = pool[poolIdx];
            if (++poolIdx >= pool.Count)
            {
                GeneratePool();
            }

            return new(shape);
        }

        Shape.Info GetNextShape(SimulatedBoard boardState)
        {
            int rotation = Random.Range(0, 4);

            ShapeData shape = null;
            for (int i = 0; i < 30; i++)
            {
                shape = GetShapeFromPool();
                for (int j = 0; j < 4; j++)
                {
                    int rot = (rotation + j) % 4;
                    var info = new Shape.Info(shape, rot);

                    if (boardState.AddPiece(info.GetTilePositions()))
                    {
                        hints.Enqueue(new Hint(info, boardState.hint));
                        return info;
                    }
                }
            }


            // give a block that fits everywhere
            var wispShape = GetWispShape();
            if (boardState.AddPiece(wispShape.GetTilePositions()))
            {
                hints.Enqueue(new Hint(wispShape, boardState.hint));
                return wispShape;
            }

            // Should be imposible
            Debug.LogError("Nothing fits!");
            return new Shape.Info(shape, rotation);
        }


        void Clear()
        {
            foreach (var item in shapes)
            {
                if (item) Destroy(item.gameObject);
            }

            shapes.Clear();
        }

        public Shape CreateClone(Shape shape)
        {
            var slot = shape.transform.parent;
            if (!slot)
            {
                Debug.LogError("Could not get shape slot");
                return null;
            }


            var instance = Instantiate(prefabShape, slot.position, slot.rotation, slot);
            instance.InitClone(shape);
            instance.Init(shape.data, shape.rotation, board, this);

            return instance;
        }

        void SetSlotShape(Transform slot, Shape.Info info)
        {
            var instance = Instantiate(prefabShape, slot.position, slot.rotation, slot);
            instance.Init(info, board, this);
            shapes.Add(instance);
            instance.OnDropped += HandleShapeDropped;
        }

        void PopulateShapesWithTiles(Queue<Shape.Info> shapes, PoolQueue<TileData> tileQueue, int tilesToPopulate)
        {
            if (tileQueue == null || tilesToPopulate == 0) return;

            var tileslots = shapes.SelectMany(x => x.data.GetTilesRaw()).RandN(tilesToPopulate);
            foreach (var item in tileslots)
            {
                item.data = tileQueue.Get();
            }
        }

        void PopulateShapesWithBonusTiles(Queue<Shape.Info> shapes)
        {
            var count = bonuseTileQueue.Count;
            if (count == 0) return;

            var tileslots = shapes.SelectMany(x => x.data.GetTilesRaw()).RandN(count);
            foreach (var item in tileslots)
            {
                item.data = bonuseTileQueue.Dequeue();
            }
        }

        public void AddBonusTile(TileData data, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                bonuseTileQueue.Enqueue(data);
            }
        }

        public void GenerateNew(bool initial = true, PoolQueue<TileData> tileQueue = null, int tilesToPopulate = 0, int singles = 0)
        {
            Clear();

            var tempGrid = new SimulatedBoard(board);
            hints = new();
            OnHintsAviable?.Invoke(true);
            var nextShapes = new Queue<Shape.Info>();

            for (int i = 0; i < slots.Count - singles; i++)
            {
                nextShapes.Enqueue(GetNextShape(tempGrid));
            }

            for (int i = 0; i < singles; i++)
            {
                nextShapes.Enqueue(GetWispShape());
            }

            PopulateShapesWithTiles(nextShapes, tileQueue, tilesToPopulate);
            PopulateShapesWithBonusTiles(nextShapes);

            var solution = testGrid.Solve(nextShapes.ToList());
            //if (solution != null) { Debug.Log($"Solve: {string.Join(", ", solution)}"); }
            //else print("Solution failed");

            //if (solution != null) StartCoroutine(RefreshAgain());

            if (solution != null)
            {
                for (int i = 0; i < solution.Count; i++)
                {
                    slots[i].transform.position = AnchoredPosition(solution[i]);
                }
            }
            else
            {
                Debug.LogError("Could not find solution. ToDo: handle this");
            }
            
            foreach (var slot in slots)
            {
                SetSlotShape(slot, nextShapes.Dequeue());
            }

            OnShapesGenerated?.Invoke(initial);
        }

        public void GenerateCustom(List<Shape.Info> shapes)
        {
            Clear();

            OnHintsAviable?.Invoke(false);
            var nextShapes = new Queue<Shape.Info>(shapes);

            var solution = testGrid.Solve(nextShapes.ToList());
            if (solution != null)
            {
                for (int i = 0; i < solution.Count; i++)
                {
                    slots[i].transform.position = AnchoredPosition(solution[i]);
                }
            }
            else
            {
                Debug.LogError("Could not find solution. ToDo: handle this");
            }

            foreach (var slot in slots)
            {
                if (!nextShapes.TryDequeue(out var shape)) break;
                SetSlotShape(slot, shape);
            }

            OnShapesGenerated?.Invoke(false);
        }

        public void GenerateFromBytes(List<byte[,]> shapes)
        {
            var list = shapes
                .Select(x=>TetrisShapes.GetShape(x))
                .ToList();

            GenerateCustom(list);
        }

        public void GeneratePool()
        {
            pool = ShapeManager.current.shapes
                .OrderBy(x => System.Guid.NewGuid())
                .ToList();
            poolIdx = 0;
        }

        void SetShapeUnderMouse(Transform trans)
        {
            if (currentShapeUnderMouse)
            {
                currentShapeUnderMouse.SetHighlight(false);
            }

            currentColider = trans;
            currentShapeUnderMouse = trans ? trans.GetComponent<Shape>() : null;

            if (currentShapeUnderMouse)
            {
                currentShapeUnderMouse.SetHighlight(true);
            }
        }


        void HandleMouseShapeGrab()
        {
            var clone = CreateClone(currentShapeUnderMouse);
            currentShapeUnderMouse.gameObject.SetActive(false);

            currentDraggedShape = clone;
            currentDraggedShape.MouseDown();
            //currentShapeUnderMouse.MouseDown();
            //currentDraggedShape = currentShapeUnderMouse;
        }

        void HandleMouseShapeRelease()
        {
            currentDraggedShape.MouseUp();
            currentDraggedShape = null;
        }

        bool HandleMouseScroll()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f) // Scroll up
            {
                currentDraggedShape.Rotate(-1);
                return true;
            }
            else if (scroll < 0f) // Scroll down
            {
                currentDraggedShape.Rotate(+1);
                return true;
            }

            return false;
        }

        public float shapeDragStartTime;
        public Vector2 shapeDragStartPos;
        private void Update()
        {
            if (WorldUpdateCtrl.current.IsUpdating) return;
            if (FancyInputCtrl.IsMouseOverUI()) return;

            RaycastHit2D hit = Physics2D.Raycast(Helpers.MouseToWorldPosition(), Vector2.zero, 10, layerMask);
            var trans = hit.transform;
            if (currentColider != trans)
            {
                SetShapeUnderMouse(trans);
            }

            //if (Input.GetMouseButtonDown(0))
            if (currentDraggedShape == null && Input.GetMouseButtonDown(0))
            {
                if (currentShapeUnderMouse)
                {
                    shapeDragStartTime = Time.time;
                    shapeDragStartPos = Input.mousePosition;

                    HandleMouseShapeGrab();
                }
            }
            else if (currentDraggedShape)
            {
                // use drag controls
                if (Input.GetMouseButtonUp(0) && (Time.time - shapeDragStartTime > .4f || Vector2.Distance((Vector2)Input.mousePosition, shapeDragStartPos) > 40))
                {
                    HandleMouseShapeRelease();
                }
                // use click controls
                else if (Input.GetMouseButtonDown(0))
                {
                    HandleMouseShapeRelease();
                }
                // rotate shape
                else if (Input.GetMouseButtonDown(1))
                {
                    currentDraggedShape.Rotate();
                }
                else HandleMouseScroll();
            }
        }

        public bool HasHints()
        {
            return hints != null && hints.Count > 0;
        }

        public void BtnShowHint()
        {
            HintCtrl.current.Show(hints);
            //CombatArena.current.player.RemoveHp(2);
        }

        public void BtnGenerateNewShapes()
        {
            GenerateNew(false);
        }

        public void BtnLoadSave()
        {
            //BtSave.Load();
        }

        public void BtnAutoplay()
        {
            StartCoroutine(AutoPlayTurn());
        }

        public void CheckDeadEnd()
        {
            foreach (var shape in shapes)
            {
                var tempGrid = new SimulatedBoard(board);
                if (tempGrid.AddPiece(shape.GetInfo().GetTilePositions()))
                {
                    return; // all good
                }
            }

            OnDeadEnd?.Invoke();
            //StartCoroutine(RoutineStuck());
        }

        void HandleShapeDropped(Shape shape, Vector2Int pos)
        {
            shapes.Remove(shape);
            shouldCheckSlots = true;
            if (hints != null && hints.Count > 0)
            {
                if (hints.Peek().Matches(shape.GetInfo(), pos))
                {
                    hints.Dequeue();
                }
                else
                {
                    OnHintsAviable?.Invoke(false);
                    hints = null;
                }
            }
        }

        public List<Shape.Info> GetCurrentShapes()
        {
            var result = new List<Shape.Info>();
            //foreach (var slot in slots)
            foreach (var shape in shapes)
            {
                //var shape = slot.GetComponentInChildren<Shape>();
                if (shape)
                {
                    result.Add(shape.GetInfo());
                }
                else
                {
                    result.Add(null);
                }
            }

            return result;
        }

        public List<Hint> GetCurrentHints()
        {
            return new List<Hint>(hints);
        }

        public void SetCurrentShapes(List<Shape.Info> infos, List<Hint> hints)
        {
            Clear();
            int idx = 0;
            foreach (var slot in slots)
            {
                var info = infos[idx++];
                if (info == null) continue;

                SetSlotShape(slot, info);
            }
            this.hints = new(hints);
        }

        IEnumerator AutoPlayTurn()
        {
            if (hints == null || hints.Count == 0) yield break;

            /*
            int idx = 0;
            var list = new List<Hint>(hints);
            foreach (var hint in list)
            {
                var slot = slots[idx++];
                var shape = slot.GetComponentInChildren<Shape>();
                if (!shape) yield break;

                var dropped = shape.DropAt(hint.pos);
                if (!dropped)
                {
                    Debug.LogError("Could not drop");
                    yield break;
                }

                yield return new WaitForSeconds(0.2f);
            }*/
        }


        IEnumerator AutoPlay()
        {
            while (true)
            {
                yield return AutoPlayTurn();
                yield return new WaitForSeconds(0.1f);
            }
        }

        public IEnumerator WorldUpdate()
        {
            shouldCheckSlots = false;
            if (shapes.Count == 0)
            {
                yield return OnOutOfShapes.Invoke();
                yield break;
            }

            CheckDeadEnd();
        }
    }
}
