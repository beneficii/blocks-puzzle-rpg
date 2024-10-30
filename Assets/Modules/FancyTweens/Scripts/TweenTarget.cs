using DG.Tweening;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace FancyTweens
{
    public class TweenTarget : MonoBehaviour
    {
        private Vector3 originalScale;

        private void Awake()
        {
            originalScale = transform.localScale;
        }

        public void ScaleUpDown(float duration = 0.2f, float scaleUpSize = 1.4f)
        {
            DOTween.Kill(transform, complete: true);

            // Create new scale-up and scale-down sequence
            Sequence attentionSequence = DOTween.Sequence();

            // Set OnKill to ensure scale is reset if interrupted
            attentionSequence.OnKill(() => transform.localScale = originalScale);

            attentionSequence.Append(transform.DOScale(originalScale * scaleUpSize, duration))
                             .Append(transform.DOScale(originalScale, duration))
                             .SetEase(Ease.OutBounce);
        }
    }
}