using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

[DefaultExecutionOrder(-11)]
public class DataManager : MonoBehaviour
{
    public static DataManager current { get; private set; }

    public GameData gameData;

    public Dictionary<string, AnimCompanion> vfxDict;
    public Dictionary<string, FxData> fxDict;

    public Dictionary<string, GameObject> unitActionPrefabs;

    private void Awake()
    {
        /*
        if (current != null)
        {
            Destroy(gameObject);
            return;
        }*/

        current = this;
        //DontDestroyOnLoad(gameObject);
        /*if (GameSave.HasSave())
        {
            GameSave.Load();
        }
        else
        {
            //shapes = gameData.shapeGenerator.GenerateShapes();
        }*/

        unitActionPrefabs = Resources.LoadAll<GameObject>("ActionVisuals").ToDictionary(x => x.name);
        fxDict = Resources.LoadAll<FxData>("FxData").ToDictionary(x => x.name);
    }

    public FxAnimator CreateFX(string id, Vector2 position, System.Action action = null)
    {
        if (!fxDict.TryGetValue(id, out var data))
        {
            Debug.LogError($"No Fx with id `{id}` found!");
        }

        return CreateFX(data, position, action);
    }

    public FxAnimator CreateFX(FxData data, Vector2 position, System.Action action = null)
    {
        if (!data)
        {
            action?.Invoke();
            return null;
        }
        var instance = Instantiate(gameData.fxPrefab, position, Quaternion.identity);
        instance.Init(data, action);

        return instance;
    }


    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}