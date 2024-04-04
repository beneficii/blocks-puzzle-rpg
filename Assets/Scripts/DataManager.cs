using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-11)]
public class DataManager : MonoBehaviour
{
    public static DataManager current { get; private set; }

    public GameData gameData;

    private void Awake()
    {
        if (current != null)
        {
            Destroy(gameObject);
            return;
        }

        current = this;
        DontDestroyOnLoad(gameObject);
    }


    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}