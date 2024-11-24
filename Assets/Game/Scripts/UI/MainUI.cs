using GridBoard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class MainUI : MonoBehaviour
{
    static MainUI _current;
    public static MainUI current
    {
        get
        {
            if (!_current)
            {
                _current = FindAnyObjectByType<MainUI>();
            }

            return _current;
        }
    }

    [SerializeField] UITooltipMessage templateMessage;

    UITooltipMessage lastMessage;

    public void ShowMessage(string message)
    {
        if (lastMessage)
        {
            lastMessage.Fade();
        }

        lastMessage = UIUtils.CreateFromTemplate(templateMessage);
        lastMessage.Init(message);
    }

    public void BtnMap()
    {
        UIHudMap.current.Toggle();
    }

    public void BtnSettings()
    {
        UIHudSettings.current.Show();
    }

    public void BtnTiles()
    {
        UIHudDeck.current.Toggle();
    }
}