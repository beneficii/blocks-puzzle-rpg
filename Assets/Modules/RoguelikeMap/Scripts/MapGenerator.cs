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
                nodes = nodes.Select((x,i)=>new NodeInfo(i, x, 0)).ToList(),
            };
        }

        public static MapLayout GenerateLayout(Vector2Int gridSize, int roads, System.Random rng, int endNodes = 1)
        {
            var edges = new HashSet<Edge>();
            var nodes = new HashSet<Vector2Int>();
            var edgeList = new List<Edge>();
            var nodeList = new List<Vector2Int>();

            Vector2Int endTile = new(gridSize.x, gridSize.y / 2);

            void AddEdge(Vector2Int a, Vector2Int b)
            {
                var edge = new Edge(a, b);

                if (!edges.Contains(edge))
                {
                    edges.Add(edge);
                    edgeList.Add(edge);
                }
                if (!nodes.Contains(a))
                {
                    nodes.Add(a);
                    nodeList.Add(a);
                }
                if (!nodes.Contains(b))
                {
                    nodes.Add(b);
                    nodeList.Add(b);
                }
            }

            int curY;
            for (int i = 0; i < roads; i++)
            {
                curY = rng.Next(0, gridSize.y);
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

                    var nextY = connections[rng.Next(0,connections.Count)];
                    var b = new Vector2Int(x, nextY);
                    AddEdge(a, b);
                    curY = nextY;
                }

                var curTile = new Vector2Int(gridSize.x - 1, curY);  // Start at the end of the main grid
                AddEdge(curTile, endTile);  // Connect to the endTile first
                curTile = endTile;  // Update curTile to endTile

                for (int j = 0; j < endNodes; j++)
                {
                    var nextTile = new Vector2Int(endTile.x + j + 1, endTile.y);
                    AddEdge(curTile, nextTile);
                    curTile = nextTile;
                }
            }

            return new MapLayout
            {
                edges = edgeList,
                nodes = nodeList
                        .Select((x, i) => new NodeInfo(i, x, rng.Next()))
                        .ToList()
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
