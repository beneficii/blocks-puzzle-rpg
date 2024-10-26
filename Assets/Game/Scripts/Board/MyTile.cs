using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using TMPro;

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

        var mainDescription = data.GetDescription();
        if (!string.IsNullOrWhiteSpace(mainDescription))
        {
            return mainDescription;
        }

        var lines = new List<string>();

        if (!string.IsNullOrWhiteSpace(data.description))
        {
            lines.Add(data.description);
        }

        var pairs = new List<System.Tuple<TileActionBase, string>>
        {
            new(enterAction, "Enter"),
            new(clearAction, "Clear"),
            new(endOfTurnAction, "EndOfTurn"),
            new(passiveEffect, ""),
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
        if (myData.powerType == TileStatType.None)
        {
            txtPower.text = "";
            return;
        }

        txtPower.text = $"{Power}";
        if (!skipAnimation) StartCoroutine(AnimateNumber());
    }

    bool animatingNumber;
    IEnumerator AnimateNumber()
    {
        if (animatingNumber) yield break;
        animatingNumber = true;
        var ls = txtPower.transform.localScale;
        txtPower.transform.localScale *= 1.2f;
        yield return new WaitForSeconds(0.25f);
        txtPower.transform.localScale = ls;
        animatingNumber = false;
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
