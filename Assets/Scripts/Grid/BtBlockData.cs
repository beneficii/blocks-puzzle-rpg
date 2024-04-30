using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Blocks/Basic")]
public class BtBlockData : ScriptableObject
{
    public string title;
    public Sprite sprite;
    public virtual BtBlockType type => BtBlockType.None;
    public int priority;

    public BtUpgradeRarity rarity;

    public virtual string GetDescription() => "";

    public virtual void HandleMatch(BtBlock parent, BtLineClearInfo info)
    {
        // nothing
    }
}

public enum BtBlockType
{
    None,
    Basic,
    Special,
    Tutorial,
}