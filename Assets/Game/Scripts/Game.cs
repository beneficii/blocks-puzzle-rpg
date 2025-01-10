//#define TEST_SKILLS
//#define TEST_TILES

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;
using UnityEngine.SceneManagement;
using RogueLikeMap;
using GridBoard;
using TileShapes;
using System.Linq;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using DG.Tweening;


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
    public Dictionary<string, GenericBullet> bulletDict;

    public Dictionary<string, GameObject> unitActionPrefabs;
    public Dictionary<string, Sprite> bgDict;


    int stageSeed;

    public bool initDone { get; private set; }

    GameObject ghostCursor;

    public GenericBullet MakeBullet(Vector2 position, string vfxId = null)
    {
        var prefab = gameData.prefabBullet;

        if (!string.IsNullOrWhiteSpace(vfxId) && bulletDict.TryGetValue(vfxId, out var vfx))
        {
            prefab = vfx;
        }


        return Instantiate(prefab, position, Quaternion.identity);
    }

    public void AddTileToDeck(string id)
    {
        state.deck.Add(id);
        //state.Save(); // don't save to prevent shop abuse
        RecordEvent(new AnalyticsEvents.TileSelected
        {
            TileId = id,
        });
    }

    public void AddSkill(string id)
    {
        state.skills.Add(id);
        RecordEvent(new AnalyticsEvents.SkillSelected
        {
            skillId = id,
        });
    }

    public List<string> GetStartingDeck()
    {
        //return TileCtrl.current.GetAllTiles().Select(x => x.id).ToList();

        return FancyCSV.FromCSV<TileEntry>("StartingTiles")
            .SelectMany(x => Enumerable.Repeat(x.id, x.amount))
            .ToList();
    }

    public void CreateGhostCursor(Vector2 start, Vector2 end)
    {
        RemoveGhostCursor();
        var instance = Instantiate(gameData.prefabGhostCursor, start, Quaternion.identity);
        instance.transform.DOMove(end, 2f)
            .SetLoops(-1, LoopType.Restart);

        ghostCursor = instance;
    }

    public void RemoveGhostCursor()
    {
        if (!ghostCursor) return;

        ghostCursor.transform.DOKill();
        Destroy(ghostCursor);
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
        bulletDict = Resources.LoadAll<GenericBullet>("Bullets").ToDictionary(x => x.name);
    }

    public System.Random CreateStageRng()
        => new System.Random(stageSeed);

    async void Awake()
    {

        if (current)
        {
            Destroy(gameObject);
            return;
        }

        current = this;
        DontDestroyOnLoad(gameObject);

        //MapScene.OnReady += HandleMapSceneReady;

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

        var serviceOptions = new InitializationOptions();
#if !UNITY_EDITOR
        serviceOptions.SetEnvironmentName("production");
#endif
        Shape.OnDroppedStatic += HandleShapeDropped;

        await UnityServices.InitializeAsync(serviceOptions);

        initDone = true;
    }

    void HandleShapeDropped(Shape shape, Vector2Int pos)
    {
        RemoveGhostCursor();
    }

    public void HandleMapSceneReady(MapScene scene)
    {
        if (state == null) NewGame();

        scene.CreateMap(state.GenerateMapLayout(), new System.Random(state.seed));
    }

    public void NewGame()
    {
        GameState.ClearSave();
        state = new GameState(Random.Range(0, int.MaxValue));
        SetCurrentStage();
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

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    public Vector2Int GetPlayerHealth()
        => state.playerHealth;

    public List<TileData> GetDeck()
    {
#if UNITY_EDITOR && TEST_TILES
        return TileCtrl.current.GetAll().ToList();
#else
        return state.deck
            .Select(TileCtrl.current.Get)
            .ToList();
#endif
    }

    public CombatSettings GetCombatSettings()
    {
        return new CombatSettings()
        {
            tilesPerTurn = state.tilesPerTurn
        };
    }

    public void AddTilesPerTurn()
    {
        state.tilesPerTurn++;
    }

    public List<SkillData> GetSkills()
    {
#if UNITY_EDITOR && TEST_SKILLS
        return SkillCtrl.current.GetAll().Take(maxSkills).ToList();
#else
        return state.skills.Take(maxSkills)
            .Select(SkillCtrl.current.Get)
            .ToList();
#endif
    }

    public string GetSceneToLoad()
    {
        if (state == null)
        {
            return sceneMenu;
        }
        else
        {
            return sceneCombat;
        }
    }

    public StateType GetStateType()
    {
        if (state.IsMapNode || state.IsTutorialNode)
        {
            return StateType.Combat;
        }
        else
        {
            return StateType.Map;
        }
    }

    void SetCurrentStage()
    {
        var idx = state.currentNode;
        if (idx > GameState.emptyNodeId)
        {
            var layout = state.GenerateMapLayout();
            var node = layout.nodes[idx];
            var nodeType = (NodeTypeStage)node.type;
            StageCtrl.current.SetStage(nodeType.GetData(node, state.currentAct));
            ResCtrl<ResourceType>.current.Set(ResourceType.Level, node.pos.x);
            stageSeed = node.random;
        }
        else if (idx < GameState.emptyNodeId)   // special levels
        {
            stageSeed = state.seed * -idx;
            var stage = StageCtrl.current.GetSpecial(state.currentAct, idx, CreateStageRng());
            StageCtrl.current.SetStage(stage);
            ResCtrl<ResourceType>.current.Set(ResourceType.Level, 0);
        }
    }

    public void EnterLevel(int idx)
    {
        state.currentNode = idx;
        SetCurrentStage();
        state.Save();
        LoadScene();
    }

    public bool IsFirstEncounter()
    {
        if (state == null) return false;

        var stageData = StageCtrl.current.Data;
        return !state.encounteredStages.Contains(stageData.id);
    }

    public List<string> GetEncounteredStages()
    {
        return state.encounteredStages;
    }

    public void FinishLevel(CombatSettings combatSettings, int? playerHealth = null)
    {
        if (state == null)
        {
            LoadScene();
            return;
        }

        if (playerHealth.HasValue)
        {
            state.playerHealth.x = playerHealth.Value;
        }
        if (state.IsMapNode)
        {
            state.visitedNodes.Add(state.currentNode);
        }
        var stageData = StageCtrl.current.Data;
        state.encounteredStages.Add(stageData.id);
        state.IsMapNode = false;
        state.tilesPerTurn = combatSettings.tilesPerTurn;
        state.Save();
        //LoadScene();
        UIHudMap.current.Show();

        RecordEvent(new AnalyticsEvents.LevelCompletion
        {
            health = playerHealth.HasValue ? playerHealth.Value : 0,
        });
        AnalyticsService.Instance.Flush();
    }

    public void RecordEvent(AnalyticsEvents.Base ev)
    {
        ev.userLevel = ResCtrl<ResourceType>.current.Get(ResourceType.Level);
        ev.seed = stageSeed;
        ev.leveId = state?.currentNode ?? -1;
        AnalyticsService.Instance.RecordEvent(ev);
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

    public enum StateType
    {
        None,
        Combat,
        Map,
    }
}