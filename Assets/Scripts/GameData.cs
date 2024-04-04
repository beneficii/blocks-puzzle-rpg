using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "RTS/GameData")]
public class GameData : ScriptableObject
{
    public Vector2Int gridSize = new Vector2Int(8, 8);

    public TileColors tileColorsGrid;
    public Sprite spriteBlock;
    public BtBlock prefabBlock;
    public BtShape prefabShape;

    public class TileColors
    {
        public Color color1;
        public Color color2;
    }
}