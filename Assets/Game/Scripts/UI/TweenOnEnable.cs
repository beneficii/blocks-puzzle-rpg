using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using FancyToolkit;

public class TweenOnEnable : MonoBehaviour
{
    [SerializeField] Type type = Type.Attention;
    [SerializeField] float animationTime = 0.7f;
    [SerializeField] float distance = .4f;

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

    public void TweenJumpTopDown()
    {
        StartCoroutine(JumpDownRoutine());
    }

    IEnumerator JumpDownRoutine()
    {
        yield return new WaitForSeconds(.2f);
        float moveDistance = distance;
        Vector3 jumpDirection = transform.up * moveDistance;

        tweenSequence = DOTween.Sequence()
            .Append(transform.DOLocalMove(transform.localPosition + jumpDirection, animationTime / 2).SetEase(Ease.InOutSine))
            .Append(transform.DOLocalMove(transform.localPosition, animationTime / 2).SetEase(Ease.InOutSine))
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void TweenGrowIn()
    {
        var scale = transform.localScale;

        transform.localScale = scale * 0.2f;

        transform.DOScale(scale, animationTime)
            .SetEase(Ease.InOutBack);
    }

    void TweenFadeInOut()
    {
        if (TryGetComponent<SpriteRenderer>(out var render))
        {
            render.SetAlpha(1f);
            tweenSequence = DOTween.Sequence()
                .Append(render.DOFade(1f - distance, animationTime))
                .SetLoops(-1, LoopType.Yoyo);
        }
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
            case Type.JumpTopDown:
                TweenJumpTopDown();
                break;
            case Type.FadeInOut:
                TweenFadeInOut();
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
        JumpTopDown,
        FadeInOut,
    }
}