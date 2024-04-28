using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAction
{
    [CreateAssetMenu(menuName = "Game/UnitAction/Expire")]
    public class Expire : UnitActionBase
    {
        public int turns;

        public override IEnumerator Execute(Unit parent, Unit target)
        {
            if (parent.lifetime < turns) yield break;

            yield return null;
            parent.reward = BtUpgradeRarity.Common;
            parent.RemoveHp(9999);

        }

        public override string GetDescription(Unit parent) => $"Will expire in {turns - parent.lifetime} turns";
        public override string GetShortDescription(Unit parent) => $"{turns - parent.lifetime}";
    }
}
