using UnityEngine;

namespace RogueLikeMap
{
    [System.Serializable]
    public class NodeInfo
    {
        public Vector2Int pos;
        public NodeType type;
        public NodeState state;

        public NodeInfo(Vector2Int pos, NodeType type = null)
        {
            this.pos = pos;
            this.type = type;
            state = NodeState.Default;
        }
    }
}
