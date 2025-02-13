using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using FancyToolkit;

public class UISliderTweener : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] RectTransform tweenerRect;

    [SerializeField] float speed = 2f;

    int cachedValue = -1;
    int cachedMax = -1;

    bool isAnimating;

    float Target => slider.fillRect.anchorMax.x;
    float Tweener
    {
        get => tweenerRect.anchorMax.x;
        set
        {
            tweenerRect.anchorMax = new Vector2(value, slider.fillRect.anchorMax.y);
        }
    }

    public void SetValue(int val, int max)
    {
        StartCoroutine(SetValueRoutine(val, max));
    }

    IEnumerator SetValueRoutine(int val, int max)
    {
        if (cachedMax != max)
        {
            slider.maxValue = max;
            cachedMax = max;
        }

        slider.value = val;
        int oldCached = cachedValue;
        cachedValue = val;
        if (oldCached == -1 )
        {
            yield return null;
            Tweener = Target;
            isAnimating = false;
        }
        else
        {
            if (val < oldCached) Tweener = 0; // means it got full

            yield return null;
            isAnimating = true;
        }
    }

    public void SetValueInstant(int val, int max)
    {
        StartCoroutine(SetValueInstantRoutine(val, max));
    }

    public IEnumerator SetValueInstantRoutine(int val, int max)
    {
        if (cachedMax != max)
        {
            slider.maxValue = max;
            cachedMax = max;
        }

        slider.value = val;
        yield return null;

        Tweener = Target;
        isAnimating = false;
    }

    private void Update()
    {
        if (!isAnimating) return;
        float tmp = Mathf.MoveTowards(Tweener, Target, speed * Time.deltaTime);
        Tweener = tmp;
        if (tmp == Target) isAnimating = false;
    }
}
