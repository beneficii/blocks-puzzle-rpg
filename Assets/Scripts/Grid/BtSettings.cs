using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/BtGrid/Settings")]
public class BtSettings : ScriptableObject
{
    public List<Color> blockColors;
    public BtBlockData wispBlock;

    public BtShapeInfo GetWispShapeInfo()
    {
        var blocks = new List<BtBlockInfo>() { new BtBlockInfo(wispBlock, Vector2Int.zero) };
        var shapeData = new BtShapeData(blocks);
        return new BtShapeInfo(shapeData);
    }
}