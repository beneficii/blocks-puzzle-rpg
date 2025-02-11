using GridBoard;
using System.Collections;
using System.Linq;
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
    [SerializeField] UIGenericButton btnSkip;

    List<UISelectTileCard> cards = new();

    int GetPrice(Rarity rarity, System.Random rng)
    {
        switch (rarity)
        {
            case Rarity.Common: return rng.Next(20, 40);
            case Rarity.Uncommon: return rng.Next(60, 100);
            case Rarity.Rare: return rng.Next(130, 180);
            case Rarity.Legendary: return rng.Next(300, 400);
            default: return Random.Range(45, 60);
        }
    }

    public UIHudSelectTile ShowShop(List<MyTileData> list, System.Random rng)
    {
        Opened();
        Clear();
        foreach (var data in list)
        {
            var instance = UIUtils.CreateFromTemplate(templateCard);
            instance.Init(SelectTileType.Shop, data, GetPrice(data.rarity, rng));
            cards.Add(instance);
        }
        bg.SetActive(false);
        btnSkip.gameObject.SetActive(true);
        return this;
    }

    public UIHudSelectTile ShowChoise(List<MyTileData> list) => ShowChoise(list.Cast<IInfoTextProvider>().ToList());
    public UIHudSelectTile ShowChoise(List<SkillData> list) => ShowChoise(list.Cast<IInfoTextProvider>().ToList());
    public UIHudSelectTile ShowChoise(List<IInfoTextProvider> list)
    {
        Opened();
        Clear();
        foreach (var data in list)
        {
            var instance = UIUtils.CreateFromTemplate(templateCard);
            instance.Init(SelectTileType.Choise, data);
            cards.Add(instance);
        }
        bg.SetActive(false);
        btnSkip.gameObject.SetActive(true);
        return this;
    }

    public UIHudSelectTile SetCanSkip(bool value)
    {
        btnSkip.gameObject.SetActive(value);
        return this;
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
}

public enum SelectTileType
{
    None,
    Choise,
    Shop
}