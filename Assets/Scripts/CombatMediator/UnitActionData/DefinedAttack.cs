using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAction
{
    [CreateAssetMenu(menuName = "Game/UnitAction/DefinedAttack")]
    public class DefinedAttack : UnitActionBase
    {
        [SerializeField] int damage;
        [SerializeField] bool isBig;
        public override IEnumerator Execute(Unit parent, Unit target)
        {
            if (!target) yield break;

            parent.AnimAttack(isBig?2:1);
            yield return new WaitForSeconds(0.3f);
            target.RemoveHp(damage);
        }

        public override string GetDescription(Unit parent) => $"Will Deal {damage} damage";
        public override string GetShortDescription(Unit parent) => $"{damage}";
    }
}
