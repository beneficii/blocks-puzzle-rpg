using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatBlock
{
    [CreateAssetMenu(menuName = "Game/Blocks/Heal")]
    public class Heal : Base
    {
        public int damage;
        public virtual int Damage => damage;

        public override string GetDescription() => $"Heals {Damage} health";

        public override void HandleMatch(BtBlock parent, BtLineClearInfo info)
        {
            MakeBullet(parent.transform.position)
                .SetTarget(Arena.player)
                .SetDamage(-Damage)
                .SetLaunchDelay(0.2f);
        }
    }
}
