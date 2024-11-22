using GridBoard;
using System.Collections;
using UnityEngine;

public abstract class TileActionBase : ActionBase
{
    public MyTile tileParent;

    public override void Init(IActionParent parent)
    {
        base.Init(parent);
        tileParent = parent as MyTile;
        if (!tileParent && parent is not MyTileData)
        {
            Debug.LogError($"Tried to init {GetType().Name} action on non tile");
        }
    }
}