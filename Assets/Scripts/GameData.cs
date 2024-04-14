using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

[CreateAssetMenu(menuName = "Game/GameData")]
public class GameData : ScriptableObject
{
    public ShapeGenerator shapeGenerator;
    public Vector2Int gridSize = new Vector2Int(8, 8);

    public List<BtBlockData> blocks;

    public TileColors tileColorsGrid;
    public Sprite spriteBlock;
    public BtBlock prefabBlock;
    public BtShape prefabShape;
    public GenericBullet prefabBullet;

    public class TileColors
    {
        public Color color1;
        public Color color2;
    }
}