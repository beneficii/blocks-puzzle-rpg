using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using FancyToolkit;
using System.Linq;

namespace TileShapes
{
    public class ShapeData
    {
        List<Tile.Info> tiles;
        Vector2Int size;
        public int level { get; private set; }
        public Rarity Rarity => (Rarity)level;
        //public int spriteIdx;

        public ShapeData(List<Tile.Info> tiles, int level = 0)
        {
            //spriteIdx = Random.Range(0, DataManager.current.gameData.tileSprites.Count);
            if (tiles.Count == 0) return;  // will be easier to find error

            var min = new Vector2Int(99, 99);
            var max = new Vector2Int(-1, -1);

            foreach (var tile in tiles)
            {
                var d = tile.pos;
                if (d.x > max.x) max.x = d.x;
                if (d.x < min.x) min.x = d.x;
                if (d.y > max.y) max.y = d.y;
                if (d.y < min.y) min.y = d.y;
            }

            size = max + Vector2Int.one - min;

            this.tiles = tiles
                .Select(b => new Tile.Info(b.data, b.pos - min))
                .ToList();

            this.level = level;
        }

        public ShapeData(List<Vector2Int> deltas, int level)
            : this(deltas.Select(d => new Tile.Info(TileCtrl.current.emptyTile, d)).ToList(), level) { }

        public static implicit operator bool(ShapeData item)
            => item.tiles != null && item.tiles.Count > 0;

        public List<Tile.Info> GetTiles(int rotation = 0)
        {
            var size = (rotation % 2 == 0) ? this.size : new Vector2Int(this.size.y, this.size.x);
            var tiles = this.tiles
                .Select(b => b.Rotate(rotation, size))
                .ToList();


            // ToDo: figure out better way
            var min = new Vector2Int(99, 99);
            var max = new Vector2Int(-99, -99);

            foreach (var tile in tiles)
            {
                var d = tile.pos;
                if (d.x > max.x) max.x = d.x;
                if (d.x < min.x) min.x = d.x;
                if (d.y > max.y) max.y = d.y;
                if (d.y < min.y) min.y = d.y;
            }

            size = max + Vector2Int.one - min;
            // ToDo end

            var half = size / 2;
            return tiles
                .Select(b => new Tile.Info(b.data, b.pos - min - half))
                .ToList();
        }

        public TileData SpecialBlock()
        {
            return tiles.FirstOrDefault(x => !x.data.isEmpty)?.data;
        }

        public bool HasUniqueBlock()
        {
            return tiles.Any(x => x.data.rarity > Rarity.Uncommon);
        }

        public bool HasEmptyBlocks()
        {
            return tiles.Any(x => x.data.isEmpty);
        }
    }
}
