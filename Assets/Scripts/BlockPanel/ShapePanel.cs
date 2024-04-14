using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

[DefaultExecutionOrder(+1)]
public class ShapePanel : MonoBehaviour
{
    public static System.Action<bool> OnShapesGenerated;

    [SerializeField] List<Transform> slots;
    HashSet<BtShape> shapes = new();

    [SerializeField] List<Color> colors;

    List<BtShapeData> pool;
    int poolIdx = 0;

    List<Vector2Int> hints;

    public BtShapeData GetShapeFromPool()
    {
        var shape = pool[poolIdx];
        if (++poolIdx >= pool.Count)
        {
            pool.Shuffle();
            poolIdx = 0;
        }

        return shape;
    }

    public BtShapeData GetNextShape(out int rotation, BtGrid.TempGridState gridState)
    {
        rotation = Random.Range(0, 4);

        BtShapeData shape = null;
        for (int i = 0; i < 20; i++)
        {
            shape = GetShapeFromPool();
            for (int j = 0; j < 4; j++)
            {
                int rot = (rotation + j) % 4;
                var blocks = shape.GetBlocks(rot)
                    .Select(b => b.pos)
                    .ToList();
                if (gridState.AddPiece(blocks))
                {
                    rotation = rot;
                    hints.Add(gridState.hint);
                    return shape;
                }
            }
        }

        Debug.Log("Nothing fits!");
        return shape;
    }

    public void Clear()
    {
        foreach (var item in shapes)
        {
            if (item) Destroy(item.gameObject);
        }

        shapes.Clear();
    }

    public void GenerateNew(bool initial)
    {
        Clear();

        var tempGrid = BtGrid.current.MakeTempGrid();
        hints = new List<Vector2Int>();

        foreach (var slot in slots)
        {
            var data = GetNextShape(out var rotation, tempGrid);
            var instance = Instantiate(DataManager.current.gameData.prefabShape, slot.position, slot.rotation, slot);
            instance.Init(data, colors.Rand(), rotation);
            shapes.Add(instance);
            instance.OnUsed += HandleShapeUsed;
        }

        OnShapesGenerated?.Invoke(initial);
    }

    private void Start()
    {
        pool = DataManager.current.shapes
            .OrderBy(x => System.Guid.NewGuid())
            .ToList();

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
    }

    public void BtnShowHint()
    {
        BtGrid.current.ShowHint(hints);
    }

    public void BtnGenerateNewShapes()
    {
        GenerateNew(false);
    }

    void HandleShapeUsed(BtShape shape)
    {
        shapes.Remove(shape);
        if (shapes.Count == 0)
        {
            GenerateNew(false);
        }
    }
}