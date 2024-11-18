using Assets.Scripts.Combat;
using FancyToolkit;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitCtrl : GenericDataCtrl<UnitData>
{
    public const string visualsFolder = "UnitVisualData";

    static UnitCtrl _current;
    public static UnitCtrl current
    {
        get
        {
            if (_current == null)
            {
                _current = new UnitCtrl();
            }

            return _current;
        }
    }

    public override void PostInit()
    {
        var visuals = Resources.LoadAll<UnitVisualData>(visualsFolder).ToDictionary(x => x.name);
        foreach (var unit in GetAll())
        {
            if (!visuals.TryGetValue(unit.id, out var visual))
            {
                Debug.LogError($"No visual data for unit '{unit.id}'");
            }
            else
            {
                unit.visuals = visual;
            }
        }
    }
}
