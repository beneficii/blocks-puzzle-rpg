using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using FancyToolkit;

public abstract class ActionBase
{
    protected Board board;
    protected IActionParent parent;
    public virtual bool OverrideDescriptionKey => false;

    public virtual ActionStatType StatType => ActionStatType.None;

    public abstract string GetDescription();

    public virtual void Init(IActionParent parent)
    {
        this.parent = parent;
    }

    public abstract IEnumerator Run(int multiplier = 1);

    protected GenericBullet MakeBullet(IActionParent source, Vector2? position = null)
    {
        if (position == null)
        {
            position = source.transform.position;
        }
        var rand = Random.Range(0, 2) == 0;
        var bullet = Game.current.MakeBullet(position.Value)
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
        bullet.SetUnitAction((x) => x.AddArmor(value));
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
    int Damage { get; set; }
    int Defense { get; set; }
    Transform transform { get; }
    Board board { get; }

    Component AsComponent();
}

public enum ActionStatType
{
    None,
    Damage,
    Defense,
    Power,  // other
}