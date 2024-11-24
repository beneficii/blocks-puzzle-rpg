using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using FancyToolkit;

public class UIHudDeck : UIHudBase
{
    public static UIHudDeck _current;
    public static UIHudDeck current
    {
        get
        {
            if (!_current) _current = FindFirstObjectByType<UIHudDeck>();
            return _current;
        }
    }

    [SerializeField] UIHoverInfo templateInfoCard;

    List<UIHoverInfo> cards = new();

    public void Show()
    {
        Opened();
        Clear();
        foreach (var item in Game.current.GetDeck())
        {
            var card = UIUtils.CreateFromTemplate(templateInfoCard);
            card.Init(item);
            cards.Add(card);
        } 

    }

    void Clear()
    {
        foreach (var item in cards)
        {
            Destroy(item.gameObject);
        }
        cards.Clear();
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
    }
}
