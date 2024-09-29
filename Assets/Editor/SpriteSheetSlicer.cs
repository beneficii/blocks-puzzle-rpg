using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using static UnityEditor.U2D.ScriptablePacker;
using UnityEditor.U2D.Sprites;
using System.IO;
using System.Text.RegularExpressions;

public static class SpriteSheetSlicer
{
    /// <summary>
    /// Slices a spritesheet texture into individual sprites.
    /// </summary>
    /// <param name="size">The size of each sprite in pixels (width x height).</param>
    /// <param name="texturePath">The asset path to the spritesheet texture.</param>
    public static bool SliceSpriteSheet(Vector2Int size, string texturePath)
    {
        // Load the TextureImporter for the asset
        TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        if (importer == null)
        {
            Debug.LogError("Failed to load TextureImporter at path: " + texturePath);
            return false;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.maxTextureSize = 4096;

        importer.SaveAndReimport();
        EditorUtility.SetDirty(importer);

        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        if (texture == null)
        {
            Debug.LogError("Failed to load Texture2D at path: " + texturePath);
            return false;
        }

        if (texture.width % size.x != 0 || texture.height % size.y != 0)
        {
            Debug.LogError("Texture dimensions are not evenly divisible by the given size.");
            return false;
        }

        var spriteRects = new List<SpriteRect>();
        int frameNumber = 0;
        for (int j = texture.height; j > 0; j -= size.y)
        {
            for (int i = 0; i < texture.width; i += size.x)
            {
                
                var spriteMetaData = new SpriteRect
                {
                    name = $"{texture.name}_{frameNumber:000}",
                    rect = new Rect(i, j - size.y, size.x, size.y),
                    alignment = SpriteAlignment.BottomCenter,
                    pivot = new Vector2(.5f, 0f),
                };
                

                spriteRects.Add(spriteMetaData);
                frameNumber++;
            }
        }

        var factory = new SpriteDataProviderFactories();
        factory.Init();
        var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
        dataProvider.InitSpriteEditorDataProvider();

        dataProvider.SetSpriteRects(spriteRects.ToArray());
        dataProvider.Apply();

        importer.SaveAndReimport();

        return true;
    }

    static Vector2Int GetSpriteSizeFromName(string pngFileName)
    {
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pngFileName);

        // Parse the file name
        string pattern = @"sprite-sheet(?:-([a-zA-Z0-9]+))? *- *(\d+)[x-](\d+)";
        Regex regex = new Regex(pattern);

        Match match = regex.Match(fileNameWithoutExtension);

        if (!match.Success)
        {
            Debug.LogError("Filename does not match expected pattern: " + pngFileName);
            return default;
        }

        string widthStr = match.Groups[2].Value;
        string heightStr = match.Groups[3].Value;

        int spriteWidth = int.Parse(widthStr);
        int spriteHeight = int.Parse(heightStr);

        return new Vector2Int(spriteWidth, spriteHeight);
    }

    [MenuItem("Tools/Slice Spritesheet")]
    public static void SliceSelectedSpritesheet()
    {
        // Get the selected texture
        Texture2D texture = Selection.activeObject as Texture2D;

        if (texture == null)
        {
            Debug.LogError("No texture selected.");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(texture);

        // Call the slicing function
        SliceSpriteSheet(GetSpriteSizeFromName(assetPath), assetPath);
    }
}
