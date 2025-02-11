using FancyToolkit;
using System.Collections;
using UnityEngine;

namespace GameActions
{
    public class Defense : ActionBase
    {
        public override string GetDescription()
            => $"Gain X ({parent.Power}) armor";

        public override ActionStatType StatType => ActionStatType.Defense;

        public override IEnumerator Run(int multiplier = 1)
        {
            SetBulletDefense(MakeBullet(parent)
                            .SetTarget(CombatArena.current.player)
                            .SetLaunchDelay(0.05f)
                            , parent.Power * multiplier);
            yield return new WaitForSeconds(.07f);
        }

        public class Builder : FactoryBuilder<ActionBase>
        {
            public override ActionBase Build() => new Defense();
        }
    }

    public class DefenseN : ActionBase
    {
        int value;
        public override string GetDescription()
            => $"Gain {value} armor";

        public DefenseN(int value)
        {
            this.value = value;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            SetBulletDefense(MakeBullet(parent)
                            .SetTarget(CombatArena.current.player)
                            .SetLaunchDelay(0.05f)
                            , value * multiplier);
            yield return new WaitForSeconds(.07f);
        }

        public class Builder : FactoryBuilder<ActionBase, int>
        {
            public override ActionBase Build() => new DefenseN(value);
        }
    }

    public class DefenseAnd : ActionBaseWithNested
    {
        public override string GetDescription()
            => $"Gain X ({parent.Power}) armor and {nestedAction.GetDescription()}";

        public override ActionStatType StatType => ActionStatType.Defense;

        public DefenseAnd(ActionBase nestedAction)
        {
            this.nestedAction = nestedAction;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            SetBulletDefense(MakeBullet(parent)
                        .SetTarget(CombatArena.current.player)
                        .SetLaunchDelay(0.2f)
                        , parent.Power * multiplier);

            yield return new WaitForSeconds(.1f);
            yield return nestedAction.Run(multiplier);
        }

        public class Builder : FactoryBuilder<ActionBase, FactoryBuilder<ActionBase>>
        {
            public override ActionBase Build() => new DefenseAnd(value.Build());
        }
    }

    public class EnemyDefense : ActionBase
    {
        public override string GetDescription()
            => $"Enemy gains X ({parent.Power}) armor";

        public override ActionStatType StatType => ActionStatType.Defense;

        public EnemyDefense()
        {
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            SetBulletDefense(MakeBullet(parent)
                            .SetTarget(CombatArena.current.enemy)
                            .SetLaunchDelay(0.05f)
                            , parent.Power * multiplier);
            yield return new WaitForSeconds(.1f);
        }

        public class Builder : FactoryBuilder<ActionBase>
        {
            public override ActionBase Build() => new EnemyDefense();
        }
    }

    public class DefenseBoth : ActionBase
    {
        public override string GetDescription()
            => $"You and enemy gain X ({parent.Power}) armor";

        public override ActionStatType StatType => ActionStatType.Defense;

        public override IEnumerator Run(int multiplier = 1)
        {
            SetBulletDefense(MakeBullet(parent)
                            .SetTarget(CombatArena.current.player)
                            .SetLaunchDelay(0.05f)
                            , parent.Power * multiplier);

            SetBulletDefense(MakeBullet(parent)
                            .SetTarget(CombatArena.current.enemy)
                            .SetLaunchDelay(0.05f)
                            , parent.Power * multiplier);
            yield return new WaitForSeconds(.07f);
        }

        public class Builder : FactoryBuilder<ActionBase>
        {
            public override ActionBase Build() => new Defense();
        }
    }

    public class RemoveArmor : ActionBase
    {
        int value;
        public override string GetDescription()
            => $"Remove {value} armor";

        public RemoveArmor(int value)
        {
            this.value = value;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var bullet = MakeBullet(parent)
                            .SetTarget(CombatArena.current.player)
                            .SetUnitAction((x) => x.RemoveArmor(value * multiplier))
                            .SetLaunchDelay(0.09f);
            yield return new WaitForSeconds(.07f);
        }

        public class Builder : FactoryBuilder<ActionBase, int>
        {
            public override ActionBase Build() => new RemoveArmor(value);
        }
    }

    public class RemoveArmorEnemy : ActionBase
    {
        int value;
        public override string GetDescription()
            => $"Remove {value} armor from enemy";

        public RemoveArmorEnemy(int value)
        {
            this.value = value;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var bullet = MakeBullet(parent)
                            .SetTarget(CombatArena.current.enemy)
                            .SetUnitAction((x) => x.RemoveArmor(value * multiplier))
                            .SetLaunchDelay(0.09f);
            yield return new WaitForSeconds(.07f);
        }

        public class Builder : FactoryBuilder<ActionBase, int>
        {
            public override ActionBase Build() => new RemoveArmorEnemy(value);
        }
    }
}