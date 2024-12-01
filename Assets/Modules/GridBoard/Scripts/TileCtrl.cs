using FancyToolkit;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GridBoard
{
    public class TileCtrl : GenericDataCtrl<TileData>
    {
        const string visualsFolder = "TileIcons";

        static TileCtrl _current;
        public static TileCtrl current
        {
            get
            {
                if (_current == null)
                {
                    _current = new TileCtrl();
                }

                return _current;
            }
        }

        public Dictionary<string, TileData> colorDict { get; private set; }

        public TileData emptyTile { get; private set; }
        public TileData placeholderTile { get; private set; }

        public TileCtrl()
        {
            colorDict = new();

            emptyTile = Add(new TileData
            {
                id = "empty",
                title = "empty",
                isEmpty = true,
                idVisuals = null,
                colorCode = "FFFFFF",
                rarity = Rarity.None,
                type = Tile.Type.Empty,
            });

            placeholderTile = Add(new TileData
            {
                id = "placeholder",
                title = "empty",
                isEmpty = true,
                idVisuals = null,
                colorCode = "C65197",
                rarity = Rarity.None,
                type = Tile.Type.Empty,
            });
        }

        public override void PostInitSingle(TileData data)
        {
            if (data.idVisuals != null)
            {
                var visual = Resources.Load<Sprite>($"{visualsFolder}/{data.idVisuals}");
                if (!visual)
                {
                    Debug.LogError($"No sprite for {data.id} ({data.idVisuals})");
                    return;
                }

                data.sprite = visual;
            }
            
            var cc = data.colorCode?.ToUpper();
            if (!string.IsNullOrWhiteSpace(cc) && cc != "000000")
            {
                colorDict.Add(cc, data);
            }
        }

        public List<TileData> GetAllTiles() => GetAll().Where(x => x.rarity != Rarity.None).ToList();
        public List<TileData> GetAllTiles(Rarity rarity) => GetAll().Where(x => x.rarity == rarity).ToList();

        public void DebugAll()
        {
            var sb = new System.Text.StringBuilder();
            foreach (var data in GetAll())
            {
                sb.AppendLine($"{data.title} ({data.type}, {string.Join(", ", data.tags)}): {data.GetDescription()}");
            }
            Debug.Log(sb.ToString());
        }
    }
}
