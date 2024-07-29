using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GridBoard;
using System.Linq;

namespace TileShapes
{
    public class ShapePanel : MonoBehaviour
    {
        public System.Action<bool> OnShapesGenerated;
        public System.Action OnOutOfShapes;
        public System.Action OnDeadEnd;
        public UnityEvent<bool> OnHintsAviable;


        [SerializeField] Shape prefabShape;
        [SerializeField] List<Transform> slots;
        [SerializeField] Board board;
        
        List<Shape> shapes = new();

        List<ShapeData> pool;
        int poolIdx = 0;

        bool shouldCheckSlots;

        Queue<Hint> hints;

        Shape.Info GetWispShape()
        {
            var tiles = new List<Tile.Info>() { new (TileCtrl.current.emptyTile, Vector2Int.zero) };
            return new Shape.Info(new(tiles));
        }

        public ShapeData GetShapeFromPool()
        {
            if (pool == null) GeneratePool();
            var shape = pool[poolIdx];
            if (++poolIdx >= pool.Count)
            {
                GeneratePool();
            }

            return shape;
        }

        void Init()
        {
            board.OnChangedLate += CheckSlots;
        }

        private void Awake()
        {
            Init();
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

        void SetSlotShape(Transform slot, Shape.Info info)
        {
            var instance = Instantiate(prefabShape, slot.position, slot.rotation, slot);
            instance.Init(info, board);
            shapes.Add(instance);
            instance.OnDropped += HandleShapeDropped;
        }

        public void GenerateNew(bool initial = true)
        {
            Clear();

            var tempGrid = new SimulatedBoard(board);
            hints = new();
            OnHintsAviable?.Invoke(true);

            foreach (var slot in slots)
            {
                var info = GetNextShape(tempGrid);
                SetSlotShape(slot, info);
            }

            OnShapesGenerated?.Invoke(initial);
            //BtSave.Create();
        }

        public void GeneratePool()
        {
            pool = ShapeManager.current.shapes
                .OrderBy(x => System.Guid.NewGuid())
                .ToList();
            poolIdx = 0;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                GenerateNew(true);
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                HintCtrl.current.Show(hints);
            }

            int upgradeLevel = -1;
            if (Input.GetKeyDown(KeyCode.Alpha0)) upgradeLevel = 0;
            if (Input.GetKeyDown(KeyCode.Alpha1)) upgradeLevel = 1;
            if (Input.GetKeyDown(KeyCode.Alpha2)) upgradeLevel = 2;
            if (Input.GetKeyDown(KeyCode.Alpha3)) upgradeLevel = 3;

            if (upgradeLevel >= 0)
            {
                //BtUpgradeCtrl.current.Show((BtUpgradeRarity)upgradeLevel, 3);
            }
        }
#endif

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

        public void CheckSlots()
        {
            if (!shouldCheckSlots) return;

            shouldCheckSlots = false;
            if (shapes.Count == 0)
            {
                OnOutOfShapes?.Invoke();
                return;
            }

            CheckDeadEnd();
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
            }
        }


        IEnumerator AutoPlay()
        {
            while (true)
            {
                yield return AutoPlayTurn();
                yield return new WaitForSeconds(0.1f);
            }

        }
    }
}
