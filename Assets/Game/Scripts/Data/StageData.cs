using FancyToolkit;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageData : DataWithId
{
    public List<string> units;
    public int difficulty;
    public Type type;
    public List<string> rewards;
    public string specialTile;

    [System.Serializable]
    public enum Type
    {
        None,
        Enemy,
        Elite,
        Shop,
        Dialog,
        Boss
    }
}