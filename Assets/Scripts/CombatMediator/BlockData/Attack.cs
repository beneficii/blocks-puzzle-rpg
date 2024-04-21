using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatBlock
{
    [CreateAssetMenu(menuName = "Game/Blocks/Attack")]
    public class Attack : Base
    {
        public int damage;
        public virtual int Damage => damage;

        public override string GetDescription() => $"Deal {Damage} damage to enemy";

        public override void HandleMatch(BtBlock parent, BtLineClearInfo info)
        {
            MakeBullet(parent.transform.position)
                .SetTarget(Arena.enemy)
                .SetDamage(Damage)
                .SetLaunchDelay(0.2f);
        }
    }
}
