using System.Collections;
using UnityEngine;
using GridBoard;
using FancyToolkit;
using System.Collections.Generic;
using System.Linq;
using GameActions;

public abstract class BuffBase
{
    protected Board board;
    public abstract string GetDescription();

    public void SetBoard(Board board)
    {
        if (this.board == board) return;
        
        if (board)
        {
            this.board = board;
            Add();
        }
        else
        {
            Remove();
            this.board = board;
        }
    }

    protected virtual void Add()
    {
        
    }

    protected virtual void Remove()
    {

    }
}

namespace Buffs
{
    public class AddPower : BuffBase
    {
        int amount;
        string tag;

        public AddPower(int amount, string tag)
        {
            this.amount = amount;
            this.tag = tag;
        }

        public override string GetDescription() => $"{amount.SignedStr()} power to {tag} tiles";

        bool IsMatching(Tile tile) => tile is MyTile myTile && myTile.HasTag(tag);

        IEnumerable<MyTile> GetTargets()
        {
            if (!board) yield break;
            foreach (var item in board.GetAllTiles().Where(x => IsMatching(x)))
            {
                yield return (MyTile)item;
            }
        }

        void Apply(Tile tile, int power)
        {
            if (tile is MyTile myTile) myTile.Power += power;
        }

        void HandleTileInited(Tile item)
        {
            if (!item.board) return;
            if (IsMatching(item)) Apply(item, amount);
        }

        protected override void Add()
        {
            Tile.OnInited += HandleTileInited;
            foreach (var item in GetTargets())
            {
                Apply(item, amount);
            }
        }

        protected override void Remove()
        {
            Tile.OnInited -= HandleTileInited;
            foreach (var item in GetTargets())
            {
                Apply(item, -amount);
            }
        }

        public class Builder : FactoryBuilder<BuffBase, int, string>
        {
            public override BuffBase Build() => new AddPower(value, value2);
        }
    }
}