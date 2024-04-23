using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public abstract class BlockActionBase : ScriptableObject
{
    public abstract string GetDescription();

    public abstract void HandleMatch(BtBlock parent, BtLineClearInfo info);

    protected GenericBullet MakeBullet(Vector2 origin)
    {
        var bullet = DataManager.current.gameData.prefabBullet.MakeInstance(origin);

        return bullet;
    }

    protected GenericBullet MakeBullet(BtBlock parent)
    {
        return DataManager.current.gameData.prefabBullet.MakeInstance(parent.transform.position)
            .SetSprite(parent.data.sprite);
    }
}