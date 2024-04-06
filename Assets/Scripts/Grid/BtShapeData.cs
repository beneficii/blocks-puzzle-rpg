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

    public BtShapeData(ShapeInfo info)
    {
        this.deltas = info.deltas;
    }

    public List<BtBlockPlacement> GetBlocks()
    {
        return deltas.Select(v => new BtBlockPlacement(new(), v.x, v.y)).ToList();
    }
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


public class ShapeInfo
{
    public List<Vector2Int> deltas;
}