using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;
using TMPro;

public class UIHudGameOver : UIHudBase
{
    public static UIHudGameOver _current;
    public static UIHudGameOver current
    {
        get
        {
            if (!_current) _current = FindFirstObjectByType<UIHudGameOver>();
            return _current;
        }
    }

    [SerializeField] TextMeshProUGUI txtTitle;

    public void Show(bool victory)
    {
        txtTitle.text = victory ? "Victory!" : "Defeat";
        Opened();
    }

    public void Close()
    {
        Closed();
        Game.current.LoadScene();
    }
}
