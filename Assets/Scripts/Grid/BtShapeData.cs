using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BtShapeData
{
    List<Vector2Int> deltas;
    Vector2Int size;

    public BtShapeData(List<Vector2Int> blocks)
    {
        if (blocks.Count == 0) return;  // will be easier to find error

        var min = new Vector2Int(99, 99);
        var max = new Vector2Int(-1, -1);

        foreach (var d in blocks)
        {
            if (d.x > max.x) max.x = d.x;
            if (d.x < min.x) min.x = d.x;
            if (d.y > max.y) max.y = d.y;
            if (d.y < min.y) min.y = d.y;
        }

        size = max + Vector2Int.one - min ;

        deltas = blocks
            .Select(d => d - min)
            .ToList();
    }

    public static implicit operator bool(BtShapeData item)
        => item.deltas != null && item.deltas.Count > 0;

    public List<Vector2Int> GetBlocks(int rotation = 0)
    {
        var size = this.size;
        var deltas = this.deltas;

        switch (rotation)
        {
            case 1: // 90%
                {
                    size = new Vector2Int(size.y, size.x);
                    deltas = deltas
                            .Select(d => new Vector2Int(d.y, size.x - d.x - 1))
                            .ToList();
                    break;
                }
            case 2: // 180%
                {
                    deltas = deltas
                            .Select(d => new Vector2Int(size.x - d.x - 1, size.y - d.y - 1))
                            .ToList();
                    break;
                }
            case 3: // 270%
                {
                    size = new Vector2Int(size.y, size.x);
                    deltas = deltas
                            .Select(d => new Vector2Int(size.y - d.y, d.x))
                            .ToList();
                    break;
                }
            default:
                break;
        }

        // ToDo: figure out better way
        var min = new Vector2Int(99, 99);
        var max = new Vector2Int(-99, -99);

        foreach (var d in deltas)
        {
            if (d.x > max.x) max.x = d.x;
            if (d.x < min.x) min.x = d.x;
            if (d.y > max.y) max.y = d.y;
            if (d.y < min.y) min.y = d.y;
        }

        size = max + Vector2Int.one - min;

        deltas = deltas
            .Select(d => d - min)
            .ToList();

        // ToDo end

        return deltas
            .Select(d => d - (size / 2))
            .ToList();
    }

    public List<BtBlockPlacement> GetBlockPlacement(int rotation)
    {
        return GetBlocks(rotation)
            .Select(v => new BtBlockPlacement(new(), v.x, v.y))
            .ToList();
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