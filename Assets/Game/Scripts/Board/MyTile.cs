using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using TMPro;
using DG.Tweening;
using FancyTweens;
using FancyToolkit;
using TileActions;

public class MyTile : Tile, IActionParent
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

    public ActionStatType StatType => clearAction?.StatType ?? ActionStatType.None;

    public int Damage { get => Power; set => Power = value; }
    public int Defense { get => Power; set => Power = value; }

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
            new(endOfTurnAction, "Turn end"),
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
        if (StatType == ActionStatType.None)
        {
            txtPower.text = "";
            return;
        }

        txtPower.text = $"{Power}";
        if (!skipAnimation) txtPower.GetComponent<TweenTarget>().ScaleUpDown(.3f);
    }

    public override IEnumerator FadeOut(float fadeSpeed)
    {
        txtPower.gameObject.SetActive(false);
        return base.FadeOut(fadeSpeed);
    }

    public override void InitVirtual(TileData data)
    {
        var myData = this.myData;
        if (myData == null) return;

        this.power = myData.power;


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

        RefreshNumber(true);
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

    public static string GetTargetingTypeName(TileTargetingType targetType, string tag = TileData.anyTag)
    {
        if (tag == TileData.anyTag)
        {
            return targetType switch
            {
                TileTargetingType.Self => "this tile",
                TileTargetingType.Around => "surrounding tiles",
                TileTargetingType.Biggest => "strongest tile",
                TileTargetingType.Closest => "closest tile",
                TileTargetingType.Random => $"random tile",
                TileTargetingType.All => $"all tiles",
                _ => "unknown",
            };
        }
        else
        {
            return targetType switch
            {
                TileTargetingType.Self => "this tile",
                TileTargetingType.Around => $"surrounding {tag} tiles",
                TileTargetingType.Biggest => $"strongest {tag} tile",
                TileTargetingType.Closest => $"closest {tag} tile",
                TileTargetingType.Random => $"random {tag} tile",
                TileTargetingType.All => $"all {tag} tiles",
                _ => "unknown",
            };
        }
    }

    protected static IEnumerable<MyTile> FindTileTargets(IActionParent parent, TileTargetingType targetType, System.Predicate<MyTile> filter = null)
    {
        var parentTile = parent as MyTile;

        if (targetType == TileTargetingType.Self)
        {
            yield return parentTile;
            yield break;
        }

        if (targetType == TileTargetingType.Around)
        {
            if (!parentTile)
            {
                Debug.LogError("parent not tile!");
                yield break;
            }
            foreach (var item in parent.board.GetTilesAround(parentTile.position.x, parentTile.position.y))
            {
                if (item is not MyTile tile || tile.isBeingPlaced) continue;
                if (filter != null && !filter(tile)) continue;

                yield return tile;

            }
            yield break;
        }

        if (targetType == TileTargetingType.Biggest)
        {
            MyTile biggest = null;
            int maxPower = -1;
            foreach (var item in parent.board.GetAllTiles())
            {

                if (item is not MyTile tile || tile.isBeingPlaced || tile == parentTile) continue;
                if (tile.StatType == ActionStatType.None) continue;
                if (filter != null && !filter(tile)) continue;

                if (tile.Power > maxPower)
                {
                    biggest = tile;
                    maxPower = tile.Power;
                }


            }
            if (biggest) yield return biggest;
            yield break;
        }

        if (targetType == TileTargetingType.Closest)
        {
            MyTile closest = null;
            int minDistanceSqr = int.MaxValue;
            foreach (var item in parent.board.GetAllTiles())
            {
                if (!parentTile)
                {
                    Debug.LogError("parent not tile!");
                    yield break;
                }

                if (item is not MyTile tile || tile.isBeingPlaced || tile == parentTile) continue;
                if (filter != null && !filter(tile)) continue;


                int distanceSqr = VectorUtil.DistanceSqr(parentTile.position, tile.position);
                if (distanceSqr < minDistanceSqr)
                {
                    closest = tile;
                    minDistanceSqr = distanceSqr;
                }


            }
            if (closest) yield return closest;
            yield break;
        }

        if (targetType == TileTargetingType.Random)
        {
            var list = new List<MyTile>();
            foreach (var item in parent.board.GetAllTiles())
            {
                if (item is not MyTile tile || tile.isBeingPlaced || tile == parentTile) continue;
                if (filter != null && !filter(tile)) continue;

                list.Add(tile);

            }
            yield return list.Rand();
            yield break;
        }

        if (targetType == TileTargetingType.All)
        {
            foreach (var item in parent.board.GetAllTiles())
            {
                if (item is not MyTile tile || tile.isBeingPlaced || tile == parentTile) continue;
                if (filter != null && !filter(tile)) continue;

                yield return tile;


            }
            yield break;
        }
    }

    protected static IEnumerable<MyTile> FindTileTargets(IActionParent parent, TileTargetingType targetType, string tag)
        => FindTileTargets(parent, targetType, (x) => x.HasTag(tag));

}

public enum TileTargetingType
{
    None,
    Self,
    Around,
    Biggest, // most power
    Closest,
    Random,
    All,
}