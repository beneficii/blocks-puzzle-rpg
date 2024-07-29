using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using TileShapes;


public class CombatCtrl : MonoBehaviour
{
    [SerializeField] TextAsset tableTiles;

    GridBoard.Board board;
    TileShapes.ShapePanel shapePanel;


    private void Awake()
    {
        TileCtrl.current.AddData<TileData>(tableTiles);
        
        board = FindAnyObjectByType<GridBoard.Board>();
        board.Init();
     

        shapePanel = FindAnyObjectByType<TileShapes.ShapePanel>();
        shapePanel.GenerateNew();

    }
}
