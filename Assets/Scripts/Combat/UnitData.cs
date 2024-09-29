using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;
using Assets.Scripts.Combat;

[CreateAssetMenu(menuName = "Game/Unit/basic")]
public class UnitData : ScriptableObject
{
    public string id;
    public string idVisuals;
    public UnitVisual visuals;
    public string title;
    public string description;
    public AnimatorOverrideController animations;
    public int damage;
    public int defense;
    public int hp;
    public BtUpgradeRarity reward = BtUpgradeRarity.Common;
    public BtBlockData specialBlockData;
    public string specialBlockId;
    public int boardLevel;

    public List<UnitActionBase> actionQueue;

    public string tutorialDescription;
    public List<string> tutorialTips;

    public AnimCompanion fxAttack;
    public AnimCompanion fxDeath;

    public AudioClip soundAttack;
    public AudioClip soundAbility;
    public AudioClip soundDeath;
}

[System.Serializable]
public class UnitData2
{
    public string id;
    public string name;
    public string description;
    public int hp;
    public string specialTile;
    public List<UnitAction.Base> actions;
}
