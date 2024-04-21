using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

[CreateAssetMenu(menuName = "Game/ShapeGenerator")]
public class ShapeGenerator : ScriptableObject
{
    [SerializeField] Texture2D textureShapes;
    [SerializeField] Texture2D textureBoards;
    [SerializeField] List<ColorDataPair> colorsToBlocks;

    [SerializeField] Vector2Int shapeCellSize = new Vector2Int(5, 5);


    public BtBlockData GetBlock(Color32 color)
    {
        foreach (var item in colorsToBlocks)
        {
            if (item.color.Equals(color)) return item.data;
        }

        return null;
    } 

    BtShapeData GetShape(int x, int y, Color32[,] pixels)
    {
        var result = new List<BtBlockInfo>();
        for (int cellY = 0; cellY < shapeCellSize.y; cellY++)
        {
            for (int cellX = 0; cellX < shapeCellSize.x; cellX++)
            {
                var data = GetBlock(pixels[x + cellX, y + cellY]);
                if (data)
                {
                    result.Add(new BtBlockInfo(data, new Vector2Int(cellX, cellY)));
                }
            }
        }

        return new BtShapeData(result, y / shapeCellSize.y);
    }

    
    public List<BtShapeData> GenerateShapes()
    {
        var result = new List<BtShapeData>();
        var pixels = textureShapes.Get2DColors32();

        // Iterate through the image by 5x5 cells
        for (int y = 0; y < textureShapes.height; y += shapeCellSize.y)
        {
            for (int x = 0; x < textureShapes.width; x += shapeCellSize.x)
            {
                var data = GetShape(x,y, pixels);
                if (!data) continue;
                result.Add(data);
            }
        }

        return result;
    }

    BtBoardInfo GetBoard(int x, int y, Color32[,] pixels)
    {
        var colorDict = colorsToBlocks.ToDictionary(x => x.color, x => x.data);
        var blocks = new List<BtBlockInfo>();
        for (int cellY = 0; cellY < BtGrid.height; cellY++)
        {
            for (int cellX = 0; cellX < BtGrid.width; cellX++)
            {
                var data = GetBlock(pixels[x + cellX, y + cellY]);
                if (data)
                {
                    blocks.Add(new BtBlockInfo(data, new Vector2Int(cellX, cellY)));
                }
            }
        }

        if (blocks.Count == 0) return null;

        return new BtBoardInfo()
        {
            blocks = blocks,
            level = y / BtGrid.height
        };
    }

    public List<BtBoardInfo> GenerateBoards()
    {
        var result = new List<BtBoardInfo>();
        var pixels = textureBoards.Get2DColors32();

        // Iterate through the image by 8x8 cells
        for (int y = 0; y < textureBoards.height; y += BtGrid.height)
        {
            for (int x = 0; x < textureBoards.width; x += BtGrid.width)
            {
                var board = GetBoard(x, y, pixels);
                if (board == null) continue;
                result.Add(board);
            }
        }

        return result;
    }

    [ContextMenu("AutoFill Colors")]
    void FillColorFromTexture()
    {
        var allColors = textureBoards.GetPixels32().Concat(textureShapes.GetPixels32());
        
        foreach (var color in allColors)
        {
            if (!color.Equals(Color.black) && colorsToBlocks.FirstOrDefault(x=>x.color.Equals(color)) == null)
            {
                colorsToBlocks.Add(new(color));
            }
        }
    }

    [System.Serializable]
    public class ColorDataPair
    {
        public Color32 color;
        public BtBlockData data;

        public ColorDataPair(Color color, BtBlockData data = null)
        {
            this.color = color;
            this.data = data;
        }
    }
}

[System.Serializable]
public class BtBoardInfo
{
    public List<BtBlockInfo> blocks;
    public int level;
}