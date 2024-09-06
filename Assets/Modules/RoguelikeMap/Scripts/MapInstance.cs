using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using FancyToolkit;

namespace RogueLikeMap
{
    public class MapInstance : MonoBehaviour
    {
        [SerializeField] MapNode prefabNode;
        [SerializeField] LineRenderer prefabLine;
        [SerializeField] Vector2 spacing = new(2,2);
        [SerializeField] float spread = 0.4f;
        [SerializeField] Transform container;

        [SerializeField] List<Material> materialLineDefault;
        [SerializeField] List<Material> materialLineCurrent;

        Dictionary<Vector2Int, MapNode> nodes;
        Dictionary<Vector2Int, List<EdgeInfo>> edges;

        MapNode currentNode;
        List<MapNode> availableNodes;
        List<EdgeInfo> availablePaths;

        public void Init(MapLayout layout)
        {
            nodes = new();
            edges = new();
            availableNodes = new();
            availablePaths = new();

            foreach (var item in layout.nodes)
            {
                var pos = item.pos;
                var worldPos = transform.position + new Vector3(pos.x * spacing.x, pos.y * spacing.y);
                var instance = Instantiate(prefabNode, worldPos.RandomAround(spread), Quaternion.identity, container);
                instance.Init(item);
                nodes.Add(item.pos, instance);
                instance.SetState(item.state);
                if(item.state == NodeState.Available)
                {
                    availableNodes.Add(instance);
                }
            }

            foreach (var item in layout.edges)
            {
                var a = nodes.Get(item.a);
                var b = nodes.Get(item.b);

                Assert.IsNotNull(a);
                Assert.IsNotNull(b);

                var direction = (b.transform.position - a.transform.position).normalized * 0.55f;

                var line = DrawLine(a.transform.position + direction, b.transform.position - direction);
                var key = item.a;
                var list = edges.Get(key);
                if (list == null)
                {
                    edges[key] = new();
                }

                edges[key].Add(new EdgeInfo
                {
                    edge = item,
                    render = line,
                });
            }
        }

        private void OnEnable()
        {
            MapNode.OnClicked += HandleNodeClick;
        }

        private void OnDisable()
        {
            MapNode.OnClicked -= HandleNodeClick;
        }

        protected virtual void HandleNodeClick(MapNode node)
        {
            if (node.info.state == NodeState.Available)
            {
                currentNode?.SetState(NodeState.Visited);
                currentNode = node;

                node.SetState(NodeState.Current);

                foreach (var item in availableNodes)
                {
                    if (item == node) continue;
                    item.SetState(NodeState.Default);
                }
                availableNodes.Clear();

                foreach (var item in availablePaths)
                {
                    if (item.edge.b == node.info.pos) continue;
                    item.render.SetMaterials(materialLineDefault);
                }
                availablePaths.Clear();

                if (edges.TryGetValue(node.info.pos, out var edgeList))
                {
                    foreach (var item in edgeList)
                    {
                        var b = nodes.Get(item.edge.b);
                        Assert.IsNotNull(b);

                        availableNodes.Add(b);
                        b.SetState(NodeState.Available);

                        availablePaths.Add(item);
                        item.render.SetMaterials(materialLineCurrent);
                    }
                }
            }
        }

        LineRenderer DrawLine(Vector2 start, Vector2 end)
        {
            LineRenderer lineRenderer = Instantiate(prefabLine, container);

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, new(start.x, start.y));
            lineRenderer.SetPosition(1, new(end.x, end.y));

            return lineRenderer;
        }
    }
}
