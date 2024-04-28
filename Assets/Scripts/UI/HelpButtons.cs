using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpButtons : MonoBehaviour
{
    public RectTransform panelCheatButtons;


    IEnumerator RoutineAnimate(RectTransform panel, float targetSize)
    {
        var size = panel.localScale;
        while (size.x != targetSize)
        {
            size.x = Mathf.MoveTowards(size.x, targetSize, Time.deltaTime * 10);
            panel.localScale = size;
            yield return null;
        }
    }

    public void BtnSlideCheatButtons()
    {
        StopAllCoroutines();
        if (panelCheatButtons.localScale.x > 0.1f)
        {
            StartCoroutine(RoutineAnimate(panelCheatButtons, 0));
        }
        else
        {
            StartCoroutine(RoutineAnimate(panelCheatButtons, 1));
        }
    }
}