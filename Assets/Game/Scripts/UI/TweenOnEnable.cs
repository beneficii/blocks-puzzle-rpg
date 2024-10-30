using System.Collections;
using UnityEngine;
using DG.Tweening;

public class TweenOnEnable : MonoBehaviour
{
    private void OnEnable()
    {
        var scale = transform.localScale;

        transform.localScale = scale * 0.9f;

        transform.DOScale(scale, 0.7f)
            .SetEase(Ease.InOutBack);
    }

    private void OnDisable()
    {
        transform.DOKill();
    }
}