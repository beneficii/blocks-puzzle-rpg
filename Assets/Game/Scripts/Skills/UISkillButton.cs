using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FancyToolkit;
using GridBoard;
using FancyTweens;
using System.Text;
using TileActions;

public class UISkillButton : MonoBehaviour, IHasInfo
{
    [SerializeField] Button button;
    [SerializeField] Image imgIcon;
    [SerializeField] Image imgBg;

    public SkillData data { get; private set; }

    SkillClickCondition clickCondition;
    SkillActionBase onClick;
    SkillActionBase onEndTurn;
    SkillActionBase onStartCombat;

    public Board board { get; private set; }

    IEnumerable<SkillActionBase> AllActions()
    {
        if (onClick != null) yield return onClick;
        if (onEndTurn != null) yield return onEndTurn;
        if (onStartCombat != null) yield return onStartCombat;
    }

    public void Init(SkillData data, Board board)
    {
        this.data = data;
        this.board = board;

        button.interactable = false;
        
        onClick = data.onClick?.Build();
        onEndTurn = data.onEndTurn?.Build();
        onStartCombat = data.onStartCombat?.Build();
        foreach (var item in AllActions())
        {
            item.Init(this);
        }

        clickCondition = data.clickCondition?.Build();
        clickCondition?.Init(this);

        if (board)
        {
            button.interactable = (clickCondition != null && onClick != null);
        }
    }

    public IEnumerator CombatStarted()
    {
        if (onStartCombat != null)
        {
            yield return onStartCombat.Run();
        }
    }

    public void OnClick()
    {
        if (clickCondition == null || onClick == null) return;
        if (!clickCondition.CanUse)
        {
            MainUI.current.ShowMessage(clickCondition.GetErrorUnusable());
            return;
        }

        clickCondition.OnClicked();
        StartCoroutine(onClick.Run());
    }

    public IEnumerator EndTurn()
    {
        if (onEndTurn != null)
        {
            yield return onEndTurn.Run();
        }
    }

    public void RefreshUse()
    {
        if (clickCondition == null) return;
        bool canUse = clickCondition.CanUse;

        imgIcon.SetAlpha(canUse ? 1 : 0.2f);
    }

    public Sprite GetIcon() => data.sprite;
    public List<string> GetTooltips() => new();
    public bool ShouldShowInfo() => true;
    public string GetTitle() => data.name;

    public string GetDescription()
    {
        var lines = new List<string>();

        var pairs = new List<System.Tuple<SkillActionBase, string>>
        {
            new(onStartCombat, "Start of combat"),
            new(onEndTurn, "Turn end"),
            new(onClick, "Use"),
        };

        foreach (var item in pairs)
        {
            var descr = item.Item1?.GetDescription();
            if (string.IsNullOrWhiteSpace(descr)) continue;

            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(item.Item2)) sb.Append($"<b>{item.Item2}: </b>");

            sb.Append(descr);
            lines.Add(sb.ToString());
        }

        if (clickCondition != null)
        {
            var descr = clickCondition.GetDescription();
            if (!string.IsNullOrWhiteSpace(descr))
            {
                lines.Add($"{descr}");
            }
        }

        return string.Join(". ", lines);
    }

}