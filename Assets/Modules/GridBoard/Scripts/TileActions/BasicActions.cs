using FancyToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GridBoard
{
    namespace TileActions
    {
        public class Collect : TileAction
        {
            public override bool Run(Tile tile)
            {
                if (!tile.board) return false;

                return tile.board.CollectAt(tile.position);
            }
        }

        public class DebugBlink : TileAction
        {
            public override bool Run(Tile tile)
            {
                tile.Blink(Color.green);

                return true;
            }
        }

        public class Spawn : TileAction
        {
            string id;
            int level = -1;

            public Spawn(StringScanner scanner)
            {
                id = scanner.NextString();
                scanner.TryGet(out level, -1);
            }

            public override bool Run(Tile tile)
            {
                var board = tile.board;
                if (!board) return false;

                var instance = board.PlaceAround(id, tile.position, level >= 0 ? level : tile.Level);

                return instance != null;
            }
        }
    }
}