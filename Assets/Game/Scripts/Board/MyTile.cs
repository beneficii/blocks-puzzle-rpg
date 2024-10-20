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
        get => power;
        set
        {
            if (power == value) return;
            power = value;
            RefreshNumber();
        }
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
            var descr = item.Item1?.GetDescription(this);
            if (string.IsNullOrWhiteSpace(descr)) continue;

            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(item.Item2)) sb.Append($"<b>{item.Item2}: </b>");

            sb.Append(descr);
            lines.Add(sb.ToString());
        }

        return string.Join(". ", lines);
    }

    public override void InitBoard(Board board)
    {
        base.InitBoard(board);

        var myData = this.myData;
        if (myData == null) return;


        clearAction = myData.clearAction?.Build();
        endOfTurnAction = myData.endTurnAction?.Build();
        enterAction = myData.enterAction?.Build();
        passiveEffect = myData.passive?.Build();
        
        clearAction?.Init(this);
        endOfTurnAction?.Init(this);
        enterAction?.Init(this);
        passiveEffect?.Init(this);
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

    public override void Init(TileData data, int level = -1)
    {
        base.Init(data, level);

        var myData = this.myData;
        if (myData == null) return;

        this.power = myData.power;

        RefreshNumber(true);
    }

    public override IEnumerator OnPlaced()
    {
        if (enterAction != null)
        {
            yield return enterAction.Run();
        }
    }

    public override void OnRemoved()
    {
        base.OnRemoved();
        if (board != null)
        {
            clearAction?.Remove();
            endOfTurnAction?.Remove();
            passiveEffect?.Remove();
        }
    }

    public IEnumerator OnCleared(LineClearData clearInfo)
    {
        if (clearAction != null)
        {
            yield return clearAction.Run(clearInfo);
        }
    }

    public IEnumerator EndOfTurn()
    {
        if (endOfTurnAction != null)
        {
            yield return endOfTurnAction.Run();
        }
    }


    /*
    protected int DamageAction(Unit src, Unit target, int damage, LineClearData clearData = null)
    {
        if (tile.HasTag("sword") && src.GetModifier(Unit.Modifier.SwordAttack, out var swordAttk))
        {
            damage = Mathf.Max(0, damage + swordAttk);
        }

        if (clearData != null)
        {
            clearData.valTotalDamage += damage;
        }

        return damage;
    }

    protected int ArmorAction(Unit src, MyTile tile, int damage, LineClearData clearData = null)
    {
        if (tile.HasTag("sword") && src.GetModifier(Unit.Modifier.SwordAttack, out var swordAttk))
        {
            damage = Mathf.Max(0, damage + swordAttk);
        }

        if (clearData != null)
        {
            clearData.valTotalDamage += damage;
        }

        return damage;
    }*/

}


public enum TileStatType
{
    None,
    Damage,
    Defense,
}
