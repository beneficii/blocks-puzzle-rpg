using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using TileShapes;
using FancyToolkit;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEditor.U2D.Aseprite;

public class CombatCtrl : MonoBehaviour, ILineClearHandler
{
    [SerializeField] List<SpriteRenderer> bgRenders;

    Board board;
    ShapePanel shapePanel;

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
        tileQueue = new(Game.current.GetDeck());
    }

    private void OnEnable()
    {
        LineClearer.AddHandler(this);
        //BtGrid.OnBoardChanged += HandleBoardChanged;
        Unit.OnKilled += HandleUnitKilled;
        UISelectTileCard.OnSelectTile += HandleTileAddToSet;
        UISelectTileCard.OnBuyTile += HandleTileAddToSet;
    }

    private void OnDisable()
    {
        LineClearer.RemoveHandler(this);
        //BtGrid.OnBoardChanged -= HandleBoardChanged;
        Unit.OnKilled -= HandleUnitKilled;
        UISelectTileCard.OnSelectTile -= HandleTileAddToSet;
        UISelectTileCard.OnBuyTile -= HandleTileAddToSet;
    }

    void HandleTileAddToSet(UISelectTileCard card)
    {
        tileQueue.Add(card.data);
        Game.current.AddTileToDeck(card.data.id);
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


        return enemy;
    }

    IEnumerator CombatFinished()
    {
        yield return new WaitForSeconds(2f);

        UIHudRewards.current.Show(StageCtrl.current.Data.rewards);
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


        foreach (var unit in arena.GetEnemies())
        {
            yield return unit.EndOfTurn();
            yield return unit.RoundActionPhase();
        }

        if (arena.player) yield return arena.player.EndOfTurn();


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

    IEnumerator InitCombat()
    {
        var stageData = StageCtrl.current.Data;

        UIHudCombat.current.Show();
        board = FindAnyObjectByType<Board>();
        board.Init();

        shapePanel = FindAnyObjectByType<ShapePanel>();

        board.LoadRandomLayout(stageData.specialTile);
        shapePanel.OnOutOfShapes += NewTurn;
        yield return UIHudCombat.current.InitSkills(board);
        shapePanel.GenerateNew(true, tileQueue, tilesPerTurn);
    }

    public void InitShop()
    {
        var stageData = StageCtrl.current.Data;
        var rng = Game.current.CreateStageRng();
        var list = new List<MyTileData>();

        void AddRarity(Rarity rarity, int count)
        {
            var items = TileCtrl.current.GetAllTiles()
                .Cast<MyTileData>()
                .Where(x => x.rarity == rarity)
                .ToList();
            list.AddRange(items.RandN(count, rng));
        }

        AddRarity(Rarity.Common, 3);
        AddRarity(Rarity.Uncommon, 2);
        list.Add(TileCtrl.current.GetTile<MyTileData>("health"));

        UIHudSelectTile.current.Show(SelectTileType.Shop, list);
    }

    IEnumerator Start()
    {
        var stageData = StageCtrl.current.Data;
        if (Game.current.bgDict.TryGetValue(stageData.background, out var bgSprite))
        {
            foreach (var item in bgRenders) item.sprite = bgSprite;
        }

        arena = CombatArena.current;
        arena.SpawnEnemy(stageData.units[0]);
        var player = arena.SpawnPlayer();
        var hp = Game.current.GetPlayerHealth();
        player.SetHp(hp.x);

        yield return null;
        switch (stageData.type)
        {
            case StageData.Type.Enemy:
            case StageData.Type.Elite:
            case StageData.Type.Boss:
                StartCoroutine(InitCombat());
                break;
            case StageData.Type.Shop:
                InitShop();
                break;
            case StageData.Type.Dialog:
                break;
            default:
                Debug.LogError("Unknown type");
                break;
        }

        
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
            UIHudRewards.current.Show(new List<string>{ "gold 1", "gold 20"});
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UIHudDialog.current.Show("test");
        }

#endif
    }


#if UNITY_EDITOR
    [SerializeField] string debugId;
    [EasyButtons.Button("DebugId")]
    public void DebugId()
    {
        if (string.IsNullOrEmpty(debugId)) return;

        Debug.Log($"{board.dictTileCounter.Get(debugId)}");
    }
#endif

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
}

public enum MatchStat
{
    None,
    MaxDamage,
    MaxArmor
}