using System.Collections;
using TMPro;
using UnityEngine;
using RogueLikeMap;
using FancyToolkit;

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
    [SerializeField] UIGenericButton backButton;
    [SerializeField] GameObject tutorialHint;

    public void Show()
    {
        Opened();
        Game.current.HandleMapSceneReady(scene);
        bool canClose = Game.current.GetStateType() == Game.StateType.Combat;
        backButton.gameObject.SetActive(canClose);
        tutorialHint.gameObject.SetActive(Game.current.ShouldShowMapHint());
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
        if (Game.current.GetStateType() == Game.StateType.Map)
        {
            return;
        }
        Closed();
        scene.Clear();
    }
}
