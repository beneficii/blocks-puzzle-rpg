using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;
using Assets.Scripts.Combat;

[System.Serializable]
public class UnitData : DataWithId
{
    public UnitVisualData visuals;
    public string idVisual;
    public string name;
    public string description;
    public int hp;
    public int damage;
    public int defense;
    public string specialTile;
    public List<FactoryBuilder<UnitAction.Base>> actions = new();
}
