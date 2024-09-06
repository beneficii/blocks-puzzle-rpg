using UnityEngine;

namespace RogueLikeMap
{
    [CreateAssetMenu(menuName = "Game/RoguelikeMap/NodeType")]
    public class NodeType : ScriptableObject
    {
        public Sprite sprite;

        public void Clicked(NodeInfo info)
        {
            var state = info.state;
            if (state == NodeState.Available)
            {
                Debug.Log("Clicked new node");
                return;
            }

            if (state == NodeState.Current)
            {
                Debug.Log("Clicked on current node");
                return;
            }

            Debug.Log($"Clicked on non aviable node (state: {state})");
        }

        public virtual void Run()
        {
            Debug.Log("Node::Run");
        }
    }
}
