﻿using GridBoard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class SelectTileScreen : MonoBehaviour
{
    public static event System.Action OnClosed;

    public static SelectTileScreen _current;
    public static SelectTileScreen current
    {
        get
        {
            if (!_current) _current = FindFirstObjectByType<SelectTileScreen>();
            return _current;
        }
    }

    [SerializeField] UISelectTileCard templateCard;

    List<UISelectTileCard> cards = new();

    public void Show(SelectTileType type, List<TileData> list)
    {
        Clear();
        //gameObject.SetActive(true);
        foreach (var data in list)
        {
            var instance = UIUtils.CreateFromTemplate(templateCard);
            instance.Init(type, data);
            cards.Add(instance);
        }
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
            if (item) Destroy(item);
        }
        cards.Clear();
    }

    public void Close()
    {
        Clear();
        //gameObject.SetActive(false);
        OnClosed?.Invoke();
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