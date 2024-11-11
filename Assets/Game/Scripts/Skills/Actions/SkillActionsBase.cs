using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FancyToolkit;
using GridBoard;
using System.Collections;
using TileActions;

public abstract class SkillClickCondition
{
    protected UISkillButton parent;
    public abstract string GetDescription();
    bool canUse;
    public virtual bool CanUse
    {
        get => canUse;
        set
        {
            canUse = value;
            parent.RefreshUse();
        }
    }

    public virtual string GetErrorUnusable() => "Can't use right now";
    public virtual bool StartingValue => false;

    public virtual void Init(UISkillButton parent)
    {
        this.parent = parent;
        if (parent.board)
        {
            CanUse = StartingValue;
        }
    }

    public abstract void OnClicked();
}

public abstract class SkillActionBase
{
    protected UISkillButton parent;

    public abstract string GetDescription();

    public virtual void Init(UISkillButton parent)
    {
        this.parent = parent;
    }

    public abstract IEnumerator Run(int multiplier = 1);

    protected GenericBullet MakeBullet(Vector2 position)
    {
        var rand = Random.Range(0, 2) == 0;
        var bullet = Game.current.MakeBullet(position)
            .SetSpleen(rand ? Vector2.left : Vector2.right);

        return bullet;
    }

    protected GenericBullet MakeBullet() => MakeBullet(parent.transform.position);

    protected string GetTargetingTypeName(ActionTargetType targetType, string tag = TileData.anyTag)
        => TileActionBase.GetTargetingTypeName(targetType, tag);


    protected IEnumerable<MyTile> FindTileTargets(ActionTargetType targetType, System.Predicate<MyTile> filter = null)
    {
        if (targetType == ActionTargetType.Biggest)
        {
            MyTile biggest = null;
            int maxPower = -1;
            foreach (var item in parent.board.GetAllTiles())
            {

                if (item is not MyTile tile || tile.isBeingPlaced) continue;
                if (tile.StatType == TileStatType.None) continue;
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

        if (targetType == ActionTargetType.Random)
        {
            var list = new List<MyTile>();
            foreach (var item in parent.board.GetAllTiles())
            {
                if (item is not MyTile tile || tile.isBeingPlaced) continue;
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
                if (item is not MyTile tile || tile.isBeingPlaced) continue;
                if (filter != null && !filter(tile)) continue;

                yield return tile;
            }
            yield break;
        }
    }

    protected IEnumerable<MyTile> FindTileTargets(ActionTargetType targetType, string tag)
        => FindTileTargets(targetType, (x) => x.HasTag(tag));


    public void SetCombatState(bool value)
    {
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