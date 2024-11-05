using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using TMPro;
using DG.Tweening;
using FancyTweens;

public class MyTile : Tile
{
    public MyTileData myData => data as MyTileData;

    [SerializeField] TextMeshPro txtPower;

    TileActionBase clearAction;
    TileActionBase endOfTurnAction;
    TileActionBase enterAction;
    TileActionBase passiveEffect;

    int power;
    public int Power
    {
        get => Mathf.Max(power, 0);
        set
        {
            if (power == value) return;
            power = value;
            RefreshNumber();
        }
    }

    public TileStatType StatType => clearAction?.StatType ?? TileStatType.None;

    IEnumerable<TileActionBase> AllActions()
    {
        if (clearAction != null) yield return clearAction;
        if (endOfTurnAction != null) yield return endOfTurnAction;
        if (enterAction != null) yield return enterAction;
        if (passiveEffect != null) yield return passiveEffect;
    }


    public override string GetDescription()
    {
        if (myData == null) return "";

        var baseDescription = data.description;
        if (!string.IsNullOrWhiteSpace(baseDescription))
        {
            return baseDescription;
        }

        var lines = new List<string>();

        if (!string.IsNullOrWhiteSpace(data.description))
        {
            lines.Add(data.description);
        }

        var buyAction = myData.buyAction?.Build();
        buyAction?.Init(this);

        var pairs = new List<System.Tuple<TileActionBase, string>>
        {
            new(enterAction, "Enter"),
            new(clearAction, "Clear"),
            new(endOfTurnAction, "EndOfTurn"),
            new(passiveEffect, ""),
            new(buyAction, ""),
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

        return string.Join(". ", lines);
    }

    void RefreshNumber(bool skipAnimation = false)
    {
        if (StatType == TileStatType.None)
        {
            txtPower.text = "";
            return;
        }

        txtPower.text = $"{Power}";
        if (!skipAnimation) txtPower.GetComponent<TweenTarget>().ScaleUpDown(.3f);
    }

    public override void InitVirtual(TileData data)
    {
        var myData = this.myData;
        if (myData == null) return;

        this.power = myData.power;

        RefreshNumber(true);

        clearAction = myData.clearAction?.Build();
        endOfTurnAction = myData.endTurnAction?.Build();
        enterAction = myData.enterAction?.Build();
        passiveEffect = myData.passive?.Build();
        foreach (var item in AllActions())
        {
            item.Init(this);
        }

        if (board)
        {
            foreach (var item in AllActions())
            {
                item.SetOnBoard(true);
            }
        }
    }

    public override IEnumerator Place()
    {
        if (!isActionLocked && enterAction != null)
        {
            yield return enterAction.Run();
        }
        yield return base.Place();
    }

    protected override void Clean()
    {
        foreach (var item in AllActions())
        {
            item.SetOnBoard(false);
        }

        clearAction = null;
        endOfTurnAction = null;
        enterAction = null;
        passiveEffect = null;

        txtPower.text = $"";

        base.Clean();
    }

    public IEnumerator OnCleared(LineClearData clearInfo)
    {
        if (!isActionLocked && clearAction != null)
        {
            yield return clearAction.Run(clearInfo);
        }
    }

    public IEnumerator EndOfTurn()
    {
        if (!isActionLocked && endOfTurnAction != null)
        {
            yield return endOfTurnAction.Run();
        }
    }

}


public enum TileStatType
{
    None,
    Damage,
    Defense,
    Power,  // other
}
