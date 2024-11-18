using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using TileShapes;
using FancyToolkit;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

public class CombatCtrl : MonoBehaviour, ILineClearHandler
{
    [SerializeField] List<SpriteRenderer> bgRenders;
    [SerializeField] Button btnEndTurn;

    Board board;
    ShapePanel shapePanel;

    CombatArena arena;

    public bool EndTurnInProgress { get; private set; } = false;
    Sequence animSequence;

    PoolQueue<TileData> tileQueue;

    public int tilesPerTurn = 5;

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
        animSequence?.Kill();
    }

    void HandleTileAddToSet(UISelectTileCard card)
    {
        tileQueue.Add(card.data);
        Game.current.AddTileToDeck(card.data.id);
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

    List<UICombatReward.Data> GenerateCombatRewards()
    {
        var rng = Game.current.CreateStageRng();
        var rarity = StageCtrl.current.Data.reward;

        int gold = rarity switch
        {
            Rarity.Common => rng.Next(20, 40),
            Rarity.Uncommon => rng.Next(60, 100),
            Rarity.Rare => rng.Next(130, 180),
            Rarity.Legendary => rng.Next(300, 400),
            _ => Random.Range(45, 60),
        };

        return new List<UICombatReward.Data> {
            new UICombatReward.DataGold(gold),
            new UICombatReward.DataTile(rarity)
        };
    }

    void HandleDeadEnd()
    {
        animSequence?.Kill();
        btnEndTurn.transform.localScale = Vector3.one;
        // Sequence to pulse the button up and down
        animSequence = DOTween.Sequence();
        animSequence.Append(btnEndTurn.transform.DOScale(1.2f, .2f).SetEase(Ease.OutQuad))
                .Append(btnEndTurn.transform.DOScale(0.9f, .2f).SetEase(Ease.InQuad))
                .SetLoops(2, LoopType.Yoyo);
    }

    IEnumerator CombatFinished(bool victory)
    {
        yield return new WaitForSeconds(2f);

        if (victory)
        {
            //if (StageCtrl.current.Data.type == StageData.Type.Boss)
            //{
            //    UIHudGameOver.current.Show(true);
            //}
            //else
            {
                UIHudRewards.current.Show(GenerateCombatRewards());
            }
        }
        else
        {
            Game.current.GameOver();
            UIHudGameOver.current.Show(false);
        }

    }


    public IEnumerator NewTurna()
    {
        yield break;
    }

    public void NewTurn() => NewTurn(0.1f);

    public void NewTurn(float delay)
    {
        StartCoroutine(TurnRoutine(delay));
    }

    public IEnumerator TurnRoutine() => TurnRoutine(0.4f);
    public IEnumerator TurnRoutine(float delay)
    {
        if (EndTurnInProgress)
        {
            yield break; // ToDo: maybe soome warning
        }
        btnEndTurn.GetComponent<CanvasGroup>().alpha = 0.3f;
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
        btnEndTurn.GetComponent<CanvasGroup>().alpha = 1f;
        EndTurnInProgress = false;
    }

    IEnumerator InitCombat()
    {
        var stageData = StageCtrl.current.Data;

        UIHudCombat.current.Show();
        board = FindAnyObjectByType<Board>();
        board.Init();

        shapePanel = FindAnyObjectByType<ShapePanel>();

        board.LoadRandomLayout(stageData.specialTile);
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
                Game.current.FinishLevel();
                break;
        }
    }

    IEnumerator Start()
    {
        var stageData = StageCtrl.current.Data;
        if (Game.current.bgDict.TryGetValue(stageData.background, out var bgSprite))
        {
            foreach (var item in bgRenders) item.sprite = bgSprite;
        }

        arena = CombatArena.current;
        var enemy = arena.SpawnEnemy(stageData.units[0]);
        var player = arena.SpawnPlayer();
        enemy.SetCombatVisible(false);
        //player.SetCombatVisible(false);
        var hp = Game.current.GetPlayerHealth();
        player.SetHp(hp.x);

        yield return null;
        Init(stageData.type, stageData.dialog);
    }


    private void Update()
    {
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
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.M))
        {
            MainUI.current.ShowMessage("Some test message!");
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

public enum MatchStat
{
    None,
    MaxDamage,
    MaxArmor
}