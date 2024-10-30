using DG.Tweening;
using FancyToolkit;
using UnityEngine;

namespace RogueLikeMap
{
    public class NodeType : DataWithId
    {
        public Sprite sprite;

        public void Clicked(NodeInfo info)
        {
            var state = info.state;
            if (state == NodeState.Available)
            {
                Run(info);
                //Debug.Log("Clicked new node");
                return;
            }

            if (state == NodeState.Current)
            {
                Debug.Log("Clicked on current node");
                return;
            }

            Debug.Log($"Clicked on non aviable node (state: {state})");
        }

        public virtual void Run(NodeInfo info)
        {
            Debug.Log("Node::Run");
        }
    }
}
