using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;
using Assets.Scripts.Combat;

[System.Serializable]
public class UnitData
{
    public string id;
    public UnitVisualData visuals;
    public string name;
    public string description;
    public int hp;
    public string specialTile;
    public List<FactoryBuilder<UnitAction.Base>> actions = new();
}
