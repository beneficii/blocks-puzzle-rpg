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

    public static event System.Action OnTurnFinished;

    [SerializeField] List<SpriteRenderer> bgRenders;
    [SerializeField] UIGenericButton btnEndTurn;
    [SerializeField] UIHudCombat hud;

    System.Random endLevelRandom;

    Board board;
    ShapePanel shapePanel;

    CombatArena arena;

    public bool EndTurnInProgress { get; private set; } = false;
    Sequence animSequence;

    PoolQueue<TileData> tileQueue;

    Queue<CombatState> stateQueue = new();

    public int tilesPerTurn;

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
        HUDCtrl.current.OnAllClosed += HandleAllHudsClosed;
    }

    private void OnEnable()
    {
        LineClearer.AddHandler(this);
        Unit.OnKilled += HandleUnitKilled;
        UISelectTileCard.OnSelectCard += HandleCardSelected;
    }

    private void OnDisable()
    {
        LineClearer.RemoveHandler(this);
        Unit.OnKilled -= HandleUnitKilled;
        UISelectTileCard.OnSelectCard -= HandleCardSelected;
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
        //if (dontCheckQueue) return;

        if (hud.IsOpen) return;
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

    public void AddTileToSet(TileData tileData)
    {
        tileQueue.Add(tileData);
        Game.current.AddTileToDeck(tileData.id);
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
            result.Add(new UICombatReward.DataTile(Rarity.Uncommon));
            result.Add(new UICombatReward.DataTile(Rarity.Uncommon));
        }
        else
        {
            result.Add(new UICombatReward.DataTile(rarity));
        }

        if (stageData.type == StageType.Elite || rarity == Rarity.Legendary)
        {
            var randomGlyph = GlyphCtrl.current.GetAll()
                .Where(x => x.rarity == Rarity.Common)
                .ToList()
                .Rand(rng);

            if (randomGlyph != null)
            {
                result.Add(new UICombatReward.DataGlyph(randomGlyph));
            }
        }

        return result;
    }

    void HandleDeadEnd()
    {
        btnEndTurn.SetNeedsAttention(true);
    }

    public void ShowCombatDialog(string dialogId)
    {
        hud.Close();
        UIHudDialog.current.Show(dialogId);
    }

    IEnumerator CombatFinished(bool victory)
    {
        yield return new WaitForSeconds(2f);

        if (victory)
        {
            hud.Close();
            UIHudRewards.current.Show(GenerateCombatRewards());
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
        OnTurnFinished?.Invoke();
    }

    IEnumerator InitCombat()
    {
        var stageData = StageCtrl.current.Data;

        tileQueue = new(Game.current.GetDeck());
        var settings = Game.current.GetCombatSettings();
        tilesPerTurn = settings.tilesPerTurn;

        hud.Show();
        board = FindAnyObjectByType<Board>();
        board.Init();

        shapePanel = FindAnyObjectByType<ShapePanel>();

        shapePanel.OnOutOfShapes.Add(TurnRoutine);
        shapePanel.OnDeadEnd += HandleDeadEnd;
        arena.player.board = board;
        arena.enemy.board = board;

        arena.enemy.SetCombatVisible(true);
        //arena.player.SetCombatVisible(true);

        var scenario = Factory<CombatScenario>.Create(stageData.scenario);
        if (scenario != null)
        {
            StartCoroutine(scenario.Start());
        }
        else
        {
            board.LoadRandomLayout(stageData.specialTile);
        }

        yield return null;
        yield return hud.InitSkills(board);
        yield return null;
        yield return hud.InitGlyphs(board);

        if (PreventEndTurn == 0)
        {
            shapePanel.GenerateNew(true, tileQueue, tilesPerTurn);
        }
        var clearer = FindAnyObjectByType<LineClearer>();
        if (clearer) WorldUpdateCtrl.current.AddSubscriber(clearer);
        WorldUpdateCtrl.current.AddSubscriber(shapePanel);
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
        //list.Add(TileCtrl.current.Get<MyTileData>("health"));

        CombatArena.current.enemy?.SetDialog("Take a look at the tiles I offer");
        UIHudSelectTile.current.ShowShop(list, rng);
    }


    // Select skill or healing
    public void InitCamp()
    {
        var stageData = StageCtrl.current.Data;
        var rng = endLevelRandom;

        var rarity = Rarity.Common; //ToDo: stageData.reward rarity
        var list = SkillCtrl.current.GetAll()
                .Where(x => x.rarity == rarity)
                .Cast<IHasInfo>()
                .ToList()
                .RandN(3, endLevelRandom);

        string dialog;
        bool noSkills = Game.current.IsFirstEncounter(); //Game.current.GetSkills().Count == 0;
        if (noSkills)
        {
            dialog = "Arcane board? I thought they no longer worked.. Never mind, let me teach you an arcane skill";
        }
        else
        {
            dialog = "Want to learn more skills? Or perhaps you want to heal wounds. Choose one!";
            list.Add(TileCtrl.current.Get<MyTileData>("health"));
        }

        CombatArena.current.enemy.SetDialog(dialog);

        UIHudSelectTile.current.ShowChoise(list)
            .SetCanSkip(false);
            //.SetCanSkip(!noSkills);
    }

    void HandleCardSelected(UISelectTileCard card, SelectTileType type)
    {
        var stageData = StageCtrl.current.Data;
        if (stageData.type == StageType.Camp)
        {
            if (Game.current.IsFirstEncounter())
            {
                AddState(new CombatStates.Dialog("teacher0"));
            }
            else
            {
                if (card.data is TileData)
                {
                    AddState(new CombatStates.Dialog("camp_healing"));
                }
                else if (card.data is SkillData)
                {
                    AddState(new CombatStates.Dialog("camp_skill"));
                }
            }
        }

    }

    public void Init(StageType type, string dialogId = null)
    {
        if (!string.IsNullOrEmpty(dialogId))
        {
            UIHudDialog.current.Show(dialogId);
            return;
        }

        switch (type)
        {
            case StageType.Enemy:
            case StageType.Elite:
            case StageType.Boss:
                StartCoroutine(InitCombat());
                break;
            case StageType.Shop:
                InitShop();
                break;
            case StageType.Camp:
                InitCamp();
                break;
               /* 
            case StageType.Victory:
                Game.current.GameOver();
                UIHudGameOver.current.Show(true);
                break;*/
            default:
                Debug.LogError($"Unknown Combat type init ({type})");
                CheckQueue();
                break;
        }
        UIHudDialog.current.MainInitDone();
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