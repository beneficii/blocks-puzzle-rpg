﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDCtrl : MonoBehaviour
{
    public static HUDCtrl _current;
    public static HUDCtrl current
    {
        get
        {
            if (!_current) _current = FindFirstObjectByType<HUDCtrl>();
            return _current;
        }
    }

    public event System.Action OnAllClosed;

    Stack<UIHudBase> stack = new();

    public void HandleHudOpened(UIHudBase hud)
    {
        if (stack.TryPeek(out var top))
        {
            if (top == hud) return;
            top.SetContentVisible(false);
        }
        hud.SetContentVisible(true);
        stack.Push(hud);
    }

    public void HandleHudClosed(UIHudBase hud)
    {
        stack.Pop();
        hud.SetContentVisible(false);
        if (stack.TryPeek(out var top))
        {
            top.SetContentVisible(true);
        }
        else
        {
            OnAllClosed?.Invoke();
        }
    }

    public void CloseAllButTop()
    {
        if (stack.Count == 0) return;

        var top = stack.Pop();
        stack.Clear();
        stack.Push(top);
    }
}

public abstract class UIHudBase : MonoBehaviour
{
    public bool IsOpen => content.activeSelf;
    [SerializeField] protected GameObject content;

    public void Opened()
    {
        HUDCtrl.current.HandleHudOpened(this);
    }

    public void Closed()
    {
        HUDCtrl.current.HandleHudClosed(this);
    }

    protected virtual void OnConectentVisible()
    {

    }

    public void SetContentVisible(bool value)
    {
        content.SetActive(value);
        if (value) OnConectentVisible();
    }
}