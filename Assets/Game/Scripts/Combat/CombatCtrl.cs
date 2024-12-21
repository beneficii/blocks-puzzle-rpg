using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GridBoard;
using TileShapes;
using FancyToolkit;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

public class CombatCtrl : MonoBehaviour, ILineClearHandler
{
    static CombatCtrl _current;

    public static CombatCtrl current
    {
        get
        {
            if (_current == null)
            {
                _current = FindAnyObjectByType<CombatCtrl>();
            }
            return _current;
        }
    }

    [SerializeField] List<SpriteRenderer> bgRenders;
    [SerializeField] UIGenericButton btnEndTurn;

    System.Random endLevelRandom;

    Board board;
    ShapePanel shapePanel;

    CombatArena arena;

    public bool EndTurnInProgress { get; private set; } = false;
    Sequence animSequence;

    PoolQueue<TileData> tileQueue;

    Queue<CombatState> stateQueue = new();

    int tilesPerTurn;

    bool dontCheckQueue;

    int preventEndTurn;
    public int PreventEndTurn
    {
        get => preventEndTurn;
        set {
            Assert.IsTrue(value >= 0);
            preventEndTurn = value;

            btnEndTurn.SetInteractable(preventEndTurn == 0);
        }
    }

    List<BuffBase> buffs = new();

    public void AddBuff(BuffBase buff)
    {
        buff.SetBoard(board);
        buffs.Add(buff);
    }

    private void Awake()
    {
        tileQueue = new(Game.current.GetDeck());
        var settings = Game.current.GetCombatSettings();
        tilesPerTurn = settings.tilesPerTurn;
        HUDCtrl.current.OnAllClosed += HandleAllHudsClosed;
    }

    private void OnEnable()
    {
        LineClearer.AddHandler(this);
        Unit.OnKilled += HandleUnitKilled;
        UISelectTileCard.OnSelectCard += HandleAddCardToSet;
    }

    private void OnDisable()
    {
        LineClearer.RemoveHandler(this);
        Unit.OnKilled -= HandleUnitKilled;
        UISelectTileCard.OnSelectCard -= HandleAddCardToSet;
        animSequence?.Kill();
    }

    private void OnDestroy()
    {
        foreach (var item in buffs)
        {
            item.SetBoard(null);
        }
    }

    void HandleAllHudsClosed()
    {
        if (dontCheckQueue) return;
        CheckQueue();
    }

    public void CheckQueue()
    {
        if (stateQueue.Count == 0)
        {
            int? playerHp = null;
            var player = CombatArena.current.player;
            if (player) playerHp = player.health.Value;

            Game.current.FinishLevel(new()
            {
                tilesPerTurn = tilesPerTurn,
            }, playerHp);
            return;
        }

        var state = stateQueue.Dequeue();
        state.Run();
    }

    public void AddState(CombatState state)
    {
        stateQueue.Enqueue(state);
    }

    void HandleAddCardToSet(UISelectTileCard card, SelectTileType type)
    {
        var data = card.data;

        if (data is TileData tileData)
        {
            tileQueue.Add(tileData);
            Game.current.AddTileToDeck(tileData.id);
        }
        else if (data is SkillData skillData)
        {
            // ToDo: maybe init skill
            Game.current.AddSkill(skillData.id);
        }
        else
        {
            Debug.LogError("Unrecognized card type");
        }
    }

    void HandleUnitKilled(Unit unit)
    {
        if (unit == arena.player)
        {
            StopAllCoroutines();
            StartCoroutine(CombatFinished(false));
        }
        else if (unit == arena.enemy)
        {
            StopAllCoroutines();
            StartCoroutine(CombatFinished(true));
        }
    }

    public void ShowTileChoise(Rarity rarity)
    {
        var list = TileCtrl.current.GetAll()
                .Where(x => x.rarity == rarity)
                .OfType<MyTileData>()
                .ToList();

        UIHudSelectTile.current.ShowChoise(list.RandN(3, endLevelRandom));
    }

    public void ShowSkillChoise(Rarity rarity)
    {
        var list = SkillCtrl.current.GetAll()
                .Where(x => x.rarity == rarity)
                .ToList();
        UIHudSelectTile.current.ShowChoise(list.RandN(3, endLevelRandom));
    }

    List<UICombatReward.Data> GenerateCombatRewards()
    {
        var rng = endLevelRandom;
        var stageData = StageCtrl.current.Data;
        var rarity = stageData.reward;

        int gold = rarity switch
        {
            Rarity.Common => rng.Next(20, 40),
            Rarity.Uncommon => rng.Next(60, 100),
            Rarity.Rare => rng.Next(130, 180),
            Rarity.Legendary => rng.Next(150, 220),
            _ => rng.Next(45, 60),
        };

        var result = new List<UICombatReward.Data> {
            new UICombatReward.DataGold(gold),
        };

        if (rarity == Rarity.Legendary)
        {
            result.Add(new UICombatReward.DataTile(Rarity.Common));
            result.Add(new UICombatReward.DataTile(Rarity.Common));
            result.Add(new UICombatReward.DataTile(Rarity.Common));
            result.Add(new UICombatReward.DataTile(Rarity.Uncommon));
            result.Add(new UICombatReward.DataTile(Rarity.Uncommon));
        }
        else
        {
            result.Add(new UICombatReward.DataTile(rarity));
        }

        if (stageData.type == StageData.Type.Elite || stageData.type == StageData.Type.Boss)
        {
            result.Add(new UICombatReward.DataSkill(Rarity.Common));
            result.Add(new UICombatReward.DataTilesPerTurn());
        }

        return result;
    }

    void HandleDeadEnd()
    {
        btnEndTurn.SetNeedsAttention(true);
    }

    IEnumerator CombatFinished(bool victory)
    {
        yield return new WaitForSeconds(2f);

        if (victory)
        {
            dontCheckQueue = true;
            UIHudCombat.current.Close();
            UIHudRewards.current.Show(GenerateCombatRewards());
            dontCheckQueue = false;
        }
        else
        {
            UIHudGameOver.current.Show(false, StageCtrl.current.Data.gameOverText);
            Game.current.GameOver();
        }

    }

    public void NewTurn() => NewTurn(0.1f);

    public void NewTurn(float delay)
    {
        StartCoroutine(TurnRoutine(delay));
    }

    public IEnumerator TurnRoutine() => TurnRoutine(0.4f);
    public IEnumerator TurnRoutine(float delay)
    {
        if (PreventEndTurn > 0) yield break;
        btnEndTurn.SetNeedsAttention(false);
        if (EndTurnInProgress)
        {
            yield break; // ToDo: maybe soome warning
        }
        btnEndTurn.SetInteractable(false);
        EndTurnInProgress = true;   // ToDo: maybe lock button
        if (!arena.enemy) yield break;   // Let's wait end of combat

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
            if (!unit)
            {
                continue;
            }
            yield return unit.RoundActionPhase();
        }

        if (arena.player) yield return arena.player.EndOfTurn();

        yield return new WaitForSeconds(0.1f);
        foreach (var unit in arena.GetUnits())
        {
            unit.RoundFinished();
        }

        shapePanel.GenerateNew(false, tileQueue, tilesPerTurn);
        btnEndTurn.SetInteractable(true);
        EndTurnInProgress = false;
    }

    IEnumerator InitCombat()
    {
        var stageData = StageCtrl.current.Data;

        UIHudCombat.current.Show();
        board = FindAnyObjectByType<Board>();
        board.Init();

        shapePanel = FindAnyObjectByType<ShapePanel>();

        shapePanel.OnOutOfShapes.Add(TurnRoutine);
        shapePanel.OnDeadEnd += HandleDeadEnd;
        arena.player.board = board;
        arena.enemy.board = board;

        arena.enemy.SetCombatVisible(true);
        //arena.player.SetCombatVisible(true);

        yield return UIHudCombat.current.InitSkills(board);
        shapePanel.GenerateNew(true, tileQueue, tilesPerTurn);
        var clearer = FindAnyObjectByType<LineClearer>();
        if (clearer) WorldUpdateCtrl.current.AddSubscriber(clearer);
        WorldUpdateCtrl.current.AddSubscriber(shapePanel);

        var scenario = Factory<CombatScenario>.Create(stageData.scenario);
        if (scenario != null)
        {
            StartCoroutine(scenario.Start());
        }
        else
        {
            board.LoadRandomLayout(stageData.specialTile);
        }
    }

    public void InitShop()
    {
        var stageData = StageCtrl.current.Data;
        var rng = endLevelRandom;
        var list = new List<MyTileData>();

        void AddRarity(Rarity rarity, int count)
        {
            var items = TileCtrl.current.GetAllTiles()
                .Where(x => x.rarity == rarity)
                .OfType<MyTileData>()
                .ToList();
            list.AddRange(items.RandN(count, rng));
        }

        AddRarity(Rarity.Common, 3);
        AddRarity(Rarity.Uncommon, 2);
        list.Add(TileCtrl.current.Get<MyTileData>("health"));

        UIHudSelectTile.current.ShowShop(list, rng);
    }

    public void Init(StageData.Type type, string dialogId = null)
    {
        if (!string.IsNullOrEmpty(dialogId))
        {
            UIHudDialog.current.Show(dialogId);
            return;
        }

        switch (type)
        {
            case StageData.Type.Enemy:
            case StageData.Type.Elite:
            case StageData.Type.Boss:
                StartCoroutine(InitCombat());
                break;
            case StageData.Type.Shop:
                InitShop();
                break;
            case StageData.Type.Victory:
                Game.current.GameOver();
                UIHudGameOver.current.Show(true);
                break;
            default:
                Debug.LogError($"Unknown Combat type init ({type})");
                CheckQueue();
                break;
        }
    }


    IEnumerator Start()
    {
        if (Game.current.GetStateType() == Game.StateType.Map)
        {
            UIHudMap.current.Show();
            yield break;
        }

        var stageData = StageCtrl.current.Data;
        endLevelRandom = Game.current.CreateStageRng();
        if (Game.current.bgDict.TryGetValue(stageData.background, out var bgSprite))
        {
            foreach (var item in bgRenders) item.sprite = bgSprite;
        }

        arena = CombatArena.current;
        var enemy = arena.SpawnEnemy(stageData.units[0]);
        var player = arena.SpawnPlayer();
        enemy.SetCombatVisible(false);
        var hp = Game.current.GetPlayerHealth();
        player.SetHp(hp.x);

        yield return null;
        Init(stageData.type, stageData.dialog);
    }


    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Z))
        {
            arena.player.AnimAttack(1);
            arena.enemy.RemoveHp(20);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            arena.player.AddArmor(5);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            arena.player.RemoveHp(5);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            arena.player.AddHp(5);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            TileCtrl.current.DebugAll();
            SkillCtrl.current.DebugAll();
        }

#endif
    }


#if UNITY_EDITOR
    [SerializeField] string debugId;
    [EasyButtons.Button("DebugId")]
    public void DebugId()
    {
        if (string.IsNullOrEmpty(debugId)) return;

        Debug.Log($"{board.GetIdTileCount(debugId)}");
    }
#endif

    IEnumerator ILineClearHandler.HandleLinesCleared(LineClearData clearData)
    {
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
    }
}

public class CombatSettings
{
    public int tilesPerTurn;
}

public enum MatchStat
{
    None,
    MaxDamage,
    MaxArmor
}