using FancyToolkit;
using System.Collections;
using UnityEngine;
using GridBoard;
using System;

namespace TileActions
{
    public abstract class PassiveEffect : TileActionBase
    {
        public override IEnumerator Run(int multiplier = 1)
        {
            yield break;
        }
    }

    public class OnPlayerDamage : PassiveEffect
    {
        int amount;
        TileActionBase nestedAction;
        public override string GetDescription()
        {
            if (amount == 0)
            {
                return $"{nestedAction.GetDescription()} every time you receive damage";
            }
            else if (amount == 1)
            {
                return $"For each received point of damage {nestedAction.GetDescription()}";
            }
            else
            {
                return $"For each {amount} received points of damage {nestedAction.GetDescription()}";
            }
        }

        public OnPlayerDamage(int amount, TileActionBase nestedAction)
        {
            this.amount = amount;
            this.nestedAction = nestedAction;
        }

        public override void Init(MyTile tile)
        {
            base.Init(tile);
            nestedAction.Init(tile);
        }

        void HandlePlayerDamage(Unit unit, int damage)
        {
            if (unit != CombatArena.current.player) return;
            if (unit.health.Value <= 0) return;
            int count = (amount == 0) ? 1 : damage / amount;

            if (count > 0)
            {
                unit.StartCoroutine(nestedAction.Run(count));
            }
        }


        public class Builder : FactoryBuilder<TileActionBase, int, FactoryBuilder<TileActionBase>>
        {
            public override TileActionBase Build() => new OnPlayerDamage(value, value2.Build());
        }

        protected override void Add()
        {
            Unit.OnReceiveDamage += HandlePlayerDamage;
        }

        protected override void Remove()
        {
            Unit.OnReceiveDamage -= HandlePlayerDamage;
        }
    }

    public class OnTilePlaced : PassiveEffect
    {
        string tag;
        TileActionBase nestedAction;
        public override string GetDescription()
        {
            return $"When {tag} tile is placed, {nestedAction.GetDescription()}";
        }

        public OnTilePlaced(string tag, TileActionBase nestedAction)
        {
            this.tag = tag;
            this.nestedAction = nestedAction;
        }

        public override void Init(MyTile tile)
        {
            base.Init(tile);
            nestedAction.Init(tile);
        }

        void HandleTilePlaced(Tile tile)
        {
            if (!tile.HasTag(tag)) return;

            Game.current.StartCoroutine(nestedAction.Run());
        }


        public class Builder : FactoryBuilder<TileActionBase, string, FactoryBuilder<TileActionBase>>
        {
            public override TileActionBase Build() => new OnTilePlaced(value, value2.Build());
        }

        protected override void Add()
        {
            Tile.OnPlaced += HandleTilePlaced;
        }

        protected override void Remove()
        {
            Tile.OnPlaced -= HandleTilePlaced;
        }
    }

    public class IfEnoughOnBoard : TileActionBase
    {
        int amount;
        TileActionBase nestedAction;
        public override string GetDescription()
        {
            return $"{nestedAction.GetDescription()} if there are at least {amount} '{parent.name}' on board";
        }

        public IfEnoughOnBoard(int amount, TileActionBase nestedAction)
        {
            this.amount = amount;
            this.nestedAction = nestedAction;
        }

        public override void Init(MyTile tile)
        {
            base.Init(tile);
            nestedAction.Init(tile);
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            if (parent.board.dictTileCounter.Get(parent.data.id) < amount) yield break;

            foreach (var item in FindTileTargets(parent, ActionTargetType.All, (x)=>x.data.id == parent.data.id))
            {
                item.isActionLocked = true;
            }

            yield return nestedAction.Run(multiplier);
        }

        public class Builder : FactoryBuilder<TileActionBase, int, FactoryBuilder<TileActionBase>>
        {
            public override TileActionBase Build() => new IfEnoughOnBoard(value, value2.Build());
        }

        
    }
}
