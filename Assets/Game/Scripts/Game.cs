using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;
using UnityEngine.SceneManagement;
using RogueLikeMap;
using GridBoard;
using System.Linq;

[DefaultExecutionOrder(-20)]
public class Game : MonoBehaviour
{
    public static Game current { get; private set; }

    [SerializeField] GameData gameData;

    GameState state;

    public Dictionary<string, AnimCompanion> vfxDict;
    public Dictionary<string, FxData> fxDict;

    public Dictionary<string, GameObject> unitActionPrefabs;
    public Dictionary<string, Sprite> bgDict;

    int stageSeed;

    public GenericBullet MakeBullet(Vector2 position)
    {
        return Instantiate(gameData.prefabBullet, position, Quaternion.identity);
    }

    public void AddTileToDeck(string id)
    {
        state.deck.Add(id);
        state.Save();
    }

    public List<string> GetStartingDeck()
    {
        return FancyCSV.FromText<TileEntry>(gameData.tableStartingTiles.text)
            .SelectMany(x => Enumerable.Repeat(x.id, x.amount))
            .ToList();
    }

    void Init()
    {
        StageCtrl.current.AddData(gameData.tableStages);
        TileCtrl.current.AddData<MyTileData>(gameData.tableTiles);
        UnitCtrl.current.AddData<UnitData>(gameData.tableUnits);

        unitActionPrefabs = Resources.LoadAll<GameObject>("ActionVisuals").ToDictionary(x => x.name);
        fxDict = Resources.LoadAll<FxData>("FxData").ToDictionary(x => x.name);
        bgDict = Resources.LoadAll<Sprite>("Backgrounds").ToDictionary(x => x.name);
    }

    public System.Random CreateStageRng()
        => new System.Random(stageSeed);

    private void Awake()
    {
        if (current)
        {
            Destroy(gameObject);
            return;
        }

        current = this;
        DontDestroyOnLoad(gameObject);
        MapScene.OnReady += HandleMapSceneReady;

        Init();

        if (GameState.HasSave())
        {
            state = GameState.Load();
            SetCurrentStage();
        }
        else
        {
            NewGame(); //ToDo: control this from menu or smth
        }
    }

    void HandleMapSceneReady(MapScene scene)
    {
        if (state == null) NewGame();

        scene.CreateMap(state.GenerateMapLayout(), new System.Random(state.seed));
    }

    public void NewGame()
    {
        GameState.ClearSave();
        state = new GameState(Random.Range(0, int.MaxValue));
    }

    public void Continue()
    {
        state = GameState.Load();
    }

    public void RestartLevel()
    {
        Helpers.RestartScene();
    }

    public static void ToDo(string message)
    {
        Debug.Log(message);
    }

    void LoadScene()
    {
        SceneManager.LoadScene("Loading");
    }

    public Vector2Int GetPlayerHealth()
        => state.playerHealth;

    public List<TileData> GetDeck()
    {
        return state.deck
            .Select(TileCtrl.current.GetTile)
            .ToList();
    }

    public string GetSceneToLoad()
    {
        if (state == null)
        {
            // load menu
            Debug.LogError("ToDo: menu");
            return null;
        }
        else if (state.currentNode < 0)
        {
            return "Map";
        }
        else
        {
            return "Combat";
        }
    }

    void SetCurrentStage()
    {
        var idx = state.currentNode;
        if (idx < 0) return;
        var layout = state.GenerateMapLayout();
        var node = layout.nodes[idx];
        StageCtrl.current.SetStage((StageData)node.type);
        stageSeed = node.random;
    }

    public void EnterLevel(int idx)
    {
        state.currentNode = idx;
        SetCurrentStage();
        state.Save();
        LoadScene();
    }

    public void FinishLevel(int? playerHealth = null)
    {
        if (playerHealth.HasValue)
        {
            state.playerHealth.x = playerHealth.Value;
        }
        state.visitedNodes.Add(state.currentNode);
        state.currentNode = -1;
        state.Save();
        LoadScene();
    }

    public void GameOver()
    {
        GameState.ClearSave();
    }


    public FxAnimator CreateFX(string id, Vector2 position, System.Action action = null)
    {
        if (!fxDict.TryGetValue(id, out var data))
        {
            Debug.LogError($"No Fx with id `{id}` found!");
        }

        return CreateFX(data, position, action);
    }

    public FxAnimator CreateFX(FxData data, Vector2 position, System.Action action = null)
    {
        if (!data)
        {
            action?.Invoke();
            return null;
        }
        var instance = Instantiate(gameData.fxPrefab, position, Quaternion.identity);
        instance.Init(data, action);

        return instance;
    }

    class TileEntry
    {
        public string id;
        public int amount;
    }
}