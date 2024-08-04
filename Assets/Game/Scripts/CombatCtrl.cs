using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using TileShapes;
using FancyToolkit;

public class CombatCtrl : MonoBehaviour
{
    [SerializeField] TextAsset tableTiles;

    Board board;
    TileShapes.ShapePanel shapePanel;

    CombatArena arena;

    private void Awake()
    {
        TileCtrl.current.AddData<MyTileData>(tableTiles);
        
        board = FindAnyObjectByType<Board>();
        board.Init();
     

        shapePanel = FindAnyObjectByType<TileShapes.ShapePanel>();

        arena = CombatArena.current;
    }

    private void OnEnable()
    {
        LineClearer.OnCleared += HandleLinesCleared;
        //BtGrid.OnBoardChanged += HandleBoardChanged;
        shapePanel.OnOutOfShapes += NewTurn;
        Unit.OnKilled += HandleUnitKilled;
    }

    private void OnDisable()
    {
        LineClearer.OnCleared -= HandleLinesCleared;
        //BtGrid.OnBoardChanged -= HandleBoardChanged;
        shapePanel.OnOutOfShapes -= NewTurn;
        Unit.OnKilled -= HandleUnitKilled;
    }

    void HandleUnitKilled(Unit unit)
    {
        if (unit == arena.enemy || unit == arena.player)
        {
            StopAllCoroutines();
            StartCoroutine(CombatFinished(unit.reward));
        }
    }

    public Unit SpawnEnemy(UnitData data)
    {
        var enemy = arena.SpawnEnemy(data);

        //BtGrid.current.LoadRandomBoard(data.boardLevel, data.specialBlockData);
        //ShapePanel.current.GenerateNew();

        //board.LoadRandomLayout(data.boardLevel, data.specialBlockData); // ToDo

        shapePanel.GenerateNew();


        return enemy;
    }

    IEnumerator CombatFinished(BtUpgradeRarity reward)
    {
        yield return new WaitForSeconds(2f);
        HelpPanel.current.Close();  // just in case

        if (!arena.player)
        {
            MenuCtrl.Load(GameOverType.Defeat);
            yield break;
        }

        BtUpgradeCtrl.current.Show(reward, 3);

        var next = MapCtrl.current.Next();
        if (next == null)
        {
            MenuCtrl.Load(GameOverType.Victory);
            GameSave.Clear();
            yield break;
        }
        SpawnEnemy(next.unitData);
        arena.player.CombatFinished();

        yield return new WaitWhile(() => BtUpgradeCtrl.current.IsOpen);
        GameSave.Save();
    }


    void HandleLinesCleared(LineClearData lineClearInfo)
    {
        if (lineClearInfo.tiles.Count > 10)
        {
            arena.player.AnimAttack(2);
        }
        else if (lineClearInfo.tiles.Count > 0)
        {
            arena.player.AnimAttack(1);
        }

        ResCtrl<MatchStat>.current.Clear();
        MyTile tile;
        while ((tile = lineClearInfo.PickNextTile() as MyTile) != null)
        {
            tile.myData.clearAction?.Build().Run(tile, lineClearInfo);
        }
    }

    public void NewTurn() => NewTurn(0.6f);

    public void NewTurn(float delay)
    {
        if (!arena.enemy) return;   // Let's wait end of combat

        StartCoroutine(TurnRoutine(delay));
    }

    IEnumerator TurnRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        //RunEndOfTurnPassives();
        if (!arena.player || !arena.enemy) yield break;
        yield return new WaitForSeconds(delay / 2f);

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
        shapePanel.GenerateNew(false);
    }
    /*
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
    }*/

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
            arena.enemy.RemoveHp(50);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            NewTurn();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            arena.player.AddArmor(5);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            arena.player.RemoveHp(5);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            arena.player.AddHp(5);
        }

        #endif
    }
}
