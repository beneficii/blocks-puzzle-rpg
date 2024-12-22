using FancyToolkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GridBoard
{
    [CreateAssetMenu(menuName = "Game/GridBoard/BoardPresets")]
    public class DatabaseBoardPresets : ScriptableObject
    {
        const string allowedTileColor = "00FF00";

        [SerializeField] Texture2D textureBoards;
        [SerializeField] Vector2Int size = new Vector2Int(8,8); // individual board size

        PredefinedLayout GetBoard(int x, int y, Color32[,] pixels)
        {
            var tiles = new List<Tile.Info>();
            var allowed = new List<Vector2Int>();
            for (int cellY = 0; cellY < size.y; cellY++)
            {
                for (int cellX = 0; cellX < size.x; cellX++)
                {
                    var pos = new Vector2Int(cellX, cellY);
                    var colorCode = ColorUtility.ToHtmlStringRGB(pixels[x + cellX, y + cellY]).ToUpper();
                    if (colorCode == allowedTileColor)
                    {
                        allowed.Add(pos);
                        continue;
                    }

                    var data = TileCtrl.current.colorDict.Get(colorCode);
                    if (data != null)
                    {
                        tiles.Add(new Tile.Info(data, pos));
                    }
                }
            }

            if (tiles.Count == 0) return null;

            return new PredefinedLayout()
            {
                tiles = tiles,
                allowedTiles = allowed,
                level = y / size.y
            };
        }

        public PredefinedLayout FromTexture(Texture2D source)
        {
            var pixels = source.Get2DColors32();
            return GetBoard(0,0, pixels);
        }

        public List<PredefinedLayout> GenerateBoards()
        {
            var result = new List<PredefinedLayout>();
            var pixels = textureBoards.Get2DColors32();
            for (int y = 0; y + size.y - 1 < textureBoards.height; y += size.y)
            {
                for (int x = 0; x + size.x - 1 < textureBoards.width; x += size.x)
                {
                    var board = GetBoard(x, y, pixels);
                    if (board == null) continue;
                    result.Add(board);
                }
            }

            return result;
        }
    }
}
