using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

[CreateAssetMenu(menuName = "Game/Blocks/Tutorial")]
public class TutorialBlockData : BtBlockData, IHasInfo
{
    public override BtBlockType type => BtBlockType.Tutorial;

    public List<string> GetTooltips()
    {
        var enemy = CombatArena.current.enemy;

        if (!enemy) return new List<string>();

        return enemy.data.tutorialTips;
    }

    public override string GetDescription()
    {
        var enemy = CombatArena.current.enemy;

        if (!enemy) return "";

        return enemy.data.tutorialDescription;
    }

    public string GetTitle() => title;

    public bool ShouldShowInfo()
    {
        throw new System.NotImplementedException();
    }
}