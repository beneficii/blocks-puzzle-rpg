﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class CombatRewardsPanel : MonoBehaviour
{
    public event System.Action OnClosed;

    [SerializeField] UICombatReward templateItem;
    [SerializeField] GameObject content;

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
        Clear();
        gameObject.SetActive(true);
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
        SelectTileScreen.OnClosed += HandleTileChoiseDone;
    }

    private void OnDisable()
    {
        UICombatReward.OnClicked -= HandleRewardClicked;
        SelectTileScreen.OnClosed -= HandleTileChoiseDone;
    }

    void HandleRewardClicked(UICombatReward item)
    {
        if (item.id == RewardType.Tile)
        {
            content.SetActive(false);
        }
    }

    void HandleTileChoiseDone()
    {
        content.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        OnClosed?.Invoke();
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