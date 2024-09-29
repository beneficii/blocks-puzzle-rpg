using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;
using GridBoard;

namespace EnemyAction
{
    [CreateAssetMenu(menuName = "Game/UnitAction/SpawnBlock")]
    public class SpawnBlock : UnitActionBase
    {
        [SerializeField] int count;
        [SerializeField] string blockId;

        public override string GetTooltip(Unit parent)
        {
            var data = GetData(parent);
            var descr = data.GetDescription();
            if (string.IsNullOrEmpty(descr)) return "";
            return $"{data.title}: {data.GetDescription()}";
        }

        string GetId(Unit parent)
        {
            var blockId = this.blockId;
            if (string.IsNullOrWhiteSpace(blockId)) blockId = parent.data.specialBlockId;
            return blockId;
        }

        TileData GetData(Unit parent)
            => TileCtrl.current.GetTile(GetId(parent));

        public override IEnumerator Execute(Unit parent, Unit target)
        {
            if (!target) yield break;

            var emptyBlocks = FindAnyObjectByType<Board>().GetEmptyTiles()
                .ToList()
                .RandN(count);

            if (emptyBlocks.Count == 0) yield break;
            parent.AnimAttack(2);
            var data = GetData(parent);

            yield return new WaitForSeconds(0.1f);
            foreach (var item in emptyBlocks)
            {
                MakeBullet(parent)
                    .SetTarget(item)
                    .SetSprite(data.visuals.sprite)
                    .SetAction((comp) =>
                    {
                        if (!comp.TryGetComponent<Tile>(out var block)) return;
                        block.Init(data);
                    })
                    .SetLaunchDelay(0.1f);
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.2f);
        }

        public override string GetDescription(Unit parent) => $"Will spawn {count} '{GetData(parent).title}' on empty block";
        public override string GetShortDescription(Unit parent) => $"";
    }
}