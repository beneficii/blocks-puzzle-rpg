using FancyToolkit;
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

    [SerializeField] UITemplateItem templateOption;


    [SerializeField] GameObject objSmallBoard;

    Dictionary<string, List<DialogData>> dict;
    string nextDialogID;
    StageData.Type nextStageType;

    void Init()
    {
        dict = new();
        List<DialogData> sublist = new();
        foreach (var item in FancyCSV.FromCSV<DialogData>("Dialogs"))
        {
            if(!string.IsNullOrWhiteSpace(item.id))
            {
                sublist = new List<DialogData>();
                dict.Add(item.id, sublist);
            }
            sublist.Add(item);
        }

        SetNext(StageCtrl.current.Data.type);
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
        else for (int i = 1; i < list.Count; i++)
        {
            var data = list[i];
            templateOption.Create<UIDIalogOption>()
                .Init(data);
        }

        SpecialStagePreparation(id);
        Opened();
    }

    public void SetNext(string id = null)
    {
        nextDialogID = id;
    }

    public void SetNext(StageData.Type type)
    {
        nextStageType = type;
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
        if (nextStageType != StageData.Type.None && nextStageType != StageData.Type.Dialog)
        {
            CombatCtrl.current.AddState(new CombatStates.InitStageType(nextStageType));
        }
        Clear();
        Closed();
    }

    void SpecialStagePreparation(string dialogId)
    {
        if (dialogId == "tutorial")
        {
            objSmallBoard.SetActive(true);
        }

        if (dialogId == "tutorial_3" || dialogId == "tutorial_4")
        {
            objSmallBoard.SetActive(false);
        }
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