using FancyToolkit;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace GameActions
{
    public class BuffPower : ActionBase
    {
        TileTargetingType targetType;
        int value;
        string tag;
        ActionStatType statType;
        public override string GetDescription()
            => $"Add {value} {statType} to {MyTile.GetTargetingTypeName(targetType, tag)}";

        public BuffPower(TileTargetingType targetType, int value, string tag, ActionStatType statType)
        {
            this.targetType = targetType;
            this.value = value;
            this.tag = tag;
            this.statType = statType;
        }

        public bool Filter(MyTile target)
        {
            return target.StatType == statType && target.HasTag(tag);
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            foreach (var tile in MyTile.FindTileTargets(parent, targetType, Filter))
            {
                MakeBullet(parent)
                    .SetTarget(tile)
                    .SetSpleen(default)
                    .SetTileAction(x => x.Power += value * multiplier);

                yield return new WaitForSeconds(.1f);
            }
        }
    }

    public class BuffDamage : BuffPower
    {
        public BuffDamage(TileTargetingType targetType, int value, string tag) : base(targetType, value, tag, ActionStatType.Damage)
        {
        }

        public class Builder : FactoryBuilder<ActionBase, TileTargetingType, int, string>
        {
            public override ActionBase Build() => new BuffDamage(value, value2, value3);
        }
    }

    public class BuffDefense : BuffPower
    {
        public BuffDefense(TileTargetingType targetType, int value, string tag) : base(targetType, value, tag, ActionStatType.Defense)
        {
        }

        public class Builder : FactoryBuilder<ActionBase, TileTargetingType, int, string>
        {
            public override ActionBase Build() => new BuffDefense(value, value2, value3);
        }
    }

    public abstract class AddPower : ActionBase
    {
        protected int value;
        protected abstract ActionStatType type { get; }
        public override string GetDescription()
            => $"{value.SignedStr()} {type} to this tile";

        protected AddPower(int value)
        {
            this.value = value;
        }
    }

    public class AddDefense : AddPower
    {
        protected override ActionStatType type => ActionStatType.Defense;
        public AddDefense(int value) : base(value)
        {
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            parent.Defense += value;
            yield return new WaitForSeconds(.15f);
        }

        public class Builder : FactoryBuilder<ActionBase, int>
        {
            public override ActionBase Build() => new AddDefense(value);
        }
    }

    public class AddDamage : AddPower
    {
        protected override ActionStatType type => ActionStatType.Damage;
        public AddDamage(int value) : base(value)
        {
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            parent.Damage += value;
            yield return new WaitForSeconds(.15f);
        }

        public class Builder : FactoryBuilder<ActionBase, int>
        {
            public override ActionBase Build() => new AddDamage(value);
        }
    }

    public class AddPowerForEachOnBoard : TileActionBase
    {
        int value;
        string tag;
        public override string GetDescription()
            => $"Add {value.SignedStr()} power to this tile for each {tag} on board";

        public AddPowerForEachOnBoard(int value, string tag)
        {
            this.value = value;
            this.tag = tag;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            bool hadTargets = false;
            foreach (var target in MyTile.FindTileTargets(parent, TileTargetingType.All, tag))
            {
                hadTargets = true;
                MakeBullet(target)
                    .SetTarget(tileParent)
                    //.SetSpleen(default)
                    .SetTileAction(x => {
                        x.Power += value * multiplier;
                    });
                yield return new WaitForSeconds(.05f);
            }
            if (hadTargets) yield return new WaitForSeconds(.1f);
        }

        public class Builder : FactoryBuilder<ActionBase, int, string>
        {
            public override ActionBase Build() => new AddPowerForEachOnBoard(value, value2);
        }
    }

    public class MultiplyPowerTag : ActionBase
    {
        TileTargetingType targetType;
        int value;
        string tag;
        public override string GetDescription()
            => $"Multiply Damage of {MyTile.GetTargetingTypeName(targetType, tag)} by {value}";

        public MultiplyPowerTag(TileTargetingType targetType, int value, string tag)
        {
            this.targetType = targetType;
            this.value = value;
            this.tag = tag;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            foreach (var tile in MyTile.FindTileTargets(parent, targetType, tag))
            {
                MakeBullet(parent)
                    .SetTarget(tile)
                    //.SetSpleen(default)
                    .SetTileAction(x => x.Power *= value * multiplier);

                yield return new WaitForSeconds(.1f);
            }
        }

        public class Builder : FactoryBuilder<ActionBase, TileTargetingType, int, string>
        {
            public override ActionBase Build() => new MultiplyPowerTag(value, value2, value3);
        }
    }
}