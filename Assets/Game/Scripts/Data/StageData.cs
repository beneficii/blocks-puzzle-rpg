using FancyToolkit;
using RogueLikeMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageData : DataWithId
{
    public List<string> units;
    public int act;
    public int minLevel;
    public StageType type;
    public Rarity reward;
    public string specialTile;
    public string background;
    public string dialog;
    public string gameOverText;
    public string scenario;
}

public class NodeTypeStage : NodeType
{
    public StageType type;
    public string definedId;

    public NodeTypeStage(StageType type)
    {
        this.type = type;
        this.definedId = null;
    }

    public StageData GetData(NodeInfo info, int act = 0)
    {
        StageData data = null;
        int level = info.pos.x;
        if (!string.IsNullOrEmpty(definedId))
        {
            data = StageCtrl.current.Get(definedId);
            if (data != null) return data;

            Debug.LogError($"Defined stage id `{definedId}` not found!");
        }

        var rng = new System.Random(info.random);

        switch (type)
        {
            case StageType.Enemy:
            case StageType.Elite:
            case StageType.Dialog:
            case StageType.Boss:
            {
                data = StageCtrl.current.GetRandom(type, act, level, rng, Game.current.GetEncounteredStages());
                break;
            }
            case StageType.Shop:
            case StageType.Camp:
                data = StageCtrl.current.Get(type.ToString());
                break;
        }

        // fallback to random
        if (data == null) data = StageCtrl.current.GetRandom(type, act, level, rng);
        // fallback to error
        if (data == null) data = StageCtrl.current.Get("error");

        return data;
    }

    public override void Run(NodeInfo info)
    {
        Game.current.EnterLevel(info.index);
    }

    public override void RunCurrent(NodeInfo info)
    {
        UIHudMap.current.Close();
    }
}

[System.Serializable]
public enum StageType
{
    None,
    Enemy,
    Elite,
    Shop,
    Dialog,
    Camp,
    Boss,
    Victory,
}