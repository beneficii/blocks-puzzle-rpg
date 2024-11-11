using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.U2D.Sprites;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

public static class SpriteSheetSlicer
{
    private static bool SliceTexture(Vector2Int size, TextureImporter importer, Texture2D texture, string id)
    {
        if (texture.width % size.x != 0 || texture.height % size.y != 0)
        {
            Debug.LogError($"Texture dimensions are not evenly divisible by the given size. ({id})");
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

    public static bool LoadTextureToSpriteSheet(string texturePath, out TextureImporter importer, out Texture2D texture)
    {
        importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        texture = null;
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

        texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        if (texture == null)
        {
            Debug.LogError("Failed to load Texture2D at path: " + texturePath);
            return false;
        }

        return true;
    }

    public static bool SliceSpriteSheetByPixels(Vector2Int size, string texturePath)
    {
        if (!LoadTextureToSpriteSheet(texturePath, out var importer, out var texture)) return false;
        if (!SliceTexture(size, importer, texture, texturePath)) return false;

        return true;
    }

    public static bool SliceSpriteSheetByCount(Vector2Int count, string texturePath)
    {
        if (!LoadTextureToSpriteSheet(texturePath, out var importer, out var texture)) return false;
        var size = new Vector2Int
        {
            x = texture.width / count.x,
            y = texture.height / count.y,
        };

        if (!SliceTexture(size, importer, texture, texturePath)) return false;

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
        SliceSpriteSheetByPixels(GetSpriteSizeFromName(assetPath), assetPath);
    }

    [MenuItem("Tools/Slice Fx Sheet")]
    public static void SliceFxSheet()
    {
        // Get the selected texture
        Texture2D texture = Selection.activeObject as Texture2D;

        if (texture == null)
        {
            Debug.LogError("No texture selected.");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(texture);
        var parts = Path.GetFileNameWithoutExtension(assetPath).Split('_');
        if (parts.Length != 2) return;

        // Try to parse the last part (number of frames)
        if (!int.TryParse(parts[1], out int frameCount))
        {
            Debug.LogError($"Could not parse file name!");
            return;
        }

        SliceSpriteSheetByCount(new Vector2Int(frameCount, 1), assetPath);
    }

    [MenuItem("Tools/GenerateFxData")]
    public static void GenerateFxData()
    {
        var pngFiles = Directory.GetFiles("Assets/Game/Sprites/Fx", "*.png");

        foreach (string pngFile in pngFiles)
        {
            var parts =  Path.GetFileNameWithoutExtension(pngFile).Split('_');
            if (parts.Length != 2) return;

            if (!int.TryParse(parts[1], out int frameCount))
            {
                Debug.LogError($"Could not parse file name `{pngFile}`");
                continue;
            }

            string fxName = parts[0];

            if (!SliceSpriteSheetByCount(new Vector2Int(frameCount, 1), pngFile)) continue;
            string assetPath = pngFile.Replace(Application.dataPath, "").Replace("\\", "/");

            string dataAssetPath = Path.Combine("Assets/Game/Resources/FxData", fxName + ".asset")
                .Replace("\\", "/");

            bool existed = true;
            var fxData = AssetDatabase.LoadAssetAtPath<FxData>(dataAssetPath);
            if (AssetDatabase.LoadAssetAtPath<FxData>(dataAssetPath) == null)
            {
                existed = false;
                fxData = ScriptableObject.CreateInstance<FxData>();
            }

            fxData.frames = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath)
                .OfType<Sprite>()
                .ToList();

            if (!existed) AssetDatabase.CreateAsset(fxData, dataAssetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("FxData generation complete.");
    }
}
