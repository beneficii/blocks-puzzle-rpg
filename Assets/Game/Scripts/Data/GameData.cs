using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Game/GameData")]
public class GameData : ScriptableObject
{
    public TileColors tileColorsGrid;
    public Sprite spriteBlock;
    public GenericBullet prefabBullet;
    public FxAnimator fxPrefab;
    public GameObject prefabGhostCursor;

    public class TileColors
    {
        public Color color1;
        public Color color2;
    }
}