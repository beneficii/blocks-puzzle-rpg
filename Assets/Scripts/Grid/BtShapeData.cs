using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BtShapeData
{
    List<BtBlockInfo> blocks;
    Vector2Int size;
    public int level { get; private set; }

    public BtShapeData(List<BtBlockInfo> blocks, int level = 0)
    {
        if (blocks.Count == 0) return;  // will be easier to find error

        var min = new Vector2Int(99, 99);
        var max = new Vector2Int(-1, -1);

        foreach (var block in blocks)
        {
            var d = block.pos;
            if (d.x > max.x) max.x = d.x;
            if (d.x < min.x) min.x = d.x;
            if (d.y > max.y) max.y = d.y;
            if (d.y < min.y) min.y = d.y;
        }

        size = max + Vector2Int.one - min;

        this.blocks = blocks
            .Select(b => new BtBlockInfo(b.data, b.pos - min))
            .ToList();

        this.level = level;
    }

    public BtShapeData(List<Vector2Int> deltas, int level)
        : this(deltas.Select(d => new BtBlockInfo(DataManager.current.emptyBlock, d)).ToList(), level) { }
    
    public static implicit operator bool(BtShapeData item)
        => item.blocks != null && item.blocks.Count > 0;

    public List<BtBlockInfo> GetBlocks(int rotation = 0)
    {
        var size = (rotation % 2 == 0) ? this.size : new Vector2Int(this.size.y, this.size.x);
        var blocks = this.blocks
            .Select(b => b.Rotate(rotation, size))
            .ToList();


        // ToDo: figure out better way
        var min = new Vector2Int(99, 99);
        var max = new Vector2Int(-99, -99);

        foreach (var block in blocks)
        {
            var d = block.pos;
            if (d.x > max.x) max.x = d.x;
            if (d.x < min.x) min.x = d.x;
            if (d.y > max.y) max.y = d.y;
            if (d.y < min.y) min.y = d.y;
        }

        size = max + Vector2Int.one - min;
        // ToDo end

        var half = size / 2;
        return blocks
            .Select(b => new BtBlockInfo(b.data, b.pos - min - half))
            .ToList();
    }

    public BtBlockData SpecialBlock()
    {
        foreach (var item in blocks)
        {
            if (item.data.type != BtBlockType.None)
            {
                return item.data;
            }
        }

        return null;
    }

    public bool HasEmptyBlocks()
    {
        return blocks.Any(x => x.data.type == BtBlockType.None);
    }
}

[System.Serializable]
public class BtBlockInfo
{
    public BtBlockData data;
    public Vector2Int pos;

    public BtBlockInfo(BtBlockData data, Vector2Int pos)
    {
        this.data = data;
        this.pos = pos;
    }

    public BtBlockInfo Rotate(int rotation, Vector2Int size)
    {
        return rotation switch
        {
            1 => new(data, new(pos.y, size.x - pos.x - 1)),
            2 => new(data, new(size.x - pos.x - 1, size.y - pos.y - 1)),
            3 => new(data, new(size.y - pos.y, pos.x)),
            _ => new(data, pos),
        };
    }
}