using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FancyToolkit;
using GridBoard;
using System.Collections;
using TileActions;
using System;

public abstract class TileActionBase : ActionBase
{
    protected const string keyDamage = "damage";
    protected const string keyArmor = "armor";

    public bool isOnBoard;
    public MyTile tileParent;

    protected int Power => throw new NotImplementedException();


    public virtual IEnumerator Run(LineClearData match)
    {
        yield return Run();
        yield return tileParent.FadeOut(10f);
    }

    public override void SetBulletDamage(GenericBullet bullet, int value)
    {
        LineClearData.current?.RegisterValue(keyDamage, value);
        base.SetBulletDamage(bullet, value);
    }

    public override void SetBulletDefense(GenericBullet bullet, int value)
    {
        LineClearData.current?.RegisterValue(keyArmor, value);
        base.SetBulletDefense(bullet, value);
    }

    public override void Init(IActionParent parent)
    {
        base.Init(parent);
        tileParent = parent as MyTile;
        if (!tileParent)
        {
            Debug.LogError($"Tried to init {GetType().Name} action on non tile");
        }
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