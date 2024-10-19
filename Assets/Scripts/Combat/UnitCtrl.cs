using Assets.Scripts.Combat;
using FancyToolkit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class UnitCtrl
{
    const string visualsFolder = "UnitVisualData";

    static UnitCtrl _current;
    public static UnitCtrl current
    {
        get
        {
            if (_current == null)
            {
                var cur = new UnitCtrl();
                _current = cur;
            }

            return _current;
        }
    }

    public Dictionary<string, UnitData2> unitDict { get; private set; }

    public UnitData2 GetUnit(string id) => unitDict.Get(id);
    public List<UnitData2> GetAllUnits() => unitDict.Values.ToList();

    public TClass GetUnit<TClass>(string id) where TClass : UnitData2, new()
        => unitDict.Get(id) as TClass;


    public UnitCtrl()
    {
        unitDict = new();
    }

    public void AddData<TClass>(TextAsset csv) where TClass : UnitData2, new()
    {
        var visuals = Resources.LoadAll<UnitVisualData>(visualsFolder)
            .ToDictionary(x => x.name);

        var list = FancyCSV.FromText<TClass>(csv.text);
        foreach (var item in list)
        {
            var visualId = item.id;

            if (string.IsNullOrWhiteSpace(visualId)) continue;

            if (!visuals.TryGetValue(visualId, out UnitVisualData visual))
            {
                Debug.LogError($"No visual data for unit '{visualId}'");
            }

            item.visuals = visual;
        }

        foreach (var item in list)
        {
            unitDict.Add(item.id, item);
        }
    }
}
