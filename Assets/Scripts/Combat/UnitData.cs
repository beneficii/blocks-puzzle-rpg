using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

[CreateAssetMenu(menuName = "Game/Unit/basic")]
public class UnitData : ScriptableObject
{
    public string title;
    public string description;
    public AnimatorOverrideController animations;
    public int damage;
    public int defense;
    public int hp;
    public BtUpgradeRarity reward = BtUpgradeRarity.Common;
    public BtBlockData specialBlockData;
    public int boardLevel;

    public List<UnitActionBase> actionQueue;

    public string tutorialDescription;
    public List<string> tutorialTips;

    public AnimCompanion fxAttack;
    public AnimCompanion fxDeath;
}