using FancyToolkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;
using UnityEngine.UI;

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

    [SerializeField] Panel uiMainText;


    Dictionary<string, List<DialogData>> dict;
    string nextDialogID;
    StageType nextStageType;

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

    public void MainInitDone()
    {
        SetNext(StageType.None);
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
        var mainText = list[0];
        //CombatArena.current.enemy.SetDialog(mainText.text, mainText.IsNarration());
        //TutorialCtrl.current.ShowText(TutorialPanel.Board, mainText.text, !mainText.IsNarration());
        uiMainText.Show(mainText.text, !mainText.IsNarration());
        StartCoroutine(uiMainText.BackupRefresh());

        if (list.Count == 1)
        {
            templateOption.Create<UIDIalogOption>()
                .Init(DialogData.OptContinue);
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

    public void ShowCustom(string message)
    {
        nextDialogID = null;
        Clear();

        Assert.IsNotNull(CombatArena.current.enemy);
        //CombatArena.current.enemy.SetDialog(message);
        uiMainText.Show(message, true);
        StartCoroutine(uiMainText.BackupRefresh());

        //TutorialCtrl.current.ShowText(TutorialPanel.Board, message);

        // ToDo: maybe custom options
        templateOption.Create<UIDIalogOption>()
                .Init(DialogData.OptContinue);

        Opened();
    }

    public void SetNext(string id = null)
    {
        nextDialogID = id;
    }

    public void SetNext(StageType type)
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


        //if (CombatArena.current.enemy) CombatArena.current.enemy.SetDialog(null);
        //TutorialCtrl.current.HideAll();
        uiMainText.Hide();
        if (nextStageType != StageType.None && nextStageType != StageType.Dialog)
        {
            CombatCtrl.current.AddState(new CombatStates.InitStageType(nextStageType));
            SetNext(StageType.None);
        }
        Clear();
        Closed();
    }

    void SpecialStagePreparation(string dialogId)
    {
        if (dialogId == "tutorial_1")
        {
            objSmallBoard.SetActive(true);
        }


        if (dialogId == "tutorial_3" || dialogId == "tutorial_4")
        {
            objSmallBoard.SetActive(false);
        }
    }


    [System.Serializable]
    public class Panel
    {
        public RectTransform parent;
        public TMP_Text txt;
        public GameObject speechCorner;

        public void Show(string message, bool isSpeech = false)
        {
            parent.gameObject.SetActive(true);
            txt.text = message;
            speechCorner.SetActive(isSpeech);
            LayoutRebuilder.ForceRebuildLayoutImmediate(parent);
        }

        public IEnumerator BackupRefresh()
        {
            yield return null;
            LayoutRebuilder.ForceRebuildLayoutImmediate(parent);
        }

        public void Hide()
        {
            parent.gameObject.SetActive(false);
        }
    }
}

[System.Serializable]
public class DialogData
{
    // empty Id means it's answer option
    public string id;
    public string text;
    public string tag;
    public List<string> actions = new();

    public DialogData() { }
    public DialogData(string text)
    {
        this.text = text;
    }

    public bool IsNarration() => tag.StartsWith("N");

    public static DialogData OptContinue = new ("Continue");
}