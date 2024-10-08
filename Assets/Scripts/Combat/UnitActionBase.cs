﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public abstract class UnitActionBase : ScriptableObject
{
    public GameObject prefabVisual;
    public abstract string GetDescription(Unit parent);
    public virtual string GetTooltip(Unit parent) => "";
    public abstract string GetShortDescription(Unit parent);

    public abstract IEnumerator Execute(Unit parent, Unit target);

    protected GenericBullet MakeBullet(Unit parent)
    {
        return DataManager.current.gameData.prefabBullet.MakeInstance(parent.transform.position);
    }
}