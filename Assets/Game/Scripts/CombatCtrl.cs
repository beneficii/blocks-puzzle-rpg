using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using TileShapes;
using FancyToolkit;
using System.Linq;
using ClearAction;

public class CombatCtrl : MonoBehaviour
{
    [SerializeField] TextAsset tableTiles;
    [SerializeField] TextAsset tableStartingTiles;

    Board board;
    TileShapes.ShapePanel shapePanel;

    CombatArena arena;

    public bool EndTurnInProgress { get; private set; } = false;

    PoolQueue<TileData> tileQueue;

    public int tilesPerTurn = 5;

    private void Awake()
    {
        TileCtrl.current.AddData<MyTileData>(tableTiles);
        
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
        LineClearer.OnCleared += HandleLinesCleared;
        //BtGrid.OnBoardChanged += HandleBoardChanged;
        shapePanel.OnOutOfShapes += NewTurn;
        Unit.OnKilled += HandleUnitKilled;
        UISelectTileCard.OnSelectTile += HandleTileAddToSet;
        UISelectTileCard.OnBuyTile += HandleTileAddToSet;
    }

    private void OnDisable()
    {
        LineClearer.OnCleared -= HandleLinesCleared;
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
            StartCoroutine(CombatFinished(unit.reward));
        }
    }

    public Unit SpawnEnemy(UnitData data)
    {
        var enemy = arena.SpawnEnemy(data);
        //var data = enemy.data;

        //BtGrid.current.LoadRandomBoard(data.boardLevel, data.specialBlockData);

        //board.LoadRandomLayout(data.boardLevel, data.specialBlockData); // ToDo

        board.LoadRandomLayout();
        shapePanel.GenerateNew(true, tileQueue, tilesPerTurn);

        return enemy;
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

    IEnumerator CombatFinished(BtUpgradeRarity reward)
    {
        yield return new WaitForSeconds(2f);
        HelpPanel.current.Close();  // just in case
        
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

    public void NewTurn() => NewTurn(0.1f);

    public void NewTurn(float delay)
    {
        if (EndTurnInProgress) return; // ToDo: maybe soome warning

        EndTurnInProgress = true;
        if (!arena.enemy) return;   // Let's wait end of combat

        StartCoroutine(TurnRoutine(delay));
    }

    GenericBullet MakeBullet(Tile parent, AnimCompanion fxPrefab = null)
    {
        var rand = Random.Range(0, 2) == 0;
        var bullet = DataManager.current.gameData.prefabBullet.MakeInstance(parent.transform.position)
            .AddSpleen(rand ? Vector2.left : Vector2.right)
            .SetSprite(parent.GetIcon());


        if (fxPrefab) bullet.SetFx(fxPrefab);

        return bullet;
    }

    IEnumerator TurnRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        //Temp stuff --

        foreach (var item in board.GetNonEmptyTiles())
        {
            if (item.data.id == "mushroomCurse")
            {
                MakeBullet(item)
                    .SetTarget(CombatArena.current.player)
                    .SetDamage(1)
                    .SetLaunchDelay(0.05f);
            }
        }

        //-- Temp stuff

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

        shapePanel.GenerateNew(false, tileQueue, tilesPerTurn);
        EndTurnInProgress = false;
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
        //SpawnEnemy("slime");
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