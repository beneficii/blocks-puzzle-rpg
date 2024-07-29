using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using System.Linq;
using FancyToolkit;

namespace TileShapes
{
    [CreateAssetMenu(menuName = "Game/TileShapes/DatabaseShapes")]
    public class DatabaseShapes : ScriptableObject
    {
        [SerializeField] Texture2D textureShapes;

        [SerializeField] Vector2Int shapeCellSize = new Vector2Int(5, 5);

        ShapeData GetShape(int x, int y, Color32[,] pixels)
        {
            var result = new List<Tile.Info>();
            for (int cellY = 0; cellY < shapeCellSize.y; cellY++)
            {
                for (int cellX = 0; cellX < shapeCellSize.x; cellX++)
                {
                    var colorCode = ColorUtility.ToHtmlStringRGB(pixels[x + cellX, y + cellY]).ToUpper();
                    var data = TileCtrl.current.colorDict.Get(colorCode);
                    if (data != null)
                    {
                        result.Add(new Tile.Info(data, new Vector2Int(cellX, cellY)));
                    }
                }
            }

            return new ShapeData(result, y / shapeCellSize.y);
        }


        public List<ShapeData> GenerateShapes()
        {
            var result = new List<ShapeData>();
            var pixels = textureShapes.Get2DColors32();

            // Iterate through the image by 5x5 cells
            for (int y = 0; y < textureShapes.height; y += shapeCellSize.y)
            {
                for (int x = 0; x < textureShapes.width; x += shapeCellSize.x)
                {
                    var data = GetShape(x, y, pixels);
                    if (!data) continue;
                    result.Add(data);
                }
            }

            return result;
        }
    }
}