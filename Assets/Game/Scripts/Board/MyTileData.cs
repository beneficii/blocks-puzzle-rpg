using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using FancyToolkit;
using System.Text;

public class MyTileData : TileData, IActionParent
{
    public int power;

    public FactoryBuilder<ActionBase> clearAction;
    public FactoryBuilder<ActionBase> endTurnAction;
    public FactoryBuilder<ActionBase> enterAction;
    public FactoryBuilder<ActionBase> passive;
    public FactoryBuilder<ActionBase> buyAction;

    public int Damage { get => power; set { } }
    public int Defense { get => power; set { } }

    public Transform transform => null;
    public Board board => null;
    public Component AsComponent() => null;

    public string GetDescription(TileActionContainer actionContainer)
    {
        var lines = new List<string>();
        if (!string.IsNullOrWhiteSpace(description))
        {
            lines.Add(description);
        }

        if (actionContainer != null)
        {
            lines.AddRange(actionContainer.GetDescriptions());
        }

        return string.Join(". ", lines);
    }

    public override string GetDescription()
        => GetDescription(new(this, this));
}

public class TileActionContainer
{
    public ActionBase clearAction;
    public ActionBase endOfTurnAction;
    public ActionBase enterAction;
    public ActionBase passiveEffect;
    public ActionBase buyAction;

    public TileActionContainer(MyTileData data, IActionParent parent)
    {
        clearAction = data.clearAction?.Build();
        endOfTurnAction = data.endTurnAction?.Build();
        enterAction = data.enterAction?.Build();
        passiveEffect = data.passive?.Build();
        foreach (var item in AllActions())
        {
            item.Init(parent);
        }
    }

    public IEnumerable<ActionBase> AllActions()
    {
        if (clearAction != null) yield return clearAction;
        if (endOfTurnAction != null) yield return endOfTurnAction;
        if (enterAction != null) yield return enterAction;
        if (passiveEffect != null) yield return passiveEffect;
    }

    public List<string> GetDescriptions()
    {
        var lines = new List<string>();

        var pairs = new List<System.Tuple<ActionBase, string>>
        {
            new(enterAction, "Enter"),
            new(clearAction, "Clear"),
            new(endOfTurnAction, "Turn end"),
            new(passiveEffect, ""),
            new(buyAction, ""),
        };

        foreach (var item in pairs)
        {
            var action = item.Item1;
            if (action == null) continue;
            var descr = action.GetDescription();
            if (string.IsNullOrWhiteSpace(descr)) continue;

            var sb = new StringBuilder();
            if (!action.OverrideDescriptionKey && !string.IsNullOrEmpty(item.Item2))
            {
                sb.Append($"<b>{item.Item2}: </b>");
            }

            sb.Append(descr);
            lines.Add(sb.ToString());
        }

        return lines;
    }
}