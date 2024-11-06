using FancyToolkit;
using NUnit.Framework;
using RogueLikeMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageData : NodeType
{
    public List<string> units;
    public int difficulty;
    public Type type;
    public List<string> rewards;
    public string specialTile;
    public string background;
    public string iconName;

    public override void Run(NodeInfo info)
    {
        Game.current.EnterLevel(info.index);
    }

    [System.Serializable]
    public enum Type
    {
        None,
        Enemy,
        Elite,
        Shop,
        Dialog,
        Boss,
    }
}