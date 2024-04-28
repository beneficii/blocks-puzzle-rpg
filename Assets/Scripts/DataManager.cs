using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-11)]
public class DataManager : MonoBehaviour
{
    public static DataManager current { get; private set; }

    public GameData gameData;

    public List<BtShapeData> shapes; //{ get; private set; }
    public List<BtBoardInfo> preBoards;
    public BtBlockData emptyBlock;
    public BtBlockData placeHolderBlock;

    private void Awake()
    {
        if (current != null)
        {
            Destroy(gameObject);
            return;
        }

        current = this;
        DontDestroyOnLoad(gameObject);

        shapes = gameData.shapeGenerator.GenerateShapes();
        preBoards = gameData.shapeGenerator.GenerateBoards();
    }


    private void OnApplicationQuit()
    {
        //PlayerPrefs.Save();
    }
}