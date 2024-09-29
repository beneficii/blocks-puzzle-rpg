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

    public Dictionary<string, UnitData> unitDict { get; private set; }

    private UnitVisual CreateScriptableObject(string name)
    {
#if UNITY_EDITOR
        var newObject = ScriptableObject.CreateInstance<UnitVisual>();

        // Ensure the directory exists
        string path = Path.Combine("Assets/Resources", visualsFolder);
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        // Save the new ScriptableObject to the Resources folder
        string assetPath = Path.Combine(path, name + ".asset");
        UnityEditor.AssetDatabase.CreateAsset(newObject, assetPath);
        UnityEditor.AssetDatabase.SaveAssets();

        Debug.Log("Created new ScriptableObject: " + name);
        return newObject;
#else
        return null;
#endif
    }

    public UnitData GetUnit(string id) => unitDict.Get(id);
    public List<UnitData> GetAllUnits() => unitDict.Values.ToList();

    public TClass GetUnit<TClass>(string id) where TClass : UnitData, new()
        => unitDict.Get(id) as TClass;


    public UnitCtrl()
    {
        unitDict = new();
    }

    public void AddData<TClass>(TextAsset csv) where TClass : UnitData, new()
    {
        var visuals = Resources.LoadAll<UnitVisual>(visualsFolder)
            .ToDictionary(x => x.name);

        var list = FancyCSV.FromText<TClass>(csv.text);
        foreach (var item in list)
        {
            var visualId = item.idVisuals;

            if (string.IsNullOrWhiteSpace(visualId)) continue;

            if (!visuals.TryGetValue(visualId, out UnitVisual visual))
            {
                visual = CreateScriptableObject(visualId);
            }

            item.visuals = visual;
        }

        foreach (var item in list)
        {
            unitDict.Add(item.id, item);
        }
    }
}
