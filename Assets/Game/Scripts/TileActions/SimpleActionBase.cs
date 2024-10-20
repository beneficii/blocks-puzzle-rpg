using FancyToolkit;
using GridBoard;
using System.Collections;
using UnityEngine;

namespace TileActions
{
    public class Damage : TileActionBase
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

        public class Builder : FactoryBuilder<TileActionBase>
        {
            public override TileActionBase Build() => new Damage();
        }
    }

    public class DamagePlayer : TileActionBase
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

        public class Builder : FactoryBuilder<TileActionBase>
        {
            public override TileActionBase Build() => new DamagePlayer();
        }
    }

    
    public class BuffPowerAround : TileActionBase
    {
        int value;
        TileStatType type;
        public override string GetDescription(MyTile parent)
            => $"Add {value} {type} to surrounding tiles";

        public BuffPowerAround(int value, TileStatType type)
        {
            this.value = value;
            this.type = type;
        }

        public override IEnumerator Run()
        {
            foreach (var item in parent.board.GetTilesAround(parent.position.x, parent.position.y))
            {
                if (item is not MyTile tile 
                    || tile.myData == null
                    || tile.myData.powerType != type) continue;

                tile.Power += value;
                yield return new WaitForSeconds(.1f);

            }

            yield return new WaitForSeconds(.2f);
        }

        public class Builder : FactoryBuilder<TileActionBase, int, TileStatType>
        {
            public override TileActionBase Build() => new BuffPowerAround(value1, value2);
        }
    }
}