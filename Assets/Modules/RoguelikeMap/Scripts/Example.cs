using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueLikeMap;
using FancyToolkit;

namespace RogueLikeMap
{
    public class Example : MonoBehaviour
    {
        [SerializeField] MapInstance prefabMapInstance;
        [SerializeField] Transform mapParent;

        [SerializeField] Vector2Int mapSize = new(14, 7);
        [SerializeField] int mapRoads = 6;

        [SerializeField] List<NodeProbability> randomNodes;
        [SerializeField] NodeType normalNode;
        [SerializeField] List<NodeType> bossNodes;

        MapInstance mapInstance;

        void Start()
        {
            Generate();
        }

        void Generate()
        {
            if (mapInstance) Destroy(mapInstance.gameObject);
            mapInstance = Instantiate(prefabMapInstance, mapParent);
            var layout = MapGenerator.GenerateLayout(mapSize, mapRoads);

            var typeGenerator = new NodeProbability.Generator(randomNodes);

            foreach (var item in layout.nodes)
            {
                if (item.pos.x == 0)    // starting node
                {
                    item.type = normalNode;
                    item.state = NodeState.Available;
                }
                else if (item.pos.x == mapSize.x)   // boss node
                {
                    item.type = bossNodes.Rand();
                }
                else
                {
                    item.type = typeGenerator.GetRandom();
                }
            }

            mapInstance.Init(layout);
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Generate();
            }
        }
    }
}