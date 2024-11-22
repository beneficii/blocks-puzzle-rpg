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

    List<UISelectTileCard> cards = new();

    bool backToCombatAfterClose = true;

    public void SetHasParentWindow()
    {
        backToCombatAfterClose = false;
    }

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

    public void ShowShop(List<MyTileData> list, System.Random rng)
    {
        backToCombatAfterClose = true;
        Opened();
        Clear();
        foreach (var data in list)
        {
            var instance = UIUtils.CreateFromTemplate(templateCard);
            instance.Init(SelectTileType.Shop, data, GetPrice(data.rarity, rng));
            cards.Add(instance);
        }
        bg.SetActive(false);
    }

    public UIHudSelectTile ShowChoise(List<MyTileData> list) => ShowChoise(list.Cast<IHasInfo>().ToList());
    public UIHudSelectTile ShowChoise(List<SkillData> list) => ShowChoise(list.Cast<IHasInfo>().ToList());
    public UIHudSelectTile ShowChoise(List<IHasInfo> list)
    {
        backToCombatAfterClose = true;
        Opened();
        Clear();
        foreach (var data in list)
        {
            var instance = UIUtils.CreateFromTemplate(templateCard);
            instance.Init(SelectTileType.Choise, data);
            cards.Add(instance);
        }
        bg.SetActive(true);
        return this;
    }

    private void OnEnable()
    {
        UISelectTileCard.OnSelectCard += HandleCardSelected;
    }

    private void OnDisable()
    {
        UISelectTileCard.OnSelectCard -= HandleCardSelected;
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
        if (backToCombatAfterClose)
        {
            CombatCtrl.current.CheckQueue();
        }
    }

    void HandleCardSelected(UISelectTileCard card, SelectTileType type)
    {
        if (type == SelectTileType.Choise)
        {
            Close();
        }
    }
}

public enum SelectTileType
{
    None,
    Choise,
    Shop
}