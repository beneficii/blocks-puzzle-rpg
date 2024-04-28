using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

[CreateAssetMenu(menuName = "Game/GameData")]
public class GameData : ScriptableObject
{
    public ShapeGenerator shapeGenerator;

    public List<BtBlockData> blocks;
    public List<Sprite> blockSprites;

    public TileColors tileColorsGrid;
    public Sprite spriteBlock;
    public BtBlock prefabBlock;
    public BtShape prefabShape;
    public GenericBullet prefabBullet;

    public Sprite spriteTempHero;
    public Sprite spriteTempEnemy;

    public class TileColors
    {
        public Color color1;
        public Color color2;
    }
}