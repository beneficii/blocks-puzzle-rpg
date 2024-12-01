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
        this.board = board;

        if (board)
        {
            Add();
        }
        else
        {
            Remove();
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
        ActionStatType stat;
        int amount;
        string tag;

        public AddPower(ActionStatType stat, int amount, string tag)
        {
            this.stat = stat;
            this.amount = amount;
            this.tag = tag;
        }

        public override string GetDescription() => $"{amount.SignedStr()} {stat} to {tag} tiles";

        bool IsMatching(Tile tile) => tile is MyTile myTile && myTile.HasTag(tag);

        IEnumerable<MyTile> GetTargets()
        {
            foreach (var item in board.GetAllTiles().Where(x => IsMatching(x)))
            {
                yield return (MyTile)item;
            }
        }

        void Apply(Tile tile, int power)
        {
            if (tile is MyTile myTile) myTile.Power += power;
        }

        void HandleTileOnBoard(Tile item, bool state)
        {
            if (!state) return;
            if (IsMatching(item)) Apply(item, amount);
        }

        protected override void Add()
        {
            Tile.OnChangedBoardState += HandleTileOnBoard;
            foreach (var item in GetTargets())
            {
                Apply(item, amount);
            }
        }

        protected override void Remove()
        {
            Tile.OnChangedBoardState -= HandleTileOnBoard;
            foreach (var item in GetTargets())
            {
                Apply(item, -amount);
            }
        }

        public class Builder : FactoryBuilder<BuffBase, ActionStatType, int, string>
        {
            public override BuffBase Build() => new AddPower(value, value2, value3);
        }
    }
}