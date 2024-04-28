using System.Linq;
using System.Collections.Generic;
using FancyToolkit;

public class BtLineClearInfo
{
    public HashSet<BtBlock> blocks;
    Queue<BtBlock> queue;
    public Queue<BtBlock> boardEmptyBlocks;    // empty blocks left on board (for spawning)

    public int rowsMatched;
    public int columnsMatched;

    public BtLineClearInfo(HashSet<BtBlock> blocks, int rowsMatched, int columnsMatched, List<BtBlock> emptyBlocks)
    {
        this.blocks = blocks;
        queue = new Queue<BtBlock>(blocks.OrderByDescending(x => x.data.priority));
        this.rowsMatched = rowsMatched;
        this.columnsMatched = columnsMatched;
        boardEmptyBlocks = new Queue<BtBlock>(emptyBlocks.Shuffled());
    }

    public BtBlock PickNextBlock()
    {
        while (queue.Count > 0)
        {
            var block = queue.Dequeue();
            if (blocks.Contains(block)) return block;
        }

        return null;
    }
} 