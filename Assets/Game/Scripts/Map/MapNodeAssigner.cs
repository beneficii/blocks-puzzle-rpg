using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using RogueLikeMap;
using FancyToolkit;

public class MapNodeAssigner
{
    System.Random rng;
    List<List<Node>> nodes;

    public MapLayout layout;
    public Vector2Int mapSize;
    public int maxLevelX;

    public MapNodeAssigner(System.Random rng)
    {
        this.rng = rng;
    }

    public void ActTutorial()
    {
        InitLayout(2, 1, 1, 0);

        void SetNode(int row, StageType type, string id)
        {
            var node = nodes[row][0];
            node.type = type;
            node.definedId = id;
        }


        SetNode(0, StageType.Enemy, "tutorial1");
        SetNode(1, StageType.Dialog, "tutorial2");
        SetNode(2, StageType.Camp, "tutorial3");
        Finish();
    }

    public void Act1()
    {
        InitLayout(9, 6, 8, 1);

        // first row is enemy
        foreach (var node in nodes[0]) node.type = StageType.Enemy;

        int bossRow = 9;
        // boss row
        foreach (var node in nodes[bossRow])
        {
            node.type = StageType.Boss;
            node.definedId = "boss_dragon";
        }

        // camp before boss
        foreach (var node in nodes[bossRow - 1]) node.type = StageType.Camp;

        // last row
        foreach (var node in nodes[bossRow + 1])
        {
            node.type = StageType.Dialog;
            node.definedId = "ending_demo";
        }

        Finish();
    }

    void InitLayout(int x, int y, int paths, int endNodes)
    {
        mapSize = new Vector2Int(x,y);
        layout = MapGenerator.GenerateLayout(mapSize, paths, rng, endNodes);

        nodes = new();
        maxLevelX = layout.nodes.Max(x => x.pos.x);
        for (int i = 0; i <= maxLevelX; i++) nodes.Add(new());
        var ordered = new List<Node>();
        Dictionary<Vector2Int, int> idxMap = new();

        for (int i = 0; i < layout.nodes.Count; i++)
        {
            var iNode = layout.nodes[i];
            idxMap.Add(iNode.pos, i);
            var node = new Node
            {
                info = iNode
            };
            ordered.Add(node);
            nodes[iNode.pos.x].Add(node);
        }

        foreach (var edge in layout.edges)
        {
            var a = idxMap[edge.a];
            var b = idxMap[edge.b];
            ordered[a].next.Add(ordered[b]);
            ordered[b].prev.Add(ordered[a]);
        }

    }

    void Finish()
    {
        int dbgFilterMiss = 0;
        int totalUnasignedNodes = nodes
           .SelectMany(x => x)
           .Count(x => x.type == StageType.None);

        var bucket = new FancyEnumBucket<StageType>(new Dictionary<StageType, int>
        {
            { StageType.Enemy, 24 },
            { StageType.Dialog, 20 },
            { StageType.Camp, 12 },
            { StageType.Elite, 8 },
            { StageType.Shop, 5 },
        }, StageType.Enemy, totalUnasignedNodes);

        foreach (var row in nodes)
        {
            foreach (var node in row)
            {
                if (node.type != StageType.None)
                {
                    node.info.type = node.GetStageNode();
                    continue;
                }
                var filter = GetFilter(node);
                var picked = StageType.Enemy;
                if (filter.Count == 0)
                {
                    dbgFilterMiss++;
                    bucket.RemoveOne(picked);
                }
                else
                {
                    picked = bucket.PickOne(filter, rng);
                }

                node.type = picked;
                node.info.type = node.GetStageNode();
            }
        }

        //Debug.Log($"Finished Setup. missed: {dbgFilterMiss}. totalFilled: {totalUnasignedNodes}");
    }

    protected virtual List<StageType> GetFilter(Node node)
    {
        var set = new HashSet<StageType>
        {
            StageType.Enemy,
            StageType.Dialog,
            StageType.Shop,
        };

        if (node.info.pos.x > 2) set.Add(StageType.Camp);
        if (node.info.pos.x > 3) set.Add(StageType.Elite);


        foreach (var item in node.prev)
        {
            if (item.type == StageType.Shop ||
                item.type == StageType.Elite ||
                item.type == StageType.Camp)
            {
                set.Remove(item.type);
            }

            foreach (var next in item.next)
            {
                set.Remove(next.type);
            }
        }

        foreach (var item in node.next)
        {
            if (item.type == StageType.Shop ||
                item.type == StageType.Elite ||
                item.type == StageType.Camp)
            {
                set.Remove(item.type);
            }
        }

        return set.ToList();
    }

    protected virtual bool CanAssign(Node node, StageType type)
    {
        //should not assign node that was already assinged
        Assert.IsTrue(node.type == StageType.None);

        // all next nodes must be unique
        foreach (var prev in node.prev)
        {
            var existing = new List<StageType> { type };
            foreach(var next in prev.next)
            {
                if (next.type == StageType.None) continue;

                if (existing.Contains(next.type)) return false;

                existing.Add(next.type);
            }
        }

        // can not have 2 shops, elites or camps in a row
        if (type == StageType.Elite || type == StageType.Shop || type == StageType.Camp)
        {
            foreach (var prev in node.prev)
            {
                if (prev.type == type) return false;
            }

            foreach (var next in node.next)
            {
                if (next.type == type) return false;
            }
        }



        return true;
    }

    public class Node
    {
        public HashSet<Node> prev;
        public HashSet<Node> next;

        public StageType type;
        public NodeInfo info;
        public string definedId;

        public Node()
        {
            prev = new();
            next = new();
            type = StageType.None;
            info = null;
            definedId = null;
        }

        public NodeTypeStage GetStageNode()
        {
            var stageNode = new NodeTypeStage(type)
            {
                definedId = definedId,
                sprite = StageCtrl.current.GetSprite(type),
            };

            return stageNode;
        }
    }
}


