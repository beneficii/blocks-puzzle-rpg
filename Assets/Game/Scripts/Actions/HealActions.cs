using FancyToolkit;
using System.Collections;
using UnityEngine;

namespace GameActions
{
    public class Heal : ActionBase
    {
        int value;
        public override string GetDescription()
            => $"Restore {value} hp";

        public Heal(int value)
        {
            this.value = value;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            MakeBullet(parent)
                .SetTarget(CombatArena.current.player)
                .SetHealing(value * multiplier)
                .SetLaunchDelay(0.2f);

            yield return new WaitForSeconds(.2f);
        }

        public class Builder : FactoryBuilder<ActionBase, int>
        {
            public override ActionBase Build() => new Heal(value);
        }
    }

    public class HealEnemy : ActionBase
    {
        int value;
        public override string GetDescription()
            => $"Restore {value} hp to the enemy";

        public HealEnemy(int value)
        {
            this.value = value;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var bullet = MakeBullet(parent)
                            .SetTarget(CombatArena.current.enemy)
                            .SetHealing(value * multiplier)
                            .SetLaunchDelay(0.2f);

            yield return new WaitForSeconds(.2f);
        }

        public class Builder : FactoryBuilder<ActionBase, int>
        {
            public override ActionBase Build() => new HealEnemy(value);
        }
    }
}