using FancyToolkit;
using RogueLikeMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageData : NodeType
{
    public List<string> units;
    public int difficulty;
    public Type type;
    public Rarity reward;
    public string specialTile;
    public string background;
    public string dialog;

    public override void Run(NodeInfo info)
    {
        Game.current.EnterLevel(info.index);
    }

    public override void RunCurrent(NodeInfo info)
    {
        UIHudMap.current.Close();
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
        Victory,
    }
}