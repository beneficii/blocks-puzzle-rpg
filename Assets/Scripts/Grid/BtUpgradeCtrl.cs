using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class BtUpgradeCtrl : MonoBehaviour
{
    static BtUpgradeCtrl _current;
    public static BtUpgradeCtrl current
    {
        get
        {
            if (!_current)
            {
                _current = FindFirstObjectByType<BtUpgradeCtrl>();
            }

            return _current;
        }
    }

    public Dictionary<BtShapeData, BtShapeData> upgrades;

    Queue<QItem> queue = new();

    List<BtShapeData> GetFreeShapes(BtUpgradeRarity rarity, int count)
    {
        int level = (int)rarity;
        var querry = DataManager.current.shapes
            .Where(x => x.HasEmptyBlocks());

        if (rarity == BtUpgradeRarity.Rare)
        {
            querry = querry.Where(x => x.Rarity >= BtUpgradeRarity.Uncommon)
                .Where(x => !x.HasUniqueBlock());
        }

        return querry.RandN(count);
    }

    void Close()
    {
        upgrades = null;

        if (queue.Count > 0)
        {
            var next = queue.Dequeue();
            Show(next.rarity, next.count);
        }
    }

    public void Show(BtUpgradeRarity rarity, int count)
    {
        if (upgrades != null)
        {
            queue.Enqueue(new QItem(rarity, count));
            return;
        }

        upgrades = new Dictionary<BtShapeData, BtShapeData>();

        var shapes = GetFreeShapes(rarity, count);

        if (shapes.Count == 0)
        {
            Debug.Log("Out of shapes to upgrade!");
            Close();
            return;
        }

        var upgradeBlocks = DataManager.current.gameData.blocks
            .Where(x => x.rarity == rarity)
            .RandN(count);

        if (upgradeBlocks.Count < shapes.Count)
        {
            if (upgradeBlocks.Count == 0)
            {
                Debug.Log("Out of blocks to upgrade!");
                Close();
                return;
            }
        }

        var cards = UIUpgradeScreen.Instance.Open<UIBtShapeUpgradeCard>(shapes.Count);

        for (int i = 0; i < cards.Count; i++)
        {
            var shape = shapes[i];
            var oldBlocks = shape.GetBlocks();
            var emptyBlock = oldBlocks
                .Where(x => x.data.type == BtBlockType.None)
                .Rand();

            // upgrade random empty block
            emptyBlock.data = upgradeBlocks[i%upgradeBlocks.Count];

            var upgradedShape = new BtShapeData(oldBlocks, (int)rarity);
            upgrades.Add(upgradedShape, shape);

            cards[i].Init(upgradedShape, emptyBlock);
        }
    }

    public void Select(BtShapeData data)
    {
        if (upgrades == null || !upgrades.TryGetValue(data, out var old))
        {
            Debug.LogError("Upgrade was not initialized");
            Close();
            return;
        }

        var shapes = DataManager.current.shapes;
        var idxOld = shapes.IndexOf(old);
        shapes[idxOld] = data;
        ShapePanel.current.GeneratePool();

        Close();
    }

    public class QItem
    {
        public BtUpgradeRarity rarity;
        public int count;

        public QItem(BtUpgradeRarity rarity, int count)
        {
            this.rarity = rarity;
            this.count = count;
        }
    }
}


public enum BtUpgradeRarity
{
    Basic,
    Common,
    Uncommon,
    Rare,
    Legenday,
}