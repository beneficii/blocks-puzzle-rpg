using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public static class BtUpgradeCtrl
{
    public static Dictionary<BtShapeData, BtShapeData> upgrades;

    static List<BtShapeData> GetFreeShapes(BtUpgradeRarity rarity, int count)
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

    public static void Show(BtUpgradeRarity rarity, int count)
    {
        upgrades = new Dictionary<BtShapeData, BtShapeData>();

        var shapes = GetFreeShapes(rarity, count);

        if (shapes.Count == 0)
        {
            Debug.Log("Out of shapes to upgrade!");
            return;
        }

        var upgradeBlocks = DataManager.current.gameData.blocks
            .Where(x => x.Rarity == rarity)
            .RandN(count);

        if (upgradeBlocks.Count < shapes.Count)
        {
            Debug.Log("Out of blocks to upgrade!");
            return;
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
            emptyBlock.data = upgradeBlocks[i];

            var upgradedShape = new BtShapeData(oldBlocks, (int)rarity);
            upgrades.Add(upgradedShape, shape);

            cards[i].Init(upgradedShape, emptyBlock);
        }
    }

    public static void Select(BtShapeData data)
    {
        if (upgrades == null || !upgrades.TryGetValue(data, out var old))
        {
            Debug.LogError("Upgrade was not initialized");
            return;
        }

        var shapes = DataManager.current.shapes;
        var idxOld = shapes.IndexOf(old);
        shapes[idxOld] = data;
        ShapePanel.current.GeneratePool();
    }
}


public enum BtUpgradeRarity
{
    Basic,
    Common,
    Uncommon,
    Rare,
}