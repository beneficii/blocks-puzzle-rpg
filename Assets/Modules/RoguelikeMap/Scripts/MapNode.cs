using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueLikeMap
{
    public class MapNode : MonoBehaviour
    {
        public static event System.Action<MapNode> OnClicked;

        [SerializeField] SpriteRenderer render;
        Animator animator;

        [SerializeField] public NodeInfo info;// { get; private set; }

        public NodeState state { get; private set; }

        public void Init(NodeInfo info)
        {
            this.info = info;
            render.sprite = info.type.sprite;
        }

        public void Click()
        {
            info.type.Clicked(info);
            OnClicked?.Invoke(this);
        }

        private void OnMouseDown()
        {
            Click();
        }

        public void SetState(NodeState state)
        {
            //if (info.state == state) return;

            info.state = state;
            if (!animator && !TryGetComponent<Animator>(out animator))
            {
                Debug.LogError("Map node needs animator!");
                return;
            }
            
            animator.Play(state.ToString());
        }
    }
}