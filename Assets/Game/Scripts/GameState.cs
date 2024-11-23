using FancyToolkit;
using RogueLikeMap;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public const string prefsKey = "gamesave_beta0.1";

    // serializeable stuff
    public int seed;
    public List<int> visitedNodes;
    public int currentNode;
    public Vector2Int playerHealth;
    public List<string> deck;
    public List<string> skills;
    public int gold;

    public bool HasCurrentNode
    {
        get => currentNode != -1;
        set
        {
            if (!value)
            {
                currentNode = -1;
            }
        }
    }

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

    public MapLayout GenerateMapLayout()
    {
        var mapSize = new Vector2Int(9, 6);
        var rng = new System.Random(seed);
        var mapLayout = MapGenerator.GenerateLayout(mapSize, 6, rng, 1);

        // ToDo: adjust difficulty       0, 1, 2, 3, 4, 5, 6
        //var difficulty = new List<int> { 1, 3, 1, 1, 3 };
        //var typeGenerator = new NodeProbability.Generator(randomNodes);

        var stageCtrl = StageCtrl.current;

        foreach (var item in mapLayout.nodes)
        {
            var x = item.pos.x;
            var difficulty = x;
            if (!HasCurrentNode && x == 0 && visitedNodes.Count == 0 )    // starting node
            {
                item.state = NodeState.Available;
            }

            item.type = StageCtrl.current.GetRandom(difficulty, rng);
            if (item.type == null)
            {
                Debug.LogError($"Node with difficulty {x} not found");
                return null;
            }
        }

        if (HasCurrentNode)
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

            if (!HasCurrentNode)
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
        HasCurrentNode = false;
        this.playerHealth = new(100, 100);
        this.deck = Game.current.GetStartingDeck();
        this.skills = new List<string>();
        this.gold = 200;
        GenerateMapLayout();
    }
}