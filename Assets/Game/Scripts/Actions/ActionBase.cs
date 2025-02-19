﻿using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using FancyToolkit;

public abstract class ActionBase
{
    protected Board board;
    protected IActionParent parent;
    public virtual bool OverrideDescriptionKey => false;
    public virtual IHasInfo GetExtraInfo() => null;
    public virtual IEnumerable<IHintProvider> GetHints() => Enumerable.Empty<IHintProvider>();

    public virtual ActionStatType StatType => ActionStatType.None;

    public abstract string GetDescription();

    public virtual void Init(IActionParent parent)
    {
        this.parent = parent;
    }

    public abstract IEnumerator Run(int multiplier = 1);

    protected GenericBullet MakeBullet(IActionParent source, Vector2? position = null, string vfxId = null)
    {
        if (position == null)
        {
            position = source.transform.position;
        }

        if (vfxId == null)
        {
            vfxId = source.VfxId;
        }

        var rand = Random.Range(0, 2) == 0;
        var bullet = Game.current.MakeBullet(position.Value, vfxId)
            .SetSpleen(rand ? Vector2.left : Vector2.right);

        if (source is IHasInfo info)
        {
            bullet.SetSprite(info.GetIcon());
        }

        return bullet;
    }

    public virtual void SetBulletDamage(GenericBullet bullet, int value)
    {
        LineClearData.current?.RegisterValue(MyTile.keyDamage, value);
        bullet.SetDamage(value);
    }

    public virtual void SetBulletDefense(GenericBullet bullet, int value)
    {
        LineClearData.current?.RegisterValue(MyTile.keyArmor, value);
        bullet
            .SetAudio(AudioCtrl.current?.clipArmor)
            .SetUnitAction((x) => x.AddArmor(value));
    }

    public void SetBoard(Board board)
    {
        if (this.board == board) return;
        this.board = board;

        if (board)
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

namespace GameActions
{
    public abstract class ActionBaseWithNested : ActionBase
    {
        protected ActionBase nestedAction;

        public override void Init(IActionParent parent)
        {
            base.Init(parent);
            nestedAction?.Init(parent);
        }

        public override IEnumerable<IHintProvider> GetHints()
        {
            if (nestedAction != null)
            {
                foreach (var item in nestedAction?.GetHints())
                {
                    yield return item;
                }
            }
        }
    }
}

public static class ActionExtensions
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

public interface IActionParent
{
    int Power { get; set; }
    Transform transform { get; }
    Board board { get; }
    string VfxId { get; }

    Component AsComponent();
}

public enum ActionStatType
{
    None,
    Damage,
    Defense,
    Power,  // other
}