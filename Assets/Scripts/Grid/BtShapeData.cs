using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BtShapeData
{
    List<BtBlockInfo> blocks;
    Vector2Int size;

    public BtShapeData(List<Vector2Int> deltas)
    {
        if (deltas.Count == 0) return;  // will be easier to find error

        var min = new Vector2Int(99, 99);
        var max = new Vector2Int(-1, -1);

        foreach (var d in deltas)
        {
            if (d.x > max.x) max.x = d.x;
            if (d.x < min.x) min.x = d.x;
            if (d.y > max.y) max.y = d.y;
            if (d.y < min.y) min.y = d.y;
        }

        size = max + Vector2Int.one - min ;

        blocks = deltas
            .Select(d => new BtBlockInfo(RandomBlock(), d - min))
            .ToList();
    }

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

    BtBlockData RandomBlock()
    {
        int rand = Random.Range(0, 10);
        var type = BtBlockType.None;

        if (rand < 2) type = BtBlockType.Sword;
        else if (rand < 4) type = BtBlockType.Shield;
        else if (rand == 4) type = BtBlockType.Fire;

        return DataManager.current.blocks[type];
    }
}

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