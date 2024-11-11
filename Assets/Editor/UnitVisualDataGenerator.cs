using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public class UnitVisualDataGenerator
{
    const string rootFolder = "Assets/Game/Sprites/Units";
    const string dataAssetFolder = "Assets/Game/Resources/UnitVisualData";

    [MenuItem("Tools/Print Visual Data")]
    public static void PrintVisualData()
    {
        var sb = new System.Text.StringBuilder();
        foreach (var item in Resources.LoadAll<UnitVisualData>("UnitVisualData"))
        {
            sb.AppendLine($"{item.name}\t{item.name}\t\t{100}\t\tAttack 4; Armor 2");
        } 

        Debug.Log(sb);
    }


    [MenuItem("Tools/Generate Unit Visual Data")]
    public static void GenerateUnitVisualData()
    {
        // Iterate over subfolders
        string[] subfolders = Directory.GetDirectories(rootFolder);

        foreach (string subfolder in subfolders)
        {
            string dirName = Path.GetFileName(subfolder);

            string[] pngFiles = Directory.GetFiles(subfolder, "*.png");

            foreach (string pngFile in pngFiles)
            {
                string pngFileName = Path.GetFileName(pngFile);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pngFileName);

                // Parse the file name
                string pattern = @"sprite-sheet(?:-([a-zA-Z0-9]+))? *- *(\d+)[x-](\d+)";
                Regex regex = new Regex(pattern);

                Match match = regex.Match(fileNameWithoutExtension);

                if (!match.Success)
                {
                    Debug.LogError("Filename does not match expected pattern: " + pngFileName);
                    continue;
                }

                string sequenceStr = match.Groups[1].Value;
                string widthStr = match.Groups[2].Value;
                string heightStr = match.Groups[3].Value;
                var unitName = dirName;

                int spriteWidth = int.Parse(widthStr);
                int spriteHeight = int.Parse(heightStr);

                SpriteSheetSlicer.SliceSpriteSheetByPixels(new(spriteWidth, spriteHeight), pngFile);

                // Get the asset path
                string assetPath = pngFile.Replace(Application.dataPath, "").Replace("\\", "/");

                // Create UnitVisualData asset
                if (!string.IsNullOrEmpty(sequenceStr))
                {
                    unitName = unitName + "_" + sequenceStr;
                }

                string dataAssetPath = Path.Combine(dataAssetFolder, unitName + ".asset")
                    .Replace("\\", "/");

                bool existed = true;
                UnitVisualData unitVisualData = AssetDatabase.LoadAssetAtPath<UnitVisualData>(dataAssetPath);
                if (AssetDatabase.LoadAssetAtPath<UnitVisualData>(dataAssetPath) == null)
                {
                    existed = false;
                    unitVisualData = ScriptableObject.CreateInstance<UnitVisualData>();
                }

                unitVisualData.frames = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath)
                    .OfType<Sprite>()
                    .ToList();

                if (!existed) AssetDatabase.CreateAsset(unitVisualData, dataAssetPath);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("UnitVisualData generation complete.");
    }
}