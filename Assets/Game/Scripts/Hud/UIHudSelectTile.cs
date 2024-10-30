using GridBoard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class UIHudSelectTile : UIHudBase
{
    public static UIHudSelectTile _current;
    public static UIHudSelectTile current
    {
        get
        {
            if (!_current) _current = FindFirstObjectByType<UIHudSelectTile>();
            return _current;
        }
    }

    [SerializeField] UISelectTileCard templateCard;
    [SerializeField] GameObject bg;

    List<UISelectTileCard> cards = new();

    public void Show(SelectTileType type, List<MyTileData> list)
    {
        Opened();
        Clear();
        foreach (var data in list)
        {
            var instance = UIUtils.CreateFromTemplate(templateCard);
            instance.Init(type, data);
            cards.Add(instance);
        }
        bg.SetActive(type == SelectTileType.Choise);
    }

    private void OnEnable()
    {
        UISelectTileCard.OnSelectTile += HandleTileSelected;
    }

    private void OnDisable()
    {
        UISelectTileCard.OnSelectTile -= HandleTileSelected;
    }

    void Clear()
    {
        foreach (var item in cards)
        {
            if (item) Destroy(item.gameObject);
        }
        cards.Clear();
    }

    public void Close()
    {
        Clear();
        Closed();
    }

    void HandleTileSelected(UISelectTileCard card)
    {
        Close();
    }
}

public enum SelectTileType
{
    None,
    Choise,
    Shop
}