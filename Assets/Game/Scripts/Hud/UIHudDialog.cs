﻿using FancyToolkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class UIHudDialog : UIHudBase
{
    public static UIHudDialog _current;
    public static UIHudDialog current
    {
        get
        {
            if (!_current)
            {
                _current = FindFirstObjectByType<UIHudDialog>();
                _current.Init();
            }
            
            return _current;
        }
    }

    [SerializeField] TextAsset tableDialogs;
    [SerializeField] UITemplateItem templateOption;

    Dictionary<string, List<DialogData>> dict;
    string nextDialogID;

    void Init()
    {
        dict = new();
        List<DialogData> sublist = new();
        foreach (var item in FancyCSV.FromText<DialogData>(tableDialogs.text))
        {
            if(!string.IsNullOrWhiteSpace(item.id))
            {
                sublist = new List<DialogData>();
                dict.Add(item.id, sublist);
            }
            sublist.Add(item);
        }
    }

    public void Show(string id)
    {
        nextDialogID = null;
        Clear();
        if (!dict.TryGetValue(id, out var list))
        {
            Debug.LogError($"Unknown dialog id: {id}");
            return;
        }

        Assert.IsTrue(list.Count > 0);
        Assert.IsNotNull(CombatArena.current.enemy);
        CombatArena.current.enemy.SetDialog(list[0].text);

        if (list.Count == 1)
        {
            templateOption.Create<UIDIalogOption>()
                .Init(DialogData.OptContinue);
            return;
        }

        for (int i = 1; i < list.Count; i++)
        {
            var data = list[i];
            templateOption.Create<UIDIalogOption>()
                .Init(data);
        }

        Opened();
    }

    public void SetNext(string id = null)
    {
        nextDialogID = id;
    }

    void Clear()
    {
        templateOption.Clear();
    }

    public void Close()
    {
        if (!string.IsNullOrWhiteSpace(nextDialogID))
        {
            Show(nextDialogID);
            return;
        }

        if (CombatArena.current.enemy) CombatArena.current.enemy.SetDialog(null);

        Clear();
        Closed();
    }

}

[System.Serializable]
public class DialogData
{
    // empty Id means it's answer option
    public string id;
    public string text;
    public List<string> actions = new();

    public DialogData() { }
    public DialogData(string text)
    {
        this.text = text;
    }

    public static DialogData OptContinue = new ("Continue");
}