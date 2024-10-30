using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FancyToolkit;

namespace RogueLikeMap
{
    public class MapNode : MonoBehaviour
    {
        public static event System.Action<MapNode> OnClicked;

        [SerializeField] SpriteRenderer render;
        [SerializeField] SpriteRenderer circle;

        [SerializeField] public NodeInfo info;// { get; private set; }

        bool isClickInProgress;

        Sequence attentionSequence;

        public void Init(NodeInfo info)
        {
            this.info = info;
            render.sprite = info.type.sprite;
        }

        void RotateCircle()
        {
            circle.transform.rotation = Quaternion.Euler(0f, 0f, info.random%360);
        }

        public void Click()
        {
            if (info.state != NodeState.Available) return;
            if (isClickInProgress) return;
            isClickInProgress = true;

            Sequence sequence = DOTween.Sequence();

            RotateCircle();
            sequence.Join(circle.transform.DOScale(2f, .6f).From(4f));
            sequence.Join(circle.DOFade(1f, 0.2f).From(0.2f));
            sequence.OnComplete(() =>
            {
                info.type.Clicked(info);
                OnClicked?.Invoke(this);
                isClickInProgress = false;
            });
            sequence.Play();
        }

        private void OnMouseDown()
        {
            Click();
        }

        void ClearTweens()
        {
            render.DOKill();
            render.transform.DOKill();
            circle.DOKill();
            render.transform.localScale = Vector3.one;
            attentionSequence?.Kill();
            attentionSequence = null;
        }

        public void SetState(NodeState state)
        {
            //if (info.state == state) return;

            info.state = state;

            ClearTweens();

            switch (state)
            {
                case NodeState.Available:
                    circle.SetAlpha(0);
                    
                    attentionSequence = DOTween.Sequence()
                        .Append(render.transform.DOScale(1.2f, 0.5f).SetEase(Ease.InOutSine))
                        .Join(render.DOFade(0.5f, 0.5f).SetEase(Ease.InOutSine))
                        .Append(render.transform.DOScale(1f, 0.5f).SetEase(Ease.InOutSine))
                        .Join(render.DOFade(1f, 0.5f).SetEase(Ease.InOutSine))
                        .SetLoops(-1, LoopType.Yoyo);

                    attentionSequence.Play();
                    break;

                case NodeState.Visited:
                    RotateCircle();
                    circle.SetAlpha(1f);
                    circle.transform.localScale = Vector2.one * 2;
                    break;

                default:
                    circle.SetAlpha(0f);
                    break;
            }
        }

        private void OnDestroy()
        {
            ClearTweens();
        }
    }
}