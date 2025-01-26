using FancyToolkit;
using GridBoard;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SkillData : DataWithId, IHasInfo, IActionParent
{
    public string idVisual;
    public string name;
    public Sprite sprite;
    public int power;
    public Rarity rarity;

    public FactoryBuilder<SkillClickCondition> clickCondition;
    public FactoryBuilder<ActionBase> onClick;

    public FactoryBuilder<ActionBase> onEndTurn;
    public FactoryBuilder<ActionBase> onStartCombat;

    public int Power { get => power; set { } }

    public Transform transform => null;
    public Board board => null;
    public Component AsComponent() => null;

    public string VfxId => null;

    public string GetDescription(SkillActionContainer container)
    {
        var lines = new List<string>();

        bool autoActivate = container.clickCondition?.AutoActivate ?? false;
        var pairs = new List<System.Tuple<ActionBase, string>>
        {
            new(container.onStartCombat, "Start of combat"),
            new(container.onEndTurn, "Turn end"),
            new(container.onClick, autoActivate?"Activate":"Use"),
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

        if (container.clickCondition != null)
        {
            var descr = container.clickCondition.GetDescription();
            if (!string.IsNullOrWhiteSpace(descr))
            {
                lines.Add($"{descr}");
            }
        }

        return string.Join(". ", lines);
    }

    public Sprite GetIcon()
    {
        return sprite;
    }

    public bool ShouldShowInfo()
    {
        return true;
    }

    public string GetTitle()
    {
        return name;
    }

    public string GetDescription() => GetDescription(new SkillActionContainer(this, this));

    public List<string> GetTooltips()
    {
        // Return an empty list for now; UISkillButton can add more context if needed
        return new List<string>();
    }

    public List<string> GetTags() => new();

    public IHasInfo GetExtraInfo(SkillActionContainer container)
    {
        return container.onClick?.GetExtraInfo();
    }

    public IHasInfo GetExtraInfo() => GetExtraInfo(new SkillActionContainer(this, this));
}

public class SkillActionContainer
{
    public SkillClickCondition clickCondition;
    public ActionBase onClick;
    public ActionBase onEndTurn;
    public ActionBase onStartCombat;

    public IEnumerable<ActionBase> AllActions()
    {
        if (onClick != null) yield return onClick;
        if (onEndTurn != null) yield return onEndTurn;
        if (onStartCombat != null) yield return onStartCombat;
    }

    public SkillActionContainer(SkillData data, IActionParent parent)
    {
        onClick = data.onClick?.Build();
        onEndTurn = data.onEndTurn?.Build();
        onStartCombat = data.onStartCombat?.Build();

        clickCondition = data.clickCondition?.Build();

        foreach (var item in AllActions())
        {
            item.Init(parent);
        }
    }
}