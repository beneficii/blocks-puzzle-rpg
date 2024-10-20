using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FancyToolkit;
using GridBoard;
using System.Collections;

public abstract class TileActionBase
{
    protected MyTile parent;

    protected int Power => parent.Power;

    public abstract string GetDescription(MyTile parent);

    public abstract IEnumerator Run();
    public virtual IEnumerator Run(LineClearData match)
    {
        yield return Run();
        yield return parent.FadeOut(10f);
    }

    protected GenericBullet MakeBullet(Tile parent)
    {
        var rand = Random.Range(0, 2) == 0;
        var bullet = DataManager.current.gameData.prefabBullet.MakeInstance(parent.transform.position)
            .AddSpleen(rand ? Vector2.left : Vector2.right)
            .SetSprite(parent.GetIcon());

        return bullet;
    }

    public virtual void Init(MyTile tile)
    {
        this.parent = tile;
    }

    public virtual void Remove()
    {

    }
}