using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAction
{
    [CreateAssetMenu(menuName = "Game/UnitAction/Defend")]
    public class Defend : UnitActionBase
    {
        public override IEnumerator Execute(Unit parent, Unit target)
        {
            if (!target) yield break;

            parent.AnimAttack(2);
            parent.AddArmor(parent.data.defense);
        }

        public override string GetDescription(Unit parent) => $"Will Gain {parent.data.defense} armor";
        public override string GetShortDescription(Unit parent) => $"{parent.data.defense}";
    }
}
