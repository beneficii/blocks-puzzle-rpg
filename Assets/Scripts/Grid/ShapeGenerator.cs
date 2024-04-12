using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/ShapeGenerator")]
public class ShapeGenerator : ScriptableObject
{
    [SerializeField] Texture2D image;

    [SerializeField] Vector2Int cellSize = new Vector2Int(5, 5);

    BtShapeData GetShape(int x, int y)
    {
        var result = new List<Vector2Int>();
        for (int cellY = 0; cellY < cellSize.y; cellY++)
        {
            for (int cellX = 0; cellX < cellSize.x; cellX++)
            {
                Color pixelColor = image.GetPixel(x+cellX, y+cellY);

                if (pixelColor == Color.white)
                {
                    result.Add(new Vector2Int(cellX, cellY));
                }
            }
        }

        return new BtShapeData(result, y / cellSize.y);
    }

    public List<BtShapeData> Generate()
    {
        var result = new List<BtShapeData>();

        // Iterate through the image by 5x5 cells
        for (int y = 0; y < image.height; y += cellSize.y)
        {
            for (int x = 0; x < image.width; x += cellSize.x)
            {
                var data = GetShape(x,y);
                if (!data) continue;
                result.Add(data);
            }
        }

        return result;
    }
}
