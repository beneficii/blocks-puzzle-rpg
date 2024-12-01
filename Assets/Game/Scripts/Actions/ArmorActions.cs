using FancyToolkit;
using System.Collections;
using UnityEngine;

namespace GameActions
{
    public class Defense : ActionBase
    {
        public override string GetDescription()
            => $"Gain {parent.Defense} armor";

        public override ActionStatType StatType => ActionStatType.Defense;

        public override IEnumerator Run(int multiplier = 1)
        {
            SetBulletDefense(MakeBullet(parent)
                            .SetTarget(CombatArena.current.player)
                            .SetLaunchDelay(0.05f)
                            , parent.Defense * multiplier);
            yield return new WaitForSeconds(.07f);
        }

        public class Builder : FactoryBuilder<ActionBase>
        {
            public override ActionBase Build() => new Defense();
        }
    }

    public class DefenseAnd : ActionBase
    {
        ActionBase nestedAction;

        public override string GetDescription()
            => $"Gain {parent.Defense} armor and {nestedAction.GetDescription()}";

        public override ActionStatType StatType => ActionStatType.Defense;

        public DefenseAnd(ActionBase nestedAction)
        {
            this.nestedAction = nestedAction;
        }

        public override void Init(IActionParent parent)
        {
            base.Init(parent);
            nestedAction.Init(parent);
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            SetBulletDefense(MakeBullet(parent)
                        .SetTarget(CombatArena.current.player)
                        .SetLaunchDelay(0.2f)
                        , parent.Defense * multiplier);

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
            => $"Enemy gains {parent.Defense} armor";

        public override ActionStatType StatType => ActionStatType.Defense;

        public EnemyDefense()
        {
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            SetBulletDefense(MakeBullet(parent)
                            .SetTarget(CombatArena.current.enemy)
                            .SetLaunchDelay(0.05f)
                            , parent.Defense * multiplier);
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
            => $"You and enemy gain {parent.Defense} armor";

        public override ActionStatType StatType => ActionStatType.Defense;

        public override IEnumerator Run(int multiplier = 1)
        {
            SetBulletDefense(MakeBullet(parent)
                            .SetTarget(CombatArena.current.player)
                            .SetLaunchDelay(0.05f)
                            , parent.Defense * multiplier);

            SetBulletDefense(MakeBullet(parent)
                            .SetTarget(CombatArena.current.enemy)
                            .SetLaunchDelay(0.05f)
                            , parent.Defense * multiplier);
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