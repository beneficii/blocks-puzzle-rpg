using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class ShapePanel : MonoBehaviour
{
    [SerializeField] List<Transform> slots;
    HashSet<BtShape> shapes = new();

    [SerializeField] List<Color> colors;

    public BtShapeData GetNextShape()
    {
        var rand = Random.Range(0, 3);

        return BtShapeData.TestB;
        switch (rand)
        {
            case 1: return BtShapeData.TestVertical;
            case 2: return BtShapeData.TestT;

            default: return BtShapeData.TestBasic;
        }
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