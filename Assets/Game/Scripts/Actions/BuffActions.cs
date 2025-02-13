using FancyToolkit;
using System.Collections;
using UnityEngine;

namespace GameActions
{
    public class BuffPower : ActionBase
    {
        TileTargetingType targetType;
        int value;
        string tag;

        public override string GetDescription()
            => $"{value.SignedStr()} Power to {MyTile.GetTargetingTypeName(targetType, tag)}";

        public BuffPower(TileTargetingType targetType, int value, string tag)
        {
            this.targetType = targetType;
            this.value = value;
            this.tag = tag;
        }

        public bool Filter(MyTile target)
        {
            return target.HasTag(tag);
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            foreach (var tile in MyTile.FindTileTargets(parent, targetType, Filter))
            {
                MakeBullet(parent)
                    .SetTarget(tile)
                    .SetSpleen(default)
                    .SetAudio(AudioCtrl.current?.clipPop)
                    .SetTileAction(x => x.Power += value * multiplier);

                yield return new WaitForSeconds(.1f);
            }
        }

        public class Builder : FactoryBuilder<ActionBase, TileTargetingType, int, string>
        {
            public override ActionBase Build() => new BuffPower(value, value2, value3);
        }
    }

    public class AddPower : ActionBase
    {
        protected int value;

        public override string GetDescription()
            => $"{value.SignedStr()} Power to this tile";

        protected AddPower(int value)
        {
            this.value = value;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            parent.Power += value * multiplier;
            yield return new WaitForSeconds(.15f);
        }

        public class Builder : FactoryBuilder<ActionBase, int>
        {
            public override ActionBase Build() => new AddPower(value);
        }
    }

    public class AddPowerForEachOnBoard : TileActionBase
    {
        int value;
        string tag;

        public override string GetDescription()
            => $"Add {value.SignedStr()} Power to this tile for each {tag} on board";

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
                    .SetAudio(AudioCtrl.current?.clipPop)
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
            => $"Multiply Power of {MyTile.GetTargetingTypeName(targetType, tag)} by {value}";

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
                    .SetAudio(AudioCtrl.current?.clipPop)
                    .SetTileAction(x => x.Power *= value * multiplier);

                yield return new WaitForSeconds(.1f);
            }
        }

        public class Builder : FactoryBuilder<ActionBase, TileTargetingType, int, string>
        {
            public override ActionBase Build() => new MultiplyPowerTag(value, value2, value3);
        }
    }

    public class AddBuff : ActionBase
    {
        FactoryBuilder<BuffBase> buffBuilder;
        BuffBase nestedBuff;

        public AddBuff(FactoryBuilder<BuffBase> builder)
        {
            this.buffBuilder = builder;
            this.nestedBuff = builder.Build();
        }

        public override string GetDescription()
            => $"\"{nestedBuff.GetDescription()}\" for the rest of the combat";

        public override IEnumerator Run(int multiplier = 1)
        {
            var player = CombatArena.current.player;
            if (!player) yield break;
            yield return MakeBullet(parent)
                .SetTarget(player)
                .Wait();

            CombatCtrl.current.AddBuff(buffBuilder.Build());
        }

        
        public class Builder : FactoryBuilder<ActionBase, FactoryBuilder<BuffBase>>
        {
            public override ActionBase Build() => new AddBuff(value);
        }
    }
}
