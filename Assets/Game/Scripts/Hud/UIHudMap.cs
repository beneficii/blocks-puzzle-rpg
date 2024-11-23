using System.Collections;
using TMPro;
using UnityEngine;
using RogueLikeMap;

public class UIHudMap : UIHudBase
{
    public static UIHudMap _current;
    public static UIHudMap current
    {
        get
        {
            if (!_current) _current = FindFirstObjectByType<UIHudMap>();
            return _current;
        }
    }

    [SerializeField] MapScene scene;

    public void Show()
    {
        Opened();
        Game.current.HandleMapSceneReady(scene);
    }

    public void Toggle()
    {
        if (IsOpen)
        {
            Close();
        }
        else
        {
            Show();
        }
    }

    public void Close()
    {
        Closed();
        scene.Clear();
    }
}
