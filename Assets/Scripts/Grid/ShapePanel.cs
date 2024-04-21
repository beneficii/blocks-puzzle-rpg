using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
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

    [SerializeField] List<Transform> slots;
    HashSet<BtShape> shapes = new();

    List<BtShapeData> pool;
    int poolIdx = 0;

    List<Vector2Int> hints;

    bool shouldCheckSlots = false;

    public BtShapeData GetShapeFromPool()
    {
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
                    hints.Add(gridState.hint);
                    return info;
                }
            }
        }


        // give a block that fits everywhere
        var wispShape = BtGrid.current.settings.GetWispShapeInfo();
        if (gridState.AddPiece(wispShape))
        {
            hints.Add(gridState.hint);
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
        instance.OnUsed += HandleShapeUsed;
    }

    public void GenerateNew(bool initial = true)
    {
        Clear();

        var tempGrid = BtGrid.current.MakeTempGrid();
        hints = new List<Vector2Int>();

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

    private void Start()
    {
        GeneratePool();
        GenerateNew(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateNew(true);
        }

        if (hints != null && Input.GetKeyDown(KeyCode.H))
        {
            BtGrid.current.ShowHint(hints);
        }

        int upgradeLevel = -1;
        if (Input.GetKeyDown(KeyCode.Alpha0)) upgradeLevel = 0;
        if (Input.GetKeyDown(KeyCode.Alpha1)) upgradeLevel = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2)) upgradeLevel = 2;
        if (Input.GetKeyDown(KeyCode.Alpha3)) upgradeLevel = 3;

        if (upgradeLevel >= 0)
        {
            BtUpgradeCtrl.Show((BtUpgradeRarity)upgradeLevel, 3);
        }
    }

    public void BtnShowHint()
    {
        BtGrid.current.ShowHint(hints);
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
        StartCoroutine(AutoPlay());
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

        Game.ToDo("Dead end! Offer rewind");
    }

    public void CheckSlots()
    {
        if (!shouldCheckSlots) return;
        shouldCheckSlots = false;

        if (shapes.Count == 0)
        {
            GenerateNew(false);
            return;
        }

        CheckDeadEnd();
    }

    void HandleShapeUsed(BtShape shape)
    {
        shapes.Remove(shape);
        shouldCheckSlots = true;
    }

    public List<BtShapeInfo> GetCurrentShapes()
    {
        var result = new List<BtShapeInfo>();
        foreach (var slot in slots)
        {
            var shape = slot.GetComponentInChildren<BtShape>();
            if (shape)
            {
                result.Add(new BtShapeInfo(shape.data, shape.rotation));
            } else
            {
                result.Add(null);
            }
        }

        return result;
    }

    public void SetCurrentShapes(List<BtShapeInfo> infos)
    {
        Clear();
        int idx = 0;
        foreach (var slot in slots)
        {
            var info = infos[idx++];
            if (info == null) continue;

            SetSlotShape(slot, info);
        }
    }

    IEnumerator AutoPlayTurn()
    {
        if (hints == null || hints.Count == 0) yield break;

        int idx = 0;
        foreach (var hint in hints)
        {
            var slot = slots[idx++];
            var shape = slot.GetComponentInChildren<BtShape>();
            if (!shape) yield break;

            var dropped = shape.DropAt(hint);
            if (!dropped)
            {
                Debug.LogError("Could not drop");
                yield break;
            }

            yield return new WaitForSeconds(0.05f);
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
