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
        ShapePanel.OnOutOfShapes += NewTurn;
        Unit.OnKilled += HandleUnitKilled;
    }

    private void OnDisable()
    {
        BtGrid.OnLinesCleared -= HandleLinesCleared;
        ShapePanel.OnOutOfShapes -= NewTurn;
        Unit.OnKilled += HandleUnitKilled;
    }

    int upgradeOfferCtr = 0;
    void HandleUnitKilled(Unit unit)
    {
        if (unit == arena.enemy)
        {
            StartCoroutine(CombatFinished());
        }
    }

    IEnumerator CombatFinished()
    {
        yield return new WaitForSeconds(2f);
        upgradeOfferCtr++;
        var rarity = upgradeOfferCtr % 5 == 0 ? BtUpgradeRarity.Rare : BtUpgradeRarity.Common;
        BtUpgradeCtrl.Show(rarity, 3);

        var enemy = arena.SpawnEnemy();
        BtGrid.current.LoadRandomBoard(enemy.data.level);
        ShapePanel.current.GenerateNew();
    }

    void HandleLinesCleared(BtLineClearInfo lineClearInfo)
    {
        BtBlock block;
        while ((block = lineClearInfo.PickNextBlock()) != null)
        {
            block.data.HandleMatch(block, lineClearInfo);
        }
    }

    public void NewTurn()
    {
        StartCoroutine(TurnRoutine());
    }

    IEnumerator TurnRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        if (arena.enemy)
        {
            arena.player.RemoveHp(arena.enemy.data.damage);
        }
        yield return new WaitForSeconds(0.1f);
        foreach (var unit in arena.GetUnits())
        {
            unit.RoundFinished();
        }
        yield return new WaitForSeconds(0.1f);
        ShapePanel.current.GenerateNew(false);
    }

    private void Awake()
    {
        arena = CombatArena.current;
    }
}