using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class CombatRewardsPanel : UIHudBase
{
    public static CombatRewardsPanel _current;
    public static CombatRewardsPanel current
    {
        get
        {
            if (!_current) _current = FindFirstObjectByType<CombatRewardsPanel>();
            return _current;
        }
    }

    public static event System.Action OnClosed;
    public static event System.Action OnOpen;

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
        OnClosed?.Invoke();
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
