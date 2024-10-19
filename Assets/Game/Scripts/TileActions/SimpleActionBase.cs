using ClearAction;
using FancyToolkit;
using GridBoard;
using System.Collections;
using UnityEngine;

namespace SimpleAction
{
    public abstract class Base : TileActions.Base
    {
        public abstract IEnumerator Run();
    }


    public class Damage : Base
    {
        public override string GetDescription(MyTile parent)
            => $"Deal {Power} damage";

        public Damage()
        {
        }

        public override IEnumerator Run()
        {
            var bullet = MakeBullet(parent)
                            .SetTarget(CombatArena.current.enemy)
                            .SetDamage(Power)
                            .SetLaunchDelay(0.2f);

            yield return new WaitForSeconds(.2f);
        }

        public class Builder : FactoryBuilder<Base>
        {
            public override Base Build() => new Damage();
        }
    }

    public class DamagePlayer : Base
    {
        public override string GetDescription(MyTile parent)
            => $"Deal {Power} damage to the player";

        public DamagePlayer()
        {
        }

        public override IEnumerator Run()
        {
            var bullet = MakeBullet(parent)
                            .SetTarget(CombatArena.current.player)
                            .SetDamage(Power)
                            .SetLaunchDelay(0.2f);

            yield return new WaitForSeconds(.2f);
        }

        public class Builder : FactoryBuilder<Base>
        {
            public override Base Build()
            {
                return new DamagePlayer();
            }
        }
    }
}