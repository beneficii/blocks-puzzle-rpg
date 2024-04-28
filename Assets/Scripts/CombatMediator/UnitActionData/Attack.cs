using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAction
{
    [CreateAssetMenu(menuName = "Game/UnitAction/Attack")]
    public class Attack : UnitActionBase
    {
        public override IEnumerator Execute(Unit parent, Unit target)
        {
            if (!target) yield break;

            parent.AnimAttack(1);
            yield return new WaitForSeconds(0.3f);
            target.RemoveHp(parent.data.damage);
        }

        public override string GetDescription(Unit parent) => $"Will Deal {parent.data.damage} damage";
        public override string GetShortDescription(Unit parent) => $"{parent.data.damage}";
    }
}
