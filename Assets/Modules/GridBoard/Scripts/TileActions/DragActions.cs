using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using FancyToolkit;

namespace GridBoard.TileActions
{
    public class Exchange : TileActionAccept
    {
        string idIn, idOut;
        float duration;

        public Exchange(StringScanner scanner)
        {
            idIn = scanner.NextString();
            idOut = scanner.NextString();
            duration = scanner.NextFloat();
        }

        public override bool CanAccept(Tile tile, Tile target)
            => tile.Level <= target.Level && tile.data.id == idIn;

        public override void Accept(Tile tile, Tile target)
        {
            var board = tile.board;
            Board.RemoveTile(tile, true);
            target.StartProgress(duration, (x) =>
            {
                board.PlaceAround(idOut, tile.position, tile.Level);
            });
        }
    }

    public class Morph : TileActionAccept
    {
        string idIn, idOut;
        int nIn, nTarget;

        public Morph(StringScanner scanner)
        {
            idIn = scanner.NextString();
            idOut = scanner.NextString();
            nIn = scanner.NextInt();
            nTarget = scanner.NextInt();
        }

        public override bool CanAccept(Tile tile, Tile target)
        {
            if (tile.data.id != idIn) return false;

            int lIn = tile.Level;
            int lTarget = target.Level;

            if (lIn % nIn != 0 || lTarget % nTarget != 0) return false;

            return (lIn / nIn) == (lTarget / nTarget);
        }

        public override void Accept(Tile tile, Tile target)
        {
            var board = target.board;
            int level = tile.Level / nIn;
            Board.RemoveTile(tile, true);
            Board.RemoveTile(target, true);
            board.PlaceAround(idOut, target.position, level);
        }
    }

    public class Merge : TileActionAccept
    {
        public override bool CanAccept(Tile tile, Tile target)
            => target.data == tile.data && target.Level == tile.Level;

        public override void Accept(Tile tile, Tile target)
        {
            Board.RemoveTile(tile);
            target.Level++;
        }
    }
}
