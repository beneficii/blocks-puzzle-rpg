//#define SKIP_TUTORIAL

using FancyToolkit;
using RogueLikeMap;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class GameState
{
    public const int emptyNodeId = -1;

    public const string prefsKey = "gamesave_beta0.1";

    // serializeable stuff
    public int seed;
    public List<int> visitedNodes;
    public int currentAct;
    public int currentNode;
    public int tilesPerTurn;
    public Vector2Int playerHealth;
    public List<string> deck;
    public List<string> skills;
    public List<string> glyphs;
    public List<string> encounteredStages;
    public int gold;

    public MapNodeAssigner nodeAssigner;

    public bool skipTutorialNow = false;

    public bool IsMapNode
    {
        get => currentNode >= 0;
        set
        {
            if (!value)
            {
                currentNode = -1;
            }
        }
    }
    public bool IsTutorialNode => currentNode < emptyNodeId;

    public static bool HasSave()
        => PlayerPrefs.HasKey(prefsKey);

    public static GameState Load()
    {
        if (!HasSave()) return new GameState(Random.Range(0, int.MaxValue));

        var json = PlayerPrefs.GetString(prefsKey);
        Debug.Log(json);
        return JsonUtility.FromJson<GameState>(json);
    }

    public void Save()
    {
        gold = ResCtrl<ResourceType>.current.Get(ResourceType.Gold);
        var json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(prefsKey, json);
        PlayerPrefs.Save();
    }

    public static void ClearSave()
    {
        PlayerPrefs.DeleteKey(prefsKey);
    }

    public void SwitchAct(int act)
    {
        currentAct = act;
        nodeAssigner = null;
        visitedNodes.Clear();
    }

    public LevelFinishType FinishLevel(int? playerHealth = null)
    {
        if (skipTutorialNow)
        {
            skipTutorialNow = false;
            encounteredStages.Add("skipTutorial");
            currentNode = emptyNodeId; // go to map
            SwitchAct(1);
            Save();
            return LevelFinishType.SkipTutorial;
        }

        if (playerHealth.HasValue)
        {
            this.playerHealth.x = playerHealth.Value;
        }

        var stageData = StageCtrl.current.Data;
        encounteredStages.Add(stageData.id);
        
        if (!IsMapNode)
        {
            currentNode = emptyNodeId; // go to map
            Save();
            return LevelFinishType.Map;
        }

        visitedNodes.Add(currentNode);

        Assert.IsNotNull(nodeAssigner);
        Assert.IsTrue(currentNode < nodeAssigner.layout.nodes.Count);

        var node = nodeAssigner.layout.nodes[currentNode];
        currentNode = emptyNodeId; // go to map

        // check if last node on the map
        if (node.pos.x == nodeAssigner.maxLevelX)
        {
            if (currentAct == -1)
            {
                SwitchAct(1);
                Save();
                return LevelFinishType.FinishAct;
            }
            else if (currentAct == 1)
            {
                return LevelFinishType.Victory;
            }
        }

        Save();
        return LevelFinishType.Map;
    }

    public MapLayout GenerateMapLayout()
    {
        var rng = new System.Random(seed);
        if (nodeAssigner == null)
        {
            nodeAssigner = new MapNodeAssigner(rng);
            switch (currentAct)
            {
                case -1:
                    nodeAssigner.ActTutorial();
                    break;
                case 0:
                case 1:
                    currentAct = 1;
                    nodeAssigner.Act1();
                    break;
                default:
                    Debug.Log("ToDo: go to menu?");
                    break;
            }
        }

        var mapLayout = nodeAssigner.layout;
        mapLayout.ResetStates();

        foreach (var item in mapLayout.nodes)
        {
            var x = item.pos.x;
            if (!IsMapNode && x == 0 && visitedNodes.Count == 0 )    // starting node
            {
                item.state = NodeState.Available;
            }
        }

        if (IsMapNode)
        {
            var cur =  mapLayout.nodes[currentNode];
            cur.state = NodeState.Current;
        }

        if (visitedNodes.Count > 0)
        {
            foreach (var idx in visitedNodes)
            {
                mapLayout.nodes[idx].state = NodeState.Visited;
            }

            if (!IsMapNode)
            {
                var cur = mapLayout.nodes[visitedNodes.Last()];
                // Unlock aviable nodes
                var aviable = new HashSet<Vector2Int>();
                foreach (var item in mapLayout.edges)
                {
                    if (item.a == cur.pos) aviable.Add(item.b);
                }

                foreach (var item in mapLayout.nodes)
                {
                    if (aviable.Contains(item.pos)) item.state = NodeState.Available;
                }
            }
        }

        return mapLayout;
    }

    // new game
    public GameState(int seed)
    {
        this.seed = seed;
        this.visitedNodes = new();
        this.encounteredStages = new();
        this.IsMapNode = false;   
        this.playerHealth = new(100, 100);
        this.deck = Game.current.GetStartingDeck();
        this.skills = new List<string>();
        this.glyphs = new List<string>();
        this.gold = 25;
        this.tilesPerTurn = 3;
        this.currentAct = -1;    // ToDo: skip tutorial if passed?
#if SKIP_TUTORIAL
        this.currentAct = 1;
#endif
        GenerateMapLayout();
    }
}

public enum LevelFinishType
{
    Map,
    FinishAct,
    SkipTutorial,
    Victory,
}