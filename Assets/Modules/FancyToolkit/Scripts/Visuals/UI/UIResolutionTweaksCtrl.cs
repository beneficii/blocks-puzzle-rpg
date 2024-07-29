using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIResolutionTweaksCtrl : MonoBehaviour
{
    [SerializeField] int rWidth = 540;
    [SerializeField] int rHeight = 960;

    [SerializeField] List<GameObject> aboveRatioObjs;
    [SerializeField] List<GameObject> belowRatioObjs;

    public string debug;

    Vector2 cachedScreenSize;
    bool isBelowRatio;

    private void Start()
    {
        HandleResized(true);
    }

    void HandleResized(bool forceUpdate = false)
    {
        if (Screen.height == 0) return;
        cachedScreenSize = new Vector2(Screen.width, Screen.height);

        var currentRatio = Screen.width / (float)Screen.height;
        bool wasBelow = isBelowRatio; 
        isBelowRatio = currentRatio < rWidth / (float) rHeight;

        if (!forceUpdate && (isBelowRatio == wasBelow)) return;

        foreach (var item in aboveRatioObjs) item.SetActive(!isBelowRatio);
        foreach (var item in belowRatioObjs) item.SetActive(isBelowRatio);
    }

    void Update()
    {
        if (Screen.width != cachedScreenSize.x || Screen.height != cachedScreenSize.y)
        {
            HandleResized();
        }
#if UNITY_EDITOR
        debug = $"{Screen.width} / {Screen.height} = {Screen.width / (float)Screen.height} [{isBelowRatio}]";
#endif
    }
}