using FancyToolkit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace GridBoard
{
    public class TileCtrl
    {
        const string visualsFolder = "TileVisuals";

        static TileCtrl _current;
        public static TileCtrl current
        {
            get
            {
                if (_current == null)
                {
                    var cur = new TileCtrl();
                    _current = cur;
                }

                return _current;
            }
        }

        public TileData emptyTile { get; private set; }
        public TileData placeholderTile { get; private set; }

        public Dictionary<string, TileData> tileDict { get; private set; }
        public Dictionary<string, TileData> colorDict { get; private set; }

        private TileVisuals CreateScriptableObject(string name)
        {
#if UNITY_EDITOR
            var newObject = ScriptableObject.CreateInstance<TileVisuals>();

            // Ensure the directory exists
            string path = Path.Combine("Assets/Resources", visualsFolder);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // Save the new ScriptableObject to the Resources folder
            string assetPath = Path.Combine(path, name + ".asset");
            UnityEditor.AssetDatabase.CreateAsset(newObject, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();

            Debug.Log("Created new ScriptableObject: " + name);
            return newObject;
#else
            return null;
#endif
        }

        public TileData GetTile(string id) => tileDict.Get(id);
        public List<TileData> GetAllTiles() => tileDict.Values.ToList();
        public List<TileData> GetAllTiles(Rarity rarity) => tileDict.Values.Where(x=>x.rarity == rarity).ToList();

        public TClass GetTile<TClass>(string id) where TClass : TileData, new ()
            => tileDict.Get(id) as TClass;

        void InitBaseTiles()
        {
            tileDict = new();
            colorDict = new();
            emptyTile = new TileData
            {
                id = "empty",
                isEmpty = true,
                colorCode = "FFFFFF",
            };
            placeholderTile = new TileData
            {
                id = "placeholder",
                isEmpty = true,
                colorCode = "C65197",
            };
            AddTile(emptyTile);
            AddTile(placeholderTile);
        }

        public TileCtrl()
        {
            InitBaseTiles();
        }

        void AddTile(TileData data)
        {
            tileDict.Add(data.id, data);
            var cc = data.colorCode?.ToUpper();
            if (!string.IsNullOrWhiteSpace(cc) && cc != "000000")
            {
                colorDict.Add(cc, data);
            }
        }

        public void AddData<TClass>(TextAsset csv) where TClass : TileData, new()
        {
            var visuals = Resources.LoadAll<TileVisuals>(visualsFolder)
                .ToDictionary(x => x.name);

            var list = FancyCSV.FromText<TClass>(csv.text);
            foreach (var item in list)
            {
                var visualId = item.idVisuals;

                if (string.IsNullOrWhiteSpace(visualId)) continue;

                if (!visuals.TryGetValue(visualId, out TileVisuals visual))
                {
                    visual = CreateScriptableObject(visualId);
                }

                item.visuals = visual;
            }

            foreach (var item in list)
            {
                AddTile(item);
            }
        }
    }
}