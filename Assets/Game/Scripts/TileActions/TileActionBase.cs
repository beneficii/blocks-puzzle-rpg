using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FancyToolkit;
using GridBoard;
using System.Collections;
using TileActions;
using UnityEditor.U2D.Aseprite;

public abstract class TileActionBase
{
    public bool isOnBoard;
    protected MyTile parent;

    protected int Power => parent.Power;

    public abstract string GetDescription();

    public abstract IEnumerator Run(int multiplier = 1);
    public virtual IEnumerator Run(LineClearData match)
    {
        yield return Run();
        yield return parent.FadeOut(10f);
    }

    protected GenericBullet MakeBullet(Tile parent)
    {
        var rand = Random.Range(0, 2) == 0;
        var bullet = DataManager.current.gameData.prefabBullet.MakeInstance(parent.transform.position)
            .SetSpleen(rand ? Vector2.left : Vector2.right)
            .SetSprite(parent.GetIcon());

        return bullet;
    }

    protected GenericBullet MakeBullet(Tile parent, Vector2 position)
    {
        var rand = Random.Range(0, 2) == 0;
        var bullet = DataManager.current.gameData.prefabBullet.MakeInstance(position)
            .SetSpleen(rand ? Vector2.left : Vector2.right)
            .SetSprite(parent.GetIcon());

        return bullet;
    }

    protected string GetTargetingTypeName(ActionTargetType targetType, string tag = TileData.anyTag)
    {
        if (tag == TileData.anyTag)
        {
            return targetType switch
            {
                ActionTargetType.Self => "this tile",
                ActionTargetType.Around => "surrounding tiles",
                ActionTargetType.Biggest => "strongest tile",
                ActionTargetType.Closest => "closest tile",
                ActionTargetType.Random => $"random tile",
                ActionTargetType.All => $"all tiles",
                _ => "unknown",
            };
        }
        else
        {
            return targetType switch
            {
                ActionTargetType.Self => "this tile",
                ActionTargetType.Around => $"surrounding {tag} tiles",
                ActionTargetType.Biggest => $"strongest {tag} tile",
                ActionTargetType.Closest => $"closest {tag} tile",
                ActionTargetType.Random => $"random {tag} tile",
                ActionTargetType.All => $"all {tag} tiles",
                _ => "unknown",
            };
        }
    }

    protected IEnumerable<MyTile> FindTileTargets(MyTile parent, ActionTargetType targetType, System.Predicate<MyTile> filter = null)
    {
        if (targetType == ActionTargetType.Self)
        {
            yield return parent;
            yield break;
        }

        if (targetType == ActionTargetType.Around)
        {
            foreach (var item in parent.board.GetTilesAround(parent.position.x, parent.position.y))
            {
                if (item is not MyTile tile || tile.isBeingPlaced) continue;
                if (filter != null && !filter(tile)) continue;
                
                yield return tile;

            }
            yield break;
        }

        if (targetType == ActionTargetType.Biggest)
        {
            MyTile biggest = null;
            int maxPower = -1;
            foreach (var item in parent.board.GetAllTiles())
            {

                if (item is not MyTile tile || tile.isBeingPlaced || tile == parent) continue;
                var data = tile.myData;
                if (data == null || data.powerType == TileStatType.None) continue;
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

        if (targetType == ActionTargetType.Closest)
        {
            MyTile closest = null;
            int minDistanceSqr = int.MaxValue;
            foreach (var item in parent.board.GetAllTiles())
            {

                if (item is not MyTile tile || tile.isBeingPlaced || tile == parent) continue;
                if (filter != null && !filter(tile)) continue;


                int distanceSqr = VectorUtil.DistanceSqr(parent.position, tile.position);
                if (distanceSqr < minDistanceSqr)
                {
                    closest = tile;
                    minDistanceSqr = distanceSqr;
                }


            }
            if (closest) yield return closest;
            yield break;
        }

        if (targetType == ActionTargetType.Random)
        {
            var list = new List<MyTile>();
            foreach (var item in parent.board.GetAllTiles())
            {
                if (item is not MyTile tile || tile.isBeingPlaced || tile == parent) continue;
                if (filter != null && !filter(tile)) continue;

                list.Add(tile);

            }
            yield return list.Rand();
            yield break;
        }

        if (targetType == ActionTargetType.All)
        {
            foreach (var item in parent.board.GetAllTiles())
            {
                if (item is not MyTile tile || tile.isBeingPlaced || tile == parent) continue;
                if (filter != null && !filter(tile)) continue;

                yield return tile;


            }
            yield break;
        }
    }

    protected IEnumerable<MyTile> FindTileTargets(MyTile parent, ActionTargetType targetType, string tag)
        => FindTileTargets(parent, targetType, (x) => x.HasTag(tag));


    public virtual void Init(MyTile tile)
    {
        this.parent = tile;
    }

    public void SetOnBoard(bool value)
    {
        if (value == isOnBoard) return;
        isOnBoard = value;
        if (value)
        {
            Add();
        }
        else
        {
            Remove();
        }
    }

    protected virtual void Add()
    {

    }

    protected virtual void Remove()
    {
    }
}

namespace TileActions
{
    public enum ActionTargetType
    {
        None,
        Self,
        Around,
        Biggest, // most power
        Closest,
        Random,
        All,
    }

    public static class Extensions
    {
        public static GenericBullet SetTileAction(this GenericBullet bullet, System.Action<MyTile> action)
        {
            return bullet.SetAction((comp) =>
            {
                if (!comp || comp is not MyTile tile)
                {
                    Debug.LogError("SetTileAction: Component not a tile");
                    return;
                }
                action(tile);
            });
        }

        public static GenericBullet SetUnitAction(this GenericBullet bullet, System.Action<Unit> action)
        {
            return bullet.SetAction((comp) =>
            {
                if (!comp || comp is not Unit unit)
                {
                    Debug.LogError("SetTileAction: Component not a tile");
                    return;
                }
                action(unit);
            });
        }
    }
}