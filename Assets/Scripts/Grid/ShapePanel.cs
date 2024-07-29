using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FancyToolkit;

[DefaultExecutionOrder(+1)]
public class ShapePanel : MonoBehaviour
{
    static ShapePanel _current;
    public static ShapePanel current
    {
        get
        {
            if (!_current)
            {
                _current = FindFirstObjectByType<ShapePanel>();
            }

            return _current;
        }
    }

    public static System.Action<bool> OnShapesGenerated;
    public static System.Action OnOutOfShapes;
    public UnityEvent<bool> OnHintsAviable;


    [SerializeField] List<Transform> slots;
    List<BtShape> shapes = new();

    List<BtShapeData> pool;
    int poolIdx = 0;

    bool shouldCheckSlots;

    Queue<BtHint> hints;

    public BtShapeData GetShapeFromPool()
    {
        if (pool == null) GeneratePool();
        var shape = pool[poolIdx];
        if (++poolIdx >= pool.Count)
        {
            GeneratePool();
        }

        return shape;
    }

    BtShapeInfo GetNextShape(BtGrid.TempGridState gridState)
    {
        int rotation = Random.Range(0, 4);

        BtShapeData shape = null;
        for (int i = 0; i < 30; i++)
        {
            shape = GetShapeFromPool();
            for (int j = 0; j < 4; j++)
            {
                int rot = (rotation + j) % 4;
                var info = new BtShapeInfo(shape, rot);

                if (gridState.AddPiece(info))
                {
                    hints.Enqueue(new BtHint(info, gridState.hint));
                    return info;
                }
            }
        }


        // give a block that fits everywhere
        var wispShape = BtGrid.current.settings.GetWispShapeInfo();
        if (gridState.AddPiece(wispShape))
        {
            hints.Enqueue(new BtHint(wispShape, gridState.hint));
            return wispShape;
        }
        
        // Should be imposible
        Debug.LogError("Nothing fits!");
        return new BtShapeInfo(shape, rotation);
    }

    void Clear()
    {
        foreach (var item in shapes)
        {
            if (item) Destroy(item.gameObject);
        }

        shapes.Clear();
    }

    void SetSlotShape(Transform slot, BtShapeInfo info)
    {
        var instance = Instantiate(DataManager.current.gameData.prefabShape, slot.position, slot.rotation, slot);
        instance.Init(info);
        shapes.Add(instance);
        instance.OnDropped += HandleShapeDropped;
    }

    public void GenerateNew(bool initial = true)
    {
        Clear();

        var tempGrid = BtGrid.current.MakeTempGrid();
        hints = new();
        OnHintsAviable?.Invoke(true);

        foreach (var slot in slots)
        {
            var info = GetNextShape(tempGrid);
            SetSlotShape(slot, info);
        }

        OnShapesGenerated?.Invoke(initial);
        BtSave.Create();
    }

    public void GeneratePool()
    {
        pool = DataManager.current.shapes
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
            BtUpgradeCtrl.current.Show((BtUpgradeRarity)upgradeLevel, 3);
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
        CombatArena.current.player.RemoveHp(2);
    }

    public void BtnGenerateNewShapes()
    {
        GenerateNew(false);
    }

    public void BtnLoadSave()
    {
        BtSave.Load();
    }

    public void BtnAutoplay()
    {
        StartCoroutine(AutoPlayTurn());
    }

    public void CheckDeadEnd()
    {
        foreach (var shape in shapes)
        {
            var tempGrid = BtGrid.current.MakeTempGrid();
            if (tempGrid.AddPiece(shape.GetInfo()))
            {
                return; // all good
            }
        }

        StartCoroutine(RoutineStuck());
    }

    IEnumerator RoutineStuck()
    {
        yield return new WaitForSeconds(3f);
        HelpPanel.current.ShowStuck();
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

    void HandleShapeDropped(BtShape shape, Vector2Int pos)
    {
        shapes.Remove(shape);
        shouldCheckSlots = true;
        if (hints != null && hints.Count > 0)
        {
            if(hints.Peek().Matches(shape.GetInfo(), pos))
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

    public List<BtShapeInfo> GetCurrentShapes()
    {
        var result = new List<BtShapeInfo>();
        //foreach (var slot in slots)
        foreach (var shape in shapes)
        {
            //var shape = slot.GetComponentInChildren<BtShape>();
            if (shape)
            {
                result.Add(shape.GetInfo());
            } else
            {
                result.Add(null);
            }
        }

        return result;
    }

    public List<BtHint> GetCurrentHints()
    {
        return new List<BtHint>(hints);
    }

    public void SetCurrentShapes(List<BtShapeInfo> infos, List<BtHint> hints)
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
        var list = new List<BtHint>(hints);
        foreach (var hint in list)
        {
            var slot = slots[idx++];
            var shape = slot.GetComponentInChildren<BtShape>();
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


public class BtShapeInfo
{
    public BtShapeData data;
    public int rotation;

    public BtShapeInfo(BtShapeData data, int rotation = 0)
    {
        this.data = data;
        this.rotation = rotation;
    }

    public List<BtBlockInfo> GetBlocks()
    {
        return data.GetBlocks(rotation);
    }
}
