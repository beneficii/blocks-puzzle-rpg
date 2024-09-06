using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using FancyToolkit;

namespace RogueLikeMap
{
    public static class MapGenerator
    {
        public static MapLayout GenerateLayout(Vector2Int gridSize, int roads)
        {
            var edges = new HashSet<Edge>();
            var nodes = new HashSet<Vector2Int>();

            Vector2Int endTile = new(gridSize.x, gridSize.y / 2);

            void AddEdge(Vector2Int a, Vector2Int b)
            {
                edges.Add(new(a, b));
                nodes.Add(a);
                nodes.Add(b);
            }

            int curY;
            for (int i = 0; i < roads; i++)
            {
                curY = Random.Range(0, gridSize.y);
                for (int x = 1; x < gridSize.x; x++)
                {
                    var a = new Vector2Int(x - 1, curY);
                    var connections = new List<int>();

                    for (int y = curY - 1; y <= curY + 1; y++)
                    {
                        if (y != curY)
                        {
                            if (y < 0 || y >= gridSize.y) continue;
                            var crossEdge = new Edge(x - 1, y, x, curY);
                            if (edges.Contains(crossEdge)) continue;
                        }
                        
                        connections.Add(y);
                    }

                    var nextY = connections.Rand();
                    var b = new Vector2Int(x, nextY);
                    AddEdge(a, b);
                    curY = nextY;
                }

                AddEdge(new(gridSize.x - 1, curY), endTile);
            }

            return new MapLayout
            {
                edges = edges.ToList(),
                nodes = nodes.Select(x=>new NodeInfo(x)).ToList(),
            };
        }
    }

    public class MapLayout
    {
        public List<Edge> edges;
        public List<NodeInfo> nodes;
    }

    public enum NodeState
    {
        None,
        Default,
        Available,
        Current,
        Visited,
    }
}
