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

    public UIGenericButton uiBtnSettings;
    public UIGenericButton uiBtnTiles;
    public UIGenericButton uiBtnMap;

    [SerializeField] UITooltipMessage templateMessage;
    [SerializeField] AudioClip soundReject;

    UITooltipMessage lastMessage;

    public void ShowMessage(string message, bool rejectSound = true)
    {
        if (lastMessage)
        {
            lastMessage.Fade();
        }
        if (rejectSound)
        {
            soundReject?.PlayWithRandomPitch(.1f);
        }

        lastMessage = UIUtils.CreateFromTemplate(templateMessage);
        lastMessage.Init(message);
    }

    private void Start()
    {
        uiBtnMap.gameObject.SetActive(Game.current.ShouldShowMap());
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