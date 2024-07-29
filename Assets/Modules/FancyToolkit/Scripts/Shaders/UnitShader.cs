using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FancyToolkit
{
    [DefaultExecutionOrder(-1)]
    public class UnitShader : MonoBehaviour
    {
        static Color defaultColor = new Color(0, 0, 0, 0);

        float tintUntil = -1f;

        Renderer render;
        MaterialPropertyBlock propBlock;

        List<System.Action> initActions = new List<System.Action>();

        private void Awake()
        {
            render = GetComponent<Renderer>();
        }

        private void Start()
        {
            propBlock = new MaterialPropertyBlock();
            render.GetPropertyBlock(propBlock);
            foreach (var item in initActions)
            {
                item.Invoke();
            }
            initActions.Clear();
        }

        // some Unity magic workaround
        public void AddInitAction(System.Action action)
        {
            if(propBlock == null)
            {
                initActions.Add(action);
            }
            else
            {
                action.Invoke();
            }
        }

        public void SetTeam(Color color, Color color2 = default)
        {
            propBlock.SetColor("_TeamColor", color);
            propBlock.SetColor("_TeamColor2", color2);
            render.SetPropertyBlock(propBlock);
        }

        public void SetOutline(Color color)
        {
            propBlock.SetColor("_OutlineColor", color);
            render.SetPropertyBlock(propBlock);
        }

        void SetTintColor(Color color)
        {
            propBlock.SetColor("_Tint", color);
            render.SetPropertyBlock(propBlock);
        }

        public void SetTint(Color color, float duration = 0.15f)
        {
            SetTintColor(color);
            tintUntil = Time.time + duration;
        }

        void AnimateTint()
        {
            if (tintUntil <= Time.time)
            {
                SetTintColor(defaultColor);
                tintUntil = -1f;
            }
        }

        private void Update()
        {
            if (tintUntil >= 0)
            {
                AnimateTint();
            }
        }
    }
}