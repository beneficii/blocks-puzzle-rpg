using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

[CreateAssetMenu(menuName = "Game/CombatBlockData")]
public class CombatBlockData : BtBlockData
{
    public override BtBlockType type => BtBlockType.Basic;
    protected CombatArena Arena => CombatArena.current;

    public List<BlockTag> tags;
    public List<BlockActionBase> actions;
    public List<BlockPassive> passives;

    public bool HasTag(BlockTag tag) => tags.Contains(tag);

    public override string GetDescription()
    {
        var description = new System.Text.StringBuilder();
        var sb = new System.Text.StringBuilder();
        foreach (var item in actions)
        {
            var descr = item.GetDescription();
            if (!string.IsNullOrEmpty(descr))
            {
                sb.Append(descr + ".");
            }
        }

        if (sb.Length > 0)
        {
            description.Append("<b>Collect:</b> " + sb.ToString());
        }

        sb.Clear();

        foreach (var item in passives)
        {
            var descr = item.GetDescription();
            if (!string.IsNullOrEmpty(descr))
            {
                sb.Append(descr + ".");
            }
        }

        if (sb.Length > 0)
        {
            description.Append("<b>Passive:</b> " + sb.ToString());
        }

        return description.ToString();
    }

    public override void HandleMatch(BtBlock parent, BtLineClearInfo info)
    {
        info.blocks.Remove(parent);
        foreach (var item in actions)
        {
            item.HandleMatch(parent, info);
        }
    }

    public void CalculatePassives(BtBlock parent)
    {
        foreach (var item in passives)
        {
            item.Calculate(parent);
        }
    }
}


public enum BlockTag
{
    None,
    Sword,
    Shield,
    Staff,
    Spell,
    Curse,
}
