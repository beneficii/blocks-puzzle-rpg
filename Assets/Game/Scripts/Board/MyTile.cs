using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using TMPro;
using DG.Tweening;
using FancyTweens;
using FancyToolkit;

public class MyTile : Tile, IActionParent
{
    public const string keyDamage = "damage";
    public const string keyArmor = "armor";

    public MyTileData myData => data as MyTileData;

    [SerializeField] TextMeshPro txtPower;

    public TileActionContainer actionContainer { get; private set; }

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

    public string VfxId => myData?.VfxId;

    public ActionStatType StatType => actionContainer?.clearAction?.StatType ?? ActionStatType.None;

    public override string GetDescription()
    {
        var lines = new List<string>();
        if (!string.IsNullOrWhiteSpace(data.description))
        {
            lines.Add(data.description);
        }

        if (actionContainer != null)
        {
            lines.AddRange(actionContainer.GetDescriptions());
        }

        return string.Join(". ", lines);
    }

    public override IHasInfo GetExtraInfo()
    {
        return myData?.GetExtraInfo(actionContainer);
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

        actionContainer = new TileActionContainer(myData, this);

        if (board)
        {
            foreach (var item in actionContainer.AllActions())
            {
                item.SetBoard(board);
            }
        }

        RefreshNumber(true);
    }

    public override IEnumerator Place()
    {
        if (!isActionLocked && actionContainer?.enterAction != null)
        {
            yield return actionContainer.enterAction.Run();
        }
        yield return base.Place();
    }

    protected override void Clean()
    {
        if (actionContainer == null) return;
        foreach (var item in actionContainer.AllActions())
        {
            item.SetBoard(null);
        }

        actionContainer = null;

        txtPower.text = $"";

        base.Clean();
    }

    public IEnumerator OnCleared(LineClearData clearInfo)
    {
        if (!isActionLocked && actionContainer?.clearAction != null)
        {
            yield return actionContainer.clearAction.Run();
            yield return FadeOut(10);
        }
    }

    public IEnumerator EndOfTurn()
    {
        if (!isActionLocked && actionContainer?.endOfTurnAction != null)
        {
            yield return actionContainer.endOfTurnAction.Run();
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

    public static IEnumerable<MyTile> FindTileTargets(IActionParent parent, TileTargetingType targetType, System.Predicate<MyTile> filter = null)
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

    public static IEnumerable<MyTile> FindTileTargets(IActionParent parent, TileTargetingType targetType, string tag)
        => FindTileTargets(parent, targetType, (x) => x.HasTag(tag));

    public Component AsComponent() => this;

    public override string GetInfoText(int size)
    {
        var sb = new StringBuilder();

        sb.AppendLine(data.title
            //.Size(size*3/2)
            .Center()
            .Bold());

        sb.AppendLine();
        sb.AppendLine(GetDescription());

        var tags = GetTags();
        if (tags.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine(string.Join(", ", tags)
                .Alpha(150));
        }

        return sb.ToString();
    }

    public override List<IHintProvider> GetHintProviders()
    {
        return actionContainer?.GetHintProviders();
    }

    public override bool ShouldShowHoverInfo() => data?.ShouldShowHoverInfo()??false;
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