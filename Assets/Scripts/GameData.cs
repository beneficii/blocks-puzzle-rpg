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
    public BtBlockData emptyBlock;
    public List<BtBlockData> blocks;
    public List<Sprite> blockSprites;

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

#if UNITY_EDITOR
    [ContextMenu("Assign block Ids")]
    void AssignDataIds()
    {
        var unused = Enumerable.Range(1, 99).ToHashSet();
        foreach (var item in blocks) unused.Remove(item.id);

        foreach (var item in blocks)
        {
            if (item.id != 0) continue;

            int id = unused.Rand();
            item.id = id;
            EditorUtility.SetDirty(item);
            unused.Remove(id);
        }
    }
#endif
}