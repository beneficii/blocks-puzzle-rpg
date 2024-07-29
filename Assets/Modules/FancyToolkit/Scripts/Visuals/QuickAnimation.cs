using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FancyToolkit
{
    public class QuickAnimation : MonoBehaviour
    {
        public AnimationState defaultState;
        public List<Sprite> move;
        public Sprite jump;
        public Sprite iddle;

        SpriteRenderer render;

        CooldownComponent cooldown;
        int frameIdx = 0;

        AnimationState state = AnimationState.None;
        public AnimationState State
        {
            get => state;
            set
            {
                if (state == value) return;
                state = value;
                switch (value)
                {
                    case AnimationState.Move:
                        render.sprite = move[0];
                        break;
                    case AnimationState.Iddle:
                        render.sprite = iddle;
                        break;
                    case AnimationState.Jump:
                        render.sprite = jump;
                        break;
                }

                if (value == AnimationState.Move)
                {
                    cooldown = new CooldownComponent(0.2f);
                    frameIdx = 0;
                }
                else
                {
                    cooldown = null;
                }
            }
        }

        private void Awake()
        {
            render = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            if (defaultState != AnimationState.None)
            {
                State = defaultState;
            }
        }

        private void Update()
        {
            if (cooldown == null) return;

            if (state == AnimationState.Move && cooldown.Use())
            {
                frameIdx = (frameIdx + 1) % move.Count;
                render.sprite = move[frameIdx];
            }
        }
    }

    public enum AnimationState
    {
        None,
        Iddle,
        Move,
        Jump,
    }
}

