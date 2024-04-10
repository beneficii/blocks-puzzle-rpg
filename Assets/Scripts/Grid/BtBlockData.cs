using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Blocks/Basic")]
public class BtBlockData : ScriptableObject
{
    public Sprite sprite;
    public BtBlockType type;
}

public enum BtBlockType
{
    None,
    Sword,
    Shield,
    Fire
}