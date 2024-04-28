using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

namespace EnemyAction
{
    [CreateAssetMenu(menuName = "Game/UnitAction/SpawnBlock")]
    public class SpawnBlock : UnitActionBase
    {
        [SerializeField] int count;
        [SerializeField] BtBlockData data;

        BtBlockData GetData(Unit parent)
        {
            var data = this.data;
            if (!data) data = parent.data.specialBlockData;
            return data;
        }

        public override IEnumerator Execute(Unit parent, Unit target)
        {
            if (!target) yield break;

            var emptyBlocks = BtGrid.current.GetEmptyBlocks()
                .ToList()
                .RandN(count);

            if (emptyBlocks.Count == 0) yield break;
            parent.AnimAttack(2);
            var data = GetData(parent);

            foreach (var item in emptyBlocks)
            {
                MakeBullet(parent)
                    .SetTarget(item)
                    .SetSprite(data.sprite)
                    .SetAction((comp) =>
                    {
                        if (!comp.TryGetComponent<BtBlock>(out var block)) return;
                        block.Init(data);
                    })
                    .SetLaunchDelay(0.1f);
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.2f);
        }

        public override string GetDescription(Unit parent) => $"Will spawn {count} '{GetData(parent).name}' on empty block";
        public override string GetShortDescription(Unit parent) => $"{count}";
    }
}