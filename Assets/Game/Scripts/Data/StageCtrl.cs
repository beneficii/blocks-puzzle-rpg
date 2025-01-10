using FancyToolkit;
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
        var filtered = GetAll()
            .Where(x => x.act == act || x.act == 0 || act == 0)
            .Where(x => x.minLevel >= 0 && level >= x.minLevel)
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

    public StageData GetRandom(StageType type, int act, int level, System.Random rng = null, List<string> usedIds = null)
    {
        var filtered = GetAll()
            .Where(x => x.act == act || x.act == 0 || act == 0)
            .Where(x => x.minLevel >= 0 && level >= x.minLevel)
            .Where(x => x.type == type && (usedIds == null || !usedIds.Contains(x.id)))
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
