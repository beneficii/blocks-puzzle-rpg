using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-11)]
public class DataManager : MonoBehaviour
{
    public static DataManager current { get; private set; }

    public GameData gameData;

    public List<ShapeInfo> shapes;// { get; private set; }

    private void Awake()
    {
        if (current != null)
        {
            Destroy(gameObject);
            return;
        }

        current = this;
        DontDestroyOnLoad(gameObject);

        shapes = gameData.shapeGenerator.Generate();
    }


    private void OnApplicationQuit()
    {
        //PlayerPrefs.Save();
    }
}