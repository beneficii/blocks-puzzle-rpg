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

    public bool HasTag(BlockTag tag) => tags.Contains(tag);

    public override string GetDescription()
    {
        var lines = new List<string>();
        foreach (var item in actions)
        {
            var descr = item.GetDescription();
            if (!string.IsNullOrEmpty(descr))
            {
                lines.Add(descr);
            }
        }

        var result = string.Join(". ", lines) + (lines.Count > 1 ? "." : "");
        return result;
    }

    public override void HandleMatch(BtBlock parent, BtLineClearInfo info)
    {
        info.blocks.Remove(parent);
        foreach (var item in actions)
        {
            item.HandleMatch(parent, info);
        }
    }
}


public enum BlockTag
{
    None,
    Sword,
    Shield,
    Staff,
    Spell
}
