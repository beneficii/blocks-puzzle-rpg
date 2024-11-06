using FancyToolkit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace GridBoard
{
    public class TileCtrl
    {
        const string visualsFolder = "TileIcons";

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

        public TileData GetTile(string id) => tileDict.Get(id);
        public List<TileData> GetAllTiles() => tileDict.Values.Where(x=>x.rarity != Rarity.None).ToList();
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
                rarity = Rarity.None,
                type = Tile.Type.Empty,
            };
            placeholderTile = new TileData
            {
                id = "placeholder",
                isEmpty = true,
                colorCode = "C65197",
                rarity = Rarity.None,
                type = Tile.Type.Empty,
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
            var visuals = Resources.LoadAll<Sprite>(visualsFolder)
                .ToDictionary(x => x.name);

            var list = FancyCSV.FromText<TClass>(csv.text);
            foreach (var item in list)
            {
                var visualId = item.idVisuals;

                if (string.IsNullOrWhiteSpace(visualId)) continue;

                item.sprite = visuals.Get(visualId);
                if (item.sprite == null)
                {
                    Debug.LogError($"No sprite for {item.id} ({visualId})");
                }
            }

            foreach (var item in list)
            {
                AddTile(item);
            }
        }
    }
}