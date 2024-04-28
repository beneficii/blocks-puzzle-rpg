using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

[DefaultExecutionOrder(+5)]
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
        BtGrid.OnBoardChanged += HandleBoardChanged;
        ShapePanel.OnOutOfShapes += NewTurn;
        Unit.OnKilled += HandleUnitKilled;
    }

    private void OnDisable()
    {
        BtGrid.OnLinesCleared -= HandleLinesCleared;
        BtGrid.OnBoardChanged -= HandleBoardChanged;
        ShapePanel.OnOutOfShapes -= NewTurn;
        Unit.OnKilled -= HandleUnitKilled;
    }

    void HandleUnitKilled(Unit unit)
    {
        if (unit == arena.enemy)
        {
            StartCoroutine(CombatFinished(unit.reward));
        }
    }

    public Unit SpawnEnemy(UnitData data)
    {
        var enemy = arena.SpawnEnemy(data);

        BtGrid.current.LoadRandomBoard(data.boardLevel, data.specialBlockData);
        ShapePanel.current.GenerateNew();

        return enemy;
    }

    IEnumerator CombatFinished(BtUpgradeRarity reward)
    {
        yield return new WaitForSeconds(2f);
        BtUpgradeCtrl.Show(reward, 3);

        var next = MapCtrl.current.Next();
        if (next == null)
        {
            Game.ToDo("All enemies defeated!");
            yield break;
        }
        SpawnEnemy(next.unitData);
        arena.player.CombatFinished();
    }


    void HandleLinesCleared(BtLineClearInfo lineClearInfo)
    {
        if (lineClearInfo.blocks.Count > 10)
        {
            arena.player.AnimAttack(2);
        }
        else if (lineClearInfo.blocks.Count > 0)
        {
            arena.player.AnimAttack(1);
        }

        ResCtrl<MatchStat>.current.Clear();
        BtBlock block;
        while ((block = lineClearInfo.PickNextBlock()) != null)
        {
            block.data.HandleMatch(block, lineClearInfo);
        }
    }

    public void NewTurn()
    {
        if (!arena.enemy) return;   // Let's wait end of combat

        StartCoroutine(TurnRoutine());
    }

    void RunEndOfTurnPassives()
    {
        var player = arena.player;
        if (!player) return;

        var enemy = arena.enemy;
        if (!enemy) return;

        var res = ResCtrl<CombatModifier>.current;

        var poison = res.Get(CombatModifier.Poison);
        if (poison > 0) player.RemoveHp(poison);

        var armor = res.Get(CombatModifier.PassiveArmor);
        if (armor > 0) player.AddArmor(armor);

        var enemyArmor = res.Get(CombatModifier.EnemyPassiveArmor);
        if (enemyArmor > 0) enemy.AddArmor(enemyArmor);
    }

    IEnumerator TurnRoutine()
    {
        yield return new WaitForSeconds(0.6f);

        RunEndOfTurnPassives();
        if (!arena.player || !arena.enemy) yield break;
        yield return new WaitForSeconds(0.3f);


        foreach (var unit in arena.GetUnits())
        {
            yield return unit.RoundActionPhase();
        }

        yield return new WaitForSeconds(0.5f);
        foreach (var unit in arena.GetUnits())
        {
            unit.RoundFinished();
        }
        yield return new WaitForSeconds(0.1f);
        ShapePanel.current.GenerateNew(false);
    }

    void HandleBoardChanged()
    {
        CalculateModifiers();
    }

    void CalculateModifiers()
    {
        ResCtrl<CombatModifier>.current.Clear();

        foreach (var block in BtGrid.current.GetSpecialBlocks())
        {
            if (block.data is not CombatBlockData data) continue;

            data.CalculatePassives(block);
        }
    }

    private void Awake()
    {
        arena = CombatArena.current;
    }

    IEnumerator Start()
    {
        yield return null;
        var next = MapCtrl.current.Next();
        SpawnEnemy(next.unitData);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Z))
        {
            arena.player.AnimAttack(1);
            arena.enemy.RemoveHp(3);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(arena.enemy.RoundActionPhase());
        }
#endif
    }
}


public enum MatchStat
{
    None,
    MaxDamage,
    MaxArmor
}