﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using FancyToolkit;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using GridBoard;

public class UICombatReward : MonoBehaviour
{
    public static event System.Action<UICombatReward> OnClicked;

    [SerializeField] Image imgIcon;
    [SerializeField] TextMeshProUGUI txtCaption;
    

    [Header("Icons")]
    [SerializeField] Sprite iconGold;
    [SerializeField] Sprite iconTile;

    public RewardType id { get; private set; }
    List<string> tokens;

    public void Init(string data)
    {
        tokens = data.Split(' ').ToList();
        Assert.IsTrue(tokens.Count > 0);

        id = EnumUtil.Parse<RewardType>(tokens[0]);

        switch (id)
        {
            case RewardType.Gold:
                imgIcon.sprite = iconGold;
                txtCaption.text = $"{tokens[1]} gold";
                break;
            case RewardType.Tile:
                imgIcon.sprite = iconTile;
                txtCaption.text = $"Pick a tile";
                break;
            default:
                Debug.LogError($"Unknown reward type: {tokens[0]}");
                Destroy(gameObject);
                break;
        }
    }

    public void Remove()
    {
        // ToDo: maybe some disapear animation
        Destroy(gameObject);
    }

    public void Click()
    {
        switch (id)
        {
            case RewardType.Gold:
                if (tokens.Count > 1 && int.TryParse(tokens[1], out var n))
                {
                    ResCtrl<ResourceType>.current.Add(ResourceType.Gold, n);
                }
                break;
            case RewardType.Tile:
                var tiles = TileCtrl.current.GetAllTiles();
                UIHudSelectTile.current.Show(SelectTileType.Choise, tiles.RandN(3));
                break;
            default:
                Debug.LogError($"Unknown reward type");
                break;
        }
        OnClicked?.Invoke(this);

        Remove();
    }

    
}

public enum RewardType
{
    None,
    Gold,
    Tile,
}