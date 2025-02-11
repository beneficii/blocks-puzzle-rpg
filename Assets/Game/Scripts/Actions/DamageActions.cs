using FancyToolkit;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace GameActions
{
    public class Damage : ActionBase
    {
        public override string GetDescription()
            => $"Deal X ({parent.Power}) damage";

        public override ActionStatType StatType => ActionStatType.Damage;

        public Damage()
        {
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            SetBulletDamage(MakeBullet(parent)
                        .SetTarget(CombatArena.current.enemy)
                        .SetLaunchDelay(0.2f)
                        , parent.Power * multiplier);

            yield return new WaitForSeconds(.2f);
        }

        public class Builder : FactoryBuilder<ActionBase>
        {
            public override ActionBase Build() => new Damage();
        }
    }

    public class DamageN : ActionBase
    {
        int value;
        public override string GetDescription()
            => $"Deal {value} damage";

        public DamageN(int value)
        {
            this.value = value;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            SetBulletDamage(MakeBullet(parent)
                        .SetTarget(CombatArena.current.enemy)
                        .SetLaunchDelay(0.2f)
                        , value * multiplier);
            yield return new WaitForSeconds(.07f);
        }

        public class Builder : FactoryBuilder<ActionBase, int>
        {
            public override ActionBase Build() => new DamageN(value);
        }
    }

    public class DamageAnd : ActionBaseWithNested
    {
        public override string GetDescription()
            => $"Deal X ({parent.Power}) damage and {nestedAction.GetDescription()}";

        public override ActionStatType StatType => ActionStatType.Damage;

        public DamageAnd(ActionBase nestedAction)
        {
            this.nestedAction = nestedAction;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            SetBulletDamage(MakeBullet(parent)
                        .SetTarget(CombatArena.current.enemy)
                        .SetLaunchDelay(0.2f)
                        , parent.Power * multiplier);

            yield return new WaitForSeconds(.1f);
            yield return nestedAction.Run(multiplier);
        }

        public class Builder : FactoryBuilder<ActionBase, FactoryBuilder<ActionBase>>
        {
            public override ActionBase Build() => new DamageAnd(value.Build());
        }
    }

    public class DamagePlayer : ActionBase
    {
        public override string GetDescription()
            => $"Deal X ({parent.Power}) damage to the player";

        public override ActionStatType StatType => ActionStatType.Damage;

        public override IEnumerator Run(int multiplier = 1)
        {
            SetBulletDamage(MakeBullet(parent)
                        .SetTarget(CombatArena.current.player)
                        .SetLaunchDelay(0.2f)
                        , parent.Power * multiplier);

            yield return new WaitForSeconds(.2f);
        }

        public class Builder : FactoryBuilder<ActionBase>
        {
            public override ActionBase Build() => new DamagePlayer();
        }
    }

    public class DamageBoth : ActionBase
    {
        public override string GetDescription()
            => $"Deal X ({parent.Power}) damage to both player and enemy";

        public override ActionStatType StatType => ActionStatType.Damage;

        public override IEnumerator Run(int multiplier = 1)
        {
            SetBulletDamage(MakeBullet(parent)
                        .SetTarget(CombatArena.current.player)
                        .SetLaunchDelay(0.2f)
                        , parent.Power * multiplier);

            SetBulletDamage(MakeBullet(parent)
                        .SetTarget(CombatArena.current.enemy)
                        .SetLaunchDelay(0.2f)
                        , parent.Power * multiplier);

            yield return new WaitForSeconds(.2f);
        }

        public class Builder : FactoryBuilder<ActionBase>
        {
            public override ActionBase Build() => new DamageBoth();
        }
    }
}
    
