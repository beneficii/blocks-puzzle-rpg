using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TweenOnEnable : MonoBehaviour
{
    [SerializeField] Type type = Type.Attention;
    [SerializeField] float animationTime = 0.7f;

    Sequence tweenSequence;

    public void TweenHighlight()
    {
        var render = GetComponent<Image>();
        if (!render) return;

        tweenSequence = DOTween.Sequence()
            .Append(render.DOFade(0.3f, animationTime).SetEase(Ease.InOutSine))
            .Append(render.DOFade(1f, animationTime).SetEase(Ease.InOutSine))
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void TweenAttention()
    {
        var scale = transform.localScale;

        transform.localScale = scale * 0.9f;

        transform.DOScale(scale, animationTime)
            .SetEase(Ease.InOutBack);
    }

    public void TweenGrowIn()
    {
        var scale = transform.localScale;

        transform.localScale = scale * 0.2f;

        transform.DOScale(scale, animationTime)
            .SetEase(Ease.InOutBack);
    }

    private void OnEnable()
    {
        switch (type)
        {
            case Type.Attention:
                TweenAttention();
                break;
            case Type.GrowIn:
                TweenGrowIn();
                break;
            case Type.Highlight:
                TweenHighlight();
                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        transform.DOKill();
        tweenSequence?.Kill();
    }

    public enum Type
    {
        None,
        Attention,
        GrowIn,
        Highlight,
    }
}