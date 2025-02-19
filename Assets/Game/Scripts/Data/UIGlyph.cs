﻿using FancyToolkit;
using GridBoard;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class UIGlyph : MonoBehaviour, IHasInfo, IActionParent, IIconProvider, IInfoTextProvider, IHintContainer, IHintProvider, IHoverInfoTarget
{
    [SerializeField] Image imgIcon;

    public GlyphData data { get; private set; }

    List<ActionBase> actions = new();
    int power;


    public int Power
    {
        get => Mathf.Max(power, 0);
        set
        {
            if (power == value) return;
            power = value;
            //RefreshNumber();
        }
    }

    public Board board { get; private set; }

    public string VfxId => "Spell"; //ToDo: find good match
    public Sprite GetIcon() => data.GetIcon();
    public List<string> GetTooltips() => data.GetTooltips();
    public bool ShouldShowInfo() => data.ShouldShowInfo();
    public string GetTitle() => data.GetTitle();
    public string GetDescription() => data.GetDescription(actions);

    public Component AsComponent() => this;

    public List<string> GetTags() => data.GetTags();

    public void Init(GlyphData data, Board board)
    {
        this.data = data;
        this.board = board;
        this.power = data.power;
        imgIcon.sprite = data.sprite;

        if (board)
        {
            actions = new();
            foreach (var item in data.actions)
            {
                var action = item.Build();
                action.Init(this);
                actions.Add(action);
            }
        }
    }

    public IEnumerator CombatStarted()
    {
        foreach (var item in actions)
        {
            yield return item.Run();
        }
    }

    public IHasInfo GetExtraInfo()
    {
        return data.GetExtraInfo(actions);
    }

    public string GetInfoText(int size)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{data.name} Glyph"
            .Center()
            .Bold());

        sb.AppendLine();
        sb.AppendLine(GetDescription());

        return sb.ToString();
    }

    public List<IHintProvider> GetHintProviders()
    {
        var results = new List<IHintProvider>();

        foreach (var item in actions)
        {
            foreach (var hint in item.GetHints())
            {
                results.Add(hint);
            }
        }

        return results;
    }

    public string GetHintText()
    {
        var sb = new StringBuilder();

        sb.Append(data.name
            .Center()
            .Bold());

        sb.Append(" - ");
        sb.Append(GetDescription());

        return sb.ToString();
    }

    public bool ShouldShowHoverInfo() => data.ShouldShowHoverInfo();
}

public class GlyphData : DataWithId, IHasInfo, IActionParent, IIconProvider, IInfoTextProvider, IHintContainer, IHintProvider, IHoverInfoTarget
{
    public string idVisual;
    public Sprite sprite;
    public string name;
    public Rarity rarity;
    public int power;
    public List<FactoryBuilder<ActionBase>> actions;

    public int Power { get => power; set { } }

    public Transform transform => null;
    public Board board => null;
    public Component AsComponent() => null;

    public string VfxId => null;


    public string GetDescription(List<ActionBase> actions)
    {
        if (actions == null || actions.Count == 0) return "";

        return string.Join(". ", actions.Select(x=>x.GetDescription()));
    } 

    public string GetDescription()
    {
        var actions = this.actions
                .Select(x => x.Build())
                .ToList();

        foreach (var item in actions) item.Init(this);

        return GetDescription(actions);
    }

    public Sprite GetIcon() => sprite;

    public List<string> GetTags() => new();

    public string GetTitle() => $"{name} Glyph";

    public List<string> GetTooltips() => new();

    public bool ShouldShowInfo() => true;

    public IHasInfo GetExtraInfo(List<ActionBase> actions)
    {
        foreach (var item in actions)
        {
            var extra = item.GetExtraInfo();
            if (extra != null) return extra;
        }

        return null;
    }

    public IHasInfo GetExtraInfo()
    {
        var actions = this.actions
                .Select(x => x.Build())
                .ToList();

        foreach (var item in actions) item.Init(this);

        return GetExtraInfo(actions);
    }


    public string GetInfoText(int size)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{name} Glyph"
            .Center()
            .Bold());

        sb.AppendLine();
        sb.AppendLine(GetDescription());

        return sb.ToString();
    }

    public List<IHintProvider> GetHintProviders()
    {
        var results = new List<IHintProvider>();
        var actions = this.actions
                .Select(x => x.Build())
                .ToList();

        foreach (var item in actions)
        {
            foreach (var hint in item.GetHints())
            {
                results.Add(hint);
            }
        }

        return results;
    }

    public string GetHintText()
    {
        var sb = new StringBuilder();

        sb.Append(name
            .Center()
            .Bold());

        sb.Append(" - ");
        sb.Append(GetDescription());

        return sb.ToString();
    }

    public bool ShouldShowHoverInfo() => true;
}


public class GlyphCtrl : GenericDataCtrl<GlyphData>
{
    static GlyphCtrl _current;
    public static GlyphCtrl current
    {
        get
        {
            if (_current == null)
            {
                var cur = new GlyphCtrl();
                _current = cur;
            }

            return _current;
        }
    }

    public override void PostInitSingle(GlyphData data)
    {
        if (data.idVisual != null)
        {
            var visual = Resources.Load<Sprite>($"GlyphIcons/{data.idVisual}");
            if (!visual)
            {
                Debug.LogError($"No sprite for {data.id} ({data.idVisual})");
                return;
            }

            data.sprite = visual;
        }
    }

    public void DebugAll()
    {
        var sb = new System.Text.StringBuilder();
        foreach (var data in GetAll())
        {
            sb.AppendLine($"{data.name} : {data.GetDescription()}");
        }
        Debug.Log(sb.ToString());
    }
}