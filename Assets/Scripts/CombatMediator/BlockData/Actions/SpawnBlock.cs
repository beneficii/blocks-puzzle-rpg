using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

namespace BlockAction
{
    [CreateAssetMenu(menuName = "Game/BlockAction/SpawnBlock")]
    public class SpawnBlock : BlockActionBase
    {
        [SerializeField] int count;
        [SerializeField] BtBlockData data;

        public override string GetTooltip()
        {
            var descr = data.GetDescription();
            if (string.IsNullOrEmpty(descr)) return "";
            return $"{data.title}: {data.GetDescription()}";
        }

        public override string GetDescription() => $"Spawns {count} '{data.title}' on empty block";

        public void Spawn(Component comp)
        {
            if (!comp.TryGetComponent<BtBlock>(out var block)) return;

            block.Init(data);
        }

        public override void HandleMatch(BtBlock parent, BtLineClearInfo info)
        {
            for (int i = 0; i < count; i++)
            {
                if (!info.boardEmptyBlocks.TryDequeue(out var block)) return;

                MakeBullet(parent)
                    .SetTarget(block)
                    .SetSprite(data.sprite)
                    .SetAction(Spawn)
                    .SetLaunchDelay(0.2f);
            }
            
        }
    }
}
