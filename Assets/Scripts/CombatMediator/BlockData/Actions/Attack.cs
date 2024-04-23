using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockAction
{
    [CreateAssetMenu(menuName = "Game/BlockAction/Attack")]
    public class Attack : BlockActionBase
    {
        [SerializeField] int damage;
        protected virtual int Damage => damage;

        public override string GetDescription() => $"Deal {Damage} damage";

        public override void HandleMatch(BtBlock parent, BtLineClearInfo info)
        {
            MakeBullet(parent)
                .SetTarget(CombatArena.current.enemy)
                .SetDamage(Damage)
                .SetLaunchDelay(0.2f);
        }
    }
}
