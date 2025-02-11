using FancyToolkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GridBoard;

namespace GameActions
{
    public abstract class PassiveEffect : ActionBaseWithNested
    {
        public override IEnumerator Run(int multiplier = 1)
        {
            yield break;
        }
    }

    public class OnPlayerDamage : PassiveEffect
    {
        int amount;

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

        public OnPlayerDamage(int amount, ActionBase nestedAction)
        {
            this.amount = amount;
            this.nestedAction = nestedAction;
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


        public class Builder : FactoryBuilder<ActionBase, int, FactoryBuilder<ActionBase>>
        {
            public override ActionBase Build() => new OnPlayerDamage(value, value2.Build());
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

        public override string GetDescription()
        {
            return $"When {tag} tile is placed, {nestedAction.GetDescription()}";
        }

        public OnTilePlaced(string tag, ActionBase nestedAction)
        {
            this.tag = tag;
            this.nestedAction = nestedAction;
        }

        void HandleTilePlaced(Tile tile)
        {
            if (!tile.HasTag(tag)) return;

            Game.current.StartCoroutine(nestedAction.Run());
        }


        public class Builder : FactoryBuilder<ActionBase, string, FactoryBuilder<ActionBase>>
        {
            public override ActionBase Build() => new OnTilePlaced(value, value2.Build());
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

    public class ChangeSummon : PassiveEffect
    {
        string tag;
        TileData data;

        public override string GetDescription()
        {
            return $"Whenever you spawn {tag}, change it to '{data.title}'";
        }

        public ChangeSummon(string tag, string id)
        {
            this.tag = tag;
            this.data = TileCtrl.current.Get(id);
        }

        void HandleSummon(SummonTileInfo summonInfo, MyTile target)
        {
            if (summonInfo.data.HasTag(tag))
            {
                summonInfo.data = data;
            }
        }

        public class Builder : FactoryBuilder<ActionBase, string, string>
        {
            public override ActionBase Build() => new ChangeSummon(value, value2);
        }

        protected override void Add()
        {
            SummonTileInfo.OnApplied += HandleSummon;
        }

        protected override void Remove()
        {
            SummonTileInfo.OnApplied -= HandleSummon;
        }
    }

    public class IfEnoughOnBoard : ActionBaseWithNested
    {
        int amount;
        string id;

        public override string GetDescription()
        {
            return $"{nestedAction.GetDescription()} if there are at least {amount} '{TileCtrl.current.Get(id).title}' on board";
        }

        public IfEnoughOnBoard(string id, int amount, ActionBase nestedAction)
        {
            this.id = id;
            this.amount = amount;
            this.nestedAction = nestedAction;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            if (parent.board.GetIdTileCount(id) < amount) yield break;

            if (parent is MyTile tile && tile.data.id == id)
            {
                foreach (var item in MyTile.FindTileTargets(parent, TileTargetingType.All, (x) => x.data.id == id))
                {
                    item.isActionLocked = true;
                }
            }

            yield return nestedAction.Run(multiplier);
        }

        public class Builder : FactoryBuilder<ActionBase, string, int, FactoryBuilder<ActionBase>>
        {
            public override ActionBase Build() => new IfEnoughOnBoard(value, value2, value3.Build());
        }
    }


    public class AddPowerPassiveTo : PassiveEffect
    {
        string tag;
        int amount;

        public override string GetDescription()
            => $"All {tag} tiles have {amount.SignedStr()} Power";

        public AddPowerPassiveTo(string tag, int amount)
        {
            this.tag = tag;
            this.amount = amount;
        }

        bool IsMatching(Tile tile) => tile is MyTile myTile && myTile.HasTag(tag);

        IEnumerable<MyTile> GetTargets()
        {
            foreach (var item in parent.board.GetAllTiles().Where(x => IsMatching(x)))
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

        public class Builder : FactoryBuilder<ActionBase, string, int>
        {
            public override ActionBase Build() => new AddPowerPassiveTo(value, value2);
        }
    }

    public class AddEnemyAttackPassive : PassiveEffect
    {
        int amount;

        public override string GetDescription()
            => $"Enemies have {amount.SignedStr()} damage";

        public AddEnemyAttackPassive(int amount)
        {
            this.amount = amount;
        }

        protected override void Add()
        {
            var enemy = CombatArena.current?.enemy;
            if (!enemy) return;
            enemy.damage += amount;
            enemy.RefreshAction();
        }

        protected override void Remove()
        {
            var enemy = CombatArena.current?.enemy;
            if (!enemy) return;
            enemy.damage -= amount;
            enemy.RefreshAction();
        }


        public class Builder : FactoryBuilder<ActionBase, int>
        {
            public override ActionBase Build() => new AddEnemyAttackPassive(value);
        }
    }

    public class AddEnemyDefensePassive : PassiveEffect
    {
        int amount;

        public override string GetDescription()
            => $"Enemies have {amount.SignedStr()} defense";

        public AddEnemyDefensePassive(int amount)
        {
            this.amount = amount;
        }

        protected override void Add()
        {
            var enemy = CombatArena.current?.enemy;
            if (!enemy) return;
            enemy.defense += amount;
            enemy.RefreshAction();
        }

        protected override void Remove()
        {
            var enemy = CombatArena.current?.enemy;
            if (!enemy) return;
            enemy.defense -= amount;
            enemy.RefreshAction();
        }

        public class Builder : FactoryBuilder<ActionBase, int>
        {
            public override ActionBase Build() => new AddEnemyDefensePassive(value);
        }
    }
}