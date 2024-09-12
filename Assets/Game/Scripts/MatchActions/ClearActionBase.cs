using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using FancyToolkit;

namespace ClearAction
{
    public abstract class Base
    {
        public abstract string GetDescription(MyTile parent);
        public abstract void Run(MyTile parent, LineClearData match);

        protected GenericBullet MakeBullet(MyTile parent, AnimCompanion fxPrefab = null)
        {
            var rand = Random.Range(0, 2) == 0;
            var bullet = DataManager.current.gameData.prefabBullet.MakeInstance(parent.transform.position)
                .AddSpleen(rand ? Vector2.left : Vector2.right)
                .SetSprite(parent.GetIcon());


            if (fxPrefab) bullet.SetFx(fxPrefab);

            return bullet;
        }
    }


    public class Damage : Base
    {
        int damage;
        public override string GetDescription(MyTile parent)
            => $"Deal {damage} damage";

        public Damage(int damage)
        {
            this.damage = damage;   
        }

        public override void Run(MyTile parent, LineClearData match)
        {
            var bullet = MakeBullet(parent)
                            .SetTarget(CombatArena.current.enemy)
                            .SetDamage(damage)
                            .SetLaunchDelay(0.2f);
        }

        public class Builder : FactoryBuilder<Base>
        {
            int damage;
            public override Base Build()
            {
                return new Damage(damage);
            }

            public override void Init(StringScanner scanner)
            {
                damage = scanner.NextInt();
            }
        }
    }

    public class Defense : Base
    {
        int value;
        public override string GetDescription(MyTile parent)
            => $"Gain {value} defense";

        public Defense(int value)
        {
            this.value = value;
        }

        void Action(Component comp)
        {
            if (!comp || comp is not Unit unit) return;

            unit.AddArmor(value);
        }

        public override void Run(MyTile parent, LineClearData match)
        {
            var bullet = MakeBullet(parent)
                            .SetTarget(CombatArena.current.player)
                            .SetAction(Action)
                            .SetLaunchDelay(0.2f);
        }

        public class Builder : FactoryBuilder<Base>
        {
            int value;
            public override Base Build()
            {
                return new Defense(value);
            }

            public override void Init(StringScanner scanner)
            {
                value = scanner.NextInt();
            }
        }
    }
}