using System.Collections.Generic;
using UnityEngine;

namespace RogueLikeMap
{
    [System.Serializable]
    public class NodeProbability
    {
        public int weight;
        public NodeType type;

        public class Generator
        {
            private List<NodeProbability> weights;
            private int totalWeight;

            public Generator(List<NodeProbability> weights)
            {
                this.weights = weights;
                totalWeight = 0;

                foreach (var weight in this.weights)
                {
                    totalWeight += weight.weight;
                }
            }

            public NodeType GetRandom()
            {
                int randomValue = Random.Range(0, totalWeight);
                int cumulativeWeight = 0;

                foreach (var nodeProbability in weights)
                {
                    cumulativeWeight += nodeProbability.weight;
                    if (randomValue < cumulativeWeight)
                    {
                        return nodeProbability.type;
                    }
                }

                throw new System.InvalidOperationException("The weights are not set up correctly.");
            }
        }
    }
}
