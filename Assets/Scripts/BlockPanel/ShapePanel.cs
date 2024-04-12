using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class ShapePanel : MonoBehaviour
{
    public static System.Action<bool> OnShapesGenerated;

    [SerializeField] List<Transform> slots;
    HashSet<BtShape> shapes = new();

    [SerializeField] List<Color> colors;

    List<BtShapeData> pool;
    //int poolIdx = 0;
    int generates = 55;

    public BtShapeData GetNextShape()
    {
        int level = Random.Range(0, ++generates/20);
        return pool
            .Where(e => e.level <= level)
            .Rand();

        //var shape = pool[poolIdx];
        //poolIdx = (poolIdx + 1) % pool.Count;
        //return pool.Rand();
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

        int idx = 0;
        foreach (var slot in slots)
        {
            var data = GetNextShape();
            var instance = Instantiate(DataManager.current.gameData.prefabShape, slot.position, slot.rotation, slot);
            instance.Init(data, colors.Rand(), idx++);
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