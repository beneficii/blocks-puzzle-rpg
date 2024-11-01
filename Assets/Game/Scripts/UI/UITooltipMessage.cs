using System.Collections;
using UnityEngine;
using TMPro;
using FancyToolkit;
using DG.Tweening;
using UnityEngine.UIElements;

public class UITooltipMessage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtMessage;
    [SerializeField] CanvasGroup cg;

    [SerializeField] float lifetime = 1.8f;
    [SerializeField] float riseDistance = 2f;
    [SerializeField] float fadeDuration = 0.4f;

    Sequence fadeSequence;

    IEnumerator Start()
    {
        transform.localScale = Vector3.one * .4f;
        transform.DOScale(Vector3.one, .3f)
            .SetEase(Ease.InOutBack);

        yield return new WaitForSeconds(lifetime);

        Fade();
    }

    public void Init(string text)
    {
        txtMessage.text = text;
    }

    public void Fade()
    {
        if (fadeSequence != null) return;
        
        fadeSequence = DOTween.Sequence();
        cg.alpha = .8f;
        fadeSequence.Append(transform.DOMoveY(transform.position.y + riseDistance, fadeDuration));
        fadeSequence.Join(cg.DOFade(0, fadeDuration));
        fadeSequence.OnComplete(() => Destroy(gameObject));

        // Start the sequence
        fadeSequence.Play();
    }

    private void OnDestroy()
    {
        fadeSequence.Kill();
        transform.DOKill();
    }
}