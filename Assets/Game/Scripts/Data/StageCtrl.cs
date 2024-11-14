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

    public StageData GetRandom(int difficulty, System.Random rng = null)
    {
        var filtered = list
            .Where(x => x.difficulty == difficulty)
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

    public override void PostInit()
    {
        var sprites = Resources.LoadAll<Sprite>("StageIcons").ToDictionary(x => x.name);
        Debug.Log($"StageCtrl::PostInit sprites: {sprites.Count}");
        foreach (var item in GetAll())
        {
            item.sprite = sprites.Get(item.type.ToString());
        }
    }
}
