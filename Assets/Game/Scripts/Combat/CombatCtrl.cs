using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using TileShapes;
using FancyToolkit;
using System.Linq;
using UnityEngine.SceneManagement;

public class CombatCtrl : MonoBehaviour, ILineClearHandler
{
    [SerializeField] TextAsset tableTiles;
    [SerializeField] TextAsset tableStartingTiles;
    [SerializeField] TextAsset tableUnits;
    [SerializeField] TextAsset tableStages;

    Board board;
    TileShapes.ShapePanel shapePanel;

    CombatArena arena;

    public bool EndTurnInProgress { get; private set; } = false;

    PoolQueue<TileData> tileQueue;

    public int tilesPerTurn = 5;

    public static void LoadScene(string id)
    {
        StageCtrl.current.SetStage(id);
        SceneManager.LoadScene("Combat");
    }

    private void Awake()
    {
        StageCtrl.current.AddData(tableStages);
        TileCtrl.current.AddData<MyTileData>(tableTiles);
        UnitCtrl.current.AddData<UnitData>(tableUnits);
        
        board = FindAnyObjectByType<Board>();
        board.Init();
     

        shapePanel = FindAnyObjectByType<TileShapes.ShapePanel>();

        arena = CombatArena.current;
        var startingTiles = FancyCSV.FromText<TileEntry>(tableStartingTiles.text)
            .SelectMany(x => Enumerable.Repeat(x.id, x.amount))
            .Select(id => TileCtrl.current.GetTile(id));

        tileQueue = new(startingTiles);
    }

    private void OnEnable()
    {
        LineClearer.AddHandler(this);
        //BtGrid.OnBoardChanged += HandleBoardChanged;
        shapePanel.OnOutOfShapes += NewTurn;
        Unit.OnKilled += HandleUnitKilled;
        UISelectTileCard.OnSelectTile += HandleTileAddToSet;
        UISelectTileCard.OnBuyTile += HandleTileAddToSet;
    }

    private void OnDisable()
    {
        LineClearer.RemoveHandler(this);
        //BtGrid.OnBoardChanged -= HandleBoardChanged;
        shapePanel.OnOutOfShapes -= NewTurn;
        Unit.OnKilled -= HandleUnitKilled;
        UISelectTileCard.OnSelectTile -= HandleTileAddToSet;
        UISelectTileCard.OnBuyTile -= HandleTileAddToSet;
    }

    void HandleTileAddToSet(UISelectTileCard card)
    {
        tileQueue.Add(card.data);
    }

    void HandleUnitKilled(Unit unit)
    {
        if (unit == arena.enemy || unit == arena.player)
        {
            StopAllCoroutines();
            StartCoroutine(CombatFinished());
        }
    }

    public Unit SpawnEnemy(string id)
    {
        var enemy = arena.SpawnEnemy(id);
        //var data = enemy.data;

        //BtGrid.current.LoadRandomBoard(data.boardLevel, data.specialBlockData);
        //ShapePanel.current.GenerateNew();

        //board.LoadRandomLayout(data.boardLevel, data.specialBlockData); // ToDo

        shapePanel.GenerateNew(true, tileQueue, tilesPerTurn);

        return enemy;
    }

    IEnumerator CombatFinished()
    {
        yield return new WaitForSeconds(2f);

        UIHudRewards.current.Show(StageCtrl.current.Data.rewards);

        /*
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

        yield return new WaitWhile(() => BtUpgradeCtrl.current.IsOpen);*/
    }

    public void NewTurn() => NewTurn(0.1f);

    public void NewTurn(float delay)
    {
        if (EndTurnInProgress) return; // ToDo: maybe soome warning

        EndTurnInProgress = true;
        if (!arena.enemy) return;   // Let's wait end of combat

        StartCoroutine(TurnRoutine(delay));
    }

    IEnumerator TurnRoutine(float delay)
    {
        shapePanel.IsLocked = true;
        yield return new WaitForSeconds(delay);

        //RunEndOfTurnPassives();
        board.UnlockAllTileActions();
        foreach (var item in board.GetNonEmptyTiles<MyTile>())
        {
            yield return item.EndOfTurn();
        }

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
        yield return new WaitForSeconds(0.2f);

        shapePanel.GenerateNew(false, tileQueue, tilesPerTurn);
        EndTurnInProgress = false;
        shapePanel.IsLocked = false;
    }

    IEnumerator Start()
    {
        yield return null;
        var stageData = StageCtrl.current.Data;
        board.LoadRandomLayout(stageData.specialTile);
        SpawnEnemy(stageData.units[0]);
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

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            UIHudRewards.current.Show(new List<string>{ "gold 1", "gold 20", "tile", "tile" });
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UIHudDialog.current.Show("test");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UIHudSelectTile.current.Show(SelectTileType.Shop, TileCtrl.current.GetAllTiles().RandN(5));
        }

#endif
    }

    IEnumerator ILineClearHandler.HandleLinesCleared(LineClearData clearData)
    {
        shapePanel.IsLocked = true;
        if (clearData.tiles.Count > 10)
        {
            arena.player.AnimAttack(2);
        }
        else if (clearData.tiles.Count > 0)
        {
            arena.player.AnimAttack(1);
        }

        board.UnlockAllTileActions();
        MyTile tile;
        while ((tile = clearData.PickNextTile() as MyTile) != null)
        {
            yield return tile.OnCleared(clearData);
        }
        shapePanel.IsLocked = false;
    }

    public class TileEntry
    {
        public string id;
        public int amount;
    }
}

public enum MatchStat
{
    None,
    MaxDamage,
    MaxArmor
}