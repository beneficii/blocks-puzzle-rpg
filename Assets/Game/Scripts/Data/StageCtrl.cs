﻿using FancyToolkit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class StageCtrl : GenericDataCtrl<StageData>
{
    static StageCtrl _current;
    public static StageCtrl current
    {
        get
        {
            if (_current == null)
            {
                var cur = new StageCtrl();
                _current = cur;
            }

            return _current;
        }
    }

    Dictionary<StageType, Sprite> dictTypeSprites = new();

    StageData currentData;
    string currentId = "test";

    public StageData Data
    {
        get
        {
            if (currentData == null)
            {
                currentData = current.Get(currentId);
            }

            return currentData;
        }
    }

    public void SetStage(StageData stageData)
    {
        currentData = stageData;
    }

    public void SetStage(string id)
    {
        currentId = id;
        currentData = null;
    }

    // gets node that matches level exactly. for tutorial and stuff
    public StageData GetSpecial(int act, int level, System.Random rng = null)
    {
        var filtered = GetAll()
            .Where(x => x.act == act || x.act == 0 || act == 0)
            .Where(x => x.minLevel == level)
            .ToList();

        if (filtered.Count == 0) return null;

        if (rng != null)
        {
            return filtered[rng.Next(filtered.Count)];
        }
        else
        {
            return filtered.Rand();
        }
    }

    public StageData GetRandom(int act, int level, System.Random rng = null)
    {
        int totalWeight = 0;
        var filtered = new List<StageData>();
        foreach (var item in GetAll())
        {
            if (item.weight <= 0) continue;
            if (item.act != act && item.act != 0 && act != 0) continue;
            if (item.minLevel < 0 || level < item.minLevel) continue;
            if (!item.condition.Build()) continue;

            totalWeight += item.weight;
            filtered.Add(item);
        }

        if (filtered.Count == 0) return null;
        int rand = rng?.Next(totalWeight) ?? Random.Range(0, totalWeight); 

        return filtered.First(x => (rand -= x.weight) < 0);
    }

    public StageData GetRandom(StageType type, int act, int level, System.Random rng = null, List<string> usedIds = null)
    {
        int[] dbgArr = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int totalWeight = 0;
        var filtered = new List<StageData>();
        //Debug.Log(type);
        foreach (var item in GetAll())
        {
            dbgArr[0]++;
            if (item.weight <= 0) continue;
            dbgArr[1]++;
            if (item.type != type) continue;
            dbgArr[2]++;
            if (item.act != act && item.act != 0 && act != 0) continue;
            dbgArr[3]++;
            if (item.minLevel < 0 || level < item.minLevel) continue;
            dbgArr[4]++;
            if (usedIds != null && usedIds.Contains(item.id)) continue;
            dbgArr[5]++;
            if (!item.condition?.Build()) continue;
            dbgArr[6]++;

            totalWeight += item.weight;
            filtered.Add(item);
        }
        //Debug.Log(string.Join(" ", dbgArr));

        if (filtered.Count == 0) return null;
        int rand = rng?.Next(totalWeight) ?? Random.Range(0, totalWeight);

        return filtered.First(x => (rand -= x.weight) < 0);
    }

    public Sprite GetSprite(StageType stageType) => dictTypeSprites.Get(stageType);

    public override void PostInit()
    {
        dictTypeSprites.Clear();
        foreach (var item in EnumUtil.GetValues<StageType>())
        {
            var sprite = Resources.Load<Sprite>($"StageIcons/{item}");
            if (sprite != null)
            {
                dictTypeSprites.Add(item, sprite);
            }
        }
    }
}
