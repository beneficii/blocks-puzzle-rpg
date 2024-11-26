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
    [SerializeField] GameObject warningSkip;

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

    public void Show(List<UICombatReward.Data> rewards)
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

    private void OnDisable()
    {
        warningSkip.gameObject.SetActive(false);
    }

    public void Close()
    {
        Closed();
    }

    public void Skip()
    {
        if (!warningSkip.gameObject.activeSelf)
        {
            foreach (var item in instantiatedItems)
            {
                if (!item) continue;

                warningSkip.gameObject.SetActive(true);
                return;
            }
        }

        Close();
    }
}
