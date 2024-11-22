using System.Collections;
using UnityEngine;
using DG.Tweening;

public class TweenOnEnable : MonoBehaviour
{
    [SerializeField] Type type = Type.Attention;
    [SerializeField] float animationTime = 0.7f;


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
            default:
                break;
        }
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    public enum Type
    {
        None,
        Attention,
        GrowIn,
    }
}