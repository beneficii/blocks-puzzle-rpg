using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class UIHudRewards : UIHudBase
{
    public static UIHudRewards _current;
    public static UIHudRewards current
    {
        get
        {
            if (!_current) _current = FindFirstObjectByType<UIHudRewards>();
            return _current;
        }
    }

    [SerializeField] UICombatReward templateItem;

    List<UICombatReward> instantiatedItems = new();

    void Clear()
    {
        foreach (var item in instantiatedItems)
        {
            if (!item) continue;
            Destroy(item.gameObject);
        }

        instantiatedItems.Clear();
    }

    public void Show(List<string> rewards)
    {
        Opened();
        Clear();
        foreach (var item in rewards)
        {
            var instance = UIUtils.CreateFromTemplate(templateItem);
            instance.Init(item);
            instantiatedItems.Add(instance);
        }
    }

    private void OnEnable()
    {
        UICombatReward.OnClicked += HandleRewardClicked;
    }

    private void OnDisable()
    {
        UICombatReward.OnClicked -= HandleRewardClicked;
    }

    void HandleRewardClicked(UICombatReward item)
    {
        if (item.id == RewardType.Tile)
        {
            content.SetActive(false);
        }
    }

    public void Close()
    {
        Closed();
    }

    public void Skip()
    {
        foreach (var item in instantiatedItems)
        {
            if (item)
            {
                Game.ToDo("Some warning about unclaimed rewards");
                return;
            }
        }

        Close();
    }
}
