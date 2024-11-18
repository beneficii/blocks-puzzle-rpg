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
    const string sceneMenu = "MainMenu";
    const string sceneCombat = "Combat";
    const string sceneMap = "Map";

    public static Game current { get; private set; }
    public const int maxSkills = 4;

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
        //state.Save(); // don't save to prevent shop abuse
    }

    public void AddSkill(string id)
    {
        state.skills.Add(id);
    }

    public List<string> GetStartingDeck()
    {
        //return TileCtrl.current.GetAllTiles().Select(x => x.id).ToList();

        return FancyCSV.FromCSV<TileEntry>("StartingTiles")
            .SelectMany(x => Enumerable.Repeat(x.id, x.amount))
            .ToList();
    }

    void Init()
    {
        StageCtrl.current.AddCSV("Stages");

        TileCtrl.current.AddCSV<MyTileData>("Tiles");
        UnitCtrl.current.AddCSV<UnitData>("Units");
        SkillCtrl.current.AddCSV<SkillData>("Skills");

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

        // For testing autoload level and stuff
        if (SceneManager.GetActiveScene().name != sceneMenu)
        {
            if (GameState.HasSave())
            {
                Continue();
            }
            else
            {
                NewGame();
            }
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
        SetCurrentStage();
        ResCtrl<ResourceType>.current.Set(ResourceType.Gold, state.gold);
        ResCtrl<ResourceType>.current.Set(ResourceType.Level, state.visitedNodes.Count);
    }

    public void RestartLevel()
    {
        Helpers.RestartScene();
        ResCtrl<ResourceType>.current.Set(ResourceType.Gold, state.gold);
        ResCtrl<ResourceType>.current.Set(ResourceType.Level, state.visitedNodes.Count);
    }

    public static void ToDo(string message)
    {
        Debug.Log(message);
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("Loading");
    }

    public Vector2Int GetPlayerHealth()
        => state.playerHealth;

    public List<TileData> GetDeck()
    {
        return state.deck
            .Select(TileCtrl.current.Get)
            .ToList();
    }

    public List<SkillData> GetSkills()
    {
        return state.skills.Take(maxSkills)
            .Select(SkillCtrl.current.Get)
            .ToList();
    }

    public string GetSceneToLoad()
    {
        if (state == null)
        {
            return sceneMenu;
        }
        else if (state.currentNode < 0)
        {
            return sceneMap;
        }
        else
        {
            return sceneCombat;
        }
    }

    void SetCurrentStage()
    {
        var idx = state.currentNode;
        if (idx < 0) return;
        var layout = state.GenerateMapLayout();
        var node = layout.nodes[idx];
        StageCtrl.current.SetStage((StageData)node.type);
        ResCtrl<ResourceType>.current.Set(ResourceType.Level, node.pos.x);
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
        state = null;
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