using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

namespace BlockAction
{
    [CreateAssetMenu(menuName = "Game/BlockAction/Heal")]
    public class Heal : BlockActionBase
    {
        [SerializeField] int damage;
        protected virtual int Damage => damage;

        public override string GetDescription() => $"Heal {Damage} health";

        public override void HandleMatch(BtBlock parent, BtLineClearInfo info)
        {
            var bullet = MakeBullet(parent)
                            .SetTarget(CombatArena.current.player)
                            .SetDamage(-Damage)
                            .SetLaunchDelay(0.2f);
        }
    }
}
