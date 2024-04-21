using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatBlock
{
    [CreateAssetMenu(menuName = "Game/Blocks/DamageSpell")]
    public class DamageSpell : Base
    {
        [SerializeField] int damageMultiplier = 4;

        public override string GetDescription() => $"Deal {damageMultiplier}x sword matched";

        public override void HandleMatch(BtBlock parent, BtLineClearInfo info)
        {
            int totalDamage = 0;
            var captured = new List<BtBlock>();
            foreach (var item in info.blocks)
            {
                if (item.data is not Base data) continue;

                if (data.HasTag(BlockTag.Sword))
                {
                    captured.Add(item);
                    totalDamage += damageMultiplier;

                    MakeBullet(item.transform.position)
                        .SetSprite(item.data.sprite)
                        .SetTarget(parent)
                        .SetLaunchDelay(0.1f);
                }
            }

            foreach (var item in captured)
            {
                info.blocks.Remove(item);
            }

            MakeBullet(parent.transform.position)
                .SetTarget(Arena.enemy)
                .SetDamage(totalDamage)
                .SetLaunchDelay(0.5f);
        }
    }
}
