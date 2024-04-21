using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class LineMatchListener : MonoBehaviour
{
    CombatArena arena;

    struct MyAction
    {
        public BtBlockData data;
        public Vector2 origin;

        public MyAction(BtBlockData data, Vector2 origin)
        {
            this.data = data;
            this.origin = origin;
        }
    }

    private void OnEnable()
    {
        BtGrid.OnLinesCleared += HandleLinesCleared;
        ShapePanel.OnShapesGenerated += HandleShapesGenerated;
        Unit.OnKilled += HandleUnitKilled;
    }

    private void OnDisable()
    {
        BtGrid.OnLinesCleared -= HandleLinesCleared;
        ShapePanel.OnShapesGenerated -= HandleShapesGenerated;
        Unit.OnKilled += HandleUnitKilled;
    }

    int upgradeOfferCtr = 0;
    void HandleUnitKilled(Unit unit)
    {
        if (unit == arena.enemy)
        {
            var enemy = arena.SpawnEnemy();
            upgradeOfferCtr++;
            var rarity = upgradeOfferCtr % 5 == 0 ? BtUpgradeRarity.Rare : BtUpgradeRarity.Common;
            BtUpgradeCtrl.Show(rarity, 3);
            BtGrid.current.LoadRandomBoard(enemy.data.level);
            ShapePanel.current.GenerateNew();
        }
    }

    void HandleLinesCleared(BtLineClearInfo lineClearInfo)
    {
        BtBlock block;
        while ((block = lineClearInfo.PickNextBlock()) != null)
        {
            block.data.HandleMatch(block, lineClearInfo);
        }
    }

    void HandleShapesGenerated(bool initial)
    {
        if (initial) return;

        
        arena.player.RemoveHp(arena.enemy.data.damage);
    }

    private void Awake()
    {
        arena = CombatArena.current;
    }
}