﻿using FancyToolkit;
using GridBoard;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SkillData : DataWithId, IHasInfo
{
    public string idVisual;
    public string name;
    public Sprite sprite;

    public FactoryBuilder<SkillClickCondition> clickCondition;
    public FactoryBuilder<SkillActionBase> onClick;

    public FactoryBuilder<SkillActionBase> onEndTurn;
    public FactoryBuilder<SkillActionBase> onStartCombat;

    public string GetDescription(SkillActionContainer container)
    {
        var lines = new List<string>();

        var pairs = new List<System.Tuple<SkillActionBase, string>>
        {
            new(container.onStartCombat, "Start of combat"),
            new(container.onEndTurn, "Turn end"),
            new(container.onClick, "Use"),
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

    public string GetDescription() => GetDescription(new SkillActionContainer(this));

    public List<string> GetTooltips()
    {
        // Return an empty list for now; UISkillButton can add more context if needed
        return new List<string>();
    }
}

public class SkillActionContainer
{
    public SkillClickCondition clickCondition;
    public SkillActionBase onClick;
    public SkillActionBase onEndTurn;
    public SkillActionBase onStartCombat;

    public IEnumerable<SkillActionBase> AllActions()
    {
        if (onClick != null) yield return onClick;
        if (onEndTurn != null) yield return onEndTurn;
        if (onStartCombat != null) yield return onStartCombat;
    }

    public SkillActionContainer(SkillData data)
    {
        onClick = data.onClick?.Build();
        onEndTurn = data.onEndTurn?.Build();
        onStartCombat = data.onStartCombat?.Build();

        clickCondition = data.clickCondition?.Build();
    }
}