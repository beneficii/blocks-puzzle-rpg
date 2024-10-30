using FancyToolkit;
using UnityEngine;

namespace RogueLikeMap
{
    [System.Serializable]
    public class NodeInfo
    {
        public int index;
        public Vector2Int pos;
        public int random;
        public NodeType type;
        public NodeState state;

        public NodeInfo(int index, Vector2Int pos, int random)
        {
            this.index = index;
            this.pos = pos;
            this.random = random;
            state = NodeState.Default;
            type = null;
        }
    }

    [System.Serializable]
    public class NodeInfoRaw
    {
        public int idx;
        public Vector2Int pos;
        public int random;
    }
}
