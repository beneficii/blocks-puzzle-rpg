using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public abstract class BlockActionBase : ScriptableObject
{
    public abstract string GetDescription();
    public virtual string GetTooltip() => "";
    
    [SerializeField] AnimCompanion fxPrefab;

    public abstract void HandleMatch(BtBlock parent, BtLineClearInfo info);

    protected GenericBullet MakeBullet(Vector2 origin)
    {
        var bullet = DataManager.current.gameData.prefabBullet.MakeInstance(origin);

        return bullet;
    }

    protected GenericBullet MakeBullet(BtBlock parent)
    {
        var rand = Random.Range(0, 2) == 0;
        var bullet = DataManager.current.gameData.prefabBullet.MakeInstance(parent.transform.position)
            .AddSpleen(rand?Vector2.left:Vector2.right)
            .SetSprite(parent.data.sprite);


        return bullet;
    }
}