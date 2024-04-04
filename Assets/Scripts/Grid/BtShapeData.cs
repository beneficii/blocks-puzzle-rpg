using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BtShapeData
{
    List<Vector2Int> deltas;

    public BtShapeData(List<Vector2Int> deltas)
    {
        this.deltas = deltas;
    }

    public List<BtBlockPlacement> GetBlocks()
    {
        return deltas.Select(v => new BtBlockPlacement(new(), v.x, v.y)).ToList();
    }

    public static BtShapeData TestBasic => new BtShapeData(new()
    {
        new(0,0),
    });

    public static BtShapeData TestVertical => new BtShapeData(new()
    {
        new(0,-1), new(0,0), new(0,1),
    });

    public static BtShapeData TestT => new BtShapeData(new()
    {
        new(-1,0), new(0,0), new(1,0), new(0,1),
    });

    public static BtShapeData TestB => new BtShapeData(new()
    {
        new(1,1), new(0,0), new(1,0), new(0,1),
    });
}

public struct BtBlockPlacement
{
    public BtBlockData data;
    public int x, y;

    public BtBlockPlacement(BtBlockData data, int x, int y)
    {
        this.data = data;
        this.x = x;
        this.y = y;
    }
}