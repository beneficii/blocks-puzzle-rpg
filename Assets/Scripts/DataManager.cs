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

    public List<BtShapeData> shapes; //{ get; private set; }
    //public List<BtBoardInfo> preBoards;
    public BtBlockData emptyBlock;
    public BtBlockData placeHolderBlock;
    public Dictionary<string, AnimCompanion> vfxDict;
    public Dictionary<string, GameObject> unitActionDict;

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

        //preBoards = gameData.shapeGenerator.GenerateBoards();
        vfxDict = gameData.vFxs.ToDictionary(x => x.id);
        unitActionDict = gameData.actionVisuals.ToDictionary(x => x.name);
    }


    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}