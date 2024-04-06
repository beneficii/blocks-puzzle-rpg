using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class ShapePanel : MonoBehaviour
{
    [SerializeField] List<Transform> slots;
    HashSet<BtShape> shapes = new();

    [SerializeField] List<Color> colors;

    List<BtShapeData> pool;
    int poolIdx = 0;

    public BtShapeData GetNextShape()
    {
        var shape = pool[poolIdx];
        poolIdx = (poolIdx + 1) % pool.Count;
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

    public void GenerateNew()
    {
        Clear();

        foreach (var slot in slots)
        {
            var data = GetNextShape();
            var instance = Instantiate(DataManager.current.gameData.prefabShape, slot.position, slot.rotation, slot);
            instance.Init(data, colors.Rand());
            shapes.Add(instance);
            instance.OnUsed += HandleShapeUsed;
        }
    }

    private void Start()
    {
        pool = DataManager.current.shapes
            .Select(x => new BtShapeData(x))
            .OrderBy(x => System.Guid.NewGuid())
            .ToList();

        GenerateNew();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateNew();
        }
    }

    void HandleShapeUsed(BtShape shape)
    {
        shapes.Remove(shape);
        if (shapes.Count == 0)
        {
            GenerateNew();
        }
    }
}