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
            parent.AnimAttack(1);
            yield return new WaitForSeconds(0.1f);
            if (!target || !parent) yield break;

            var damage = parent.data.damage;
            var fxAttack = parent.data.fxAttack;
            if (fxAttack)
            {
                Instantiate(fxAttack, target.transform.position, Quaternion.identity)
                    .SetTriggerAction(() =>
                    {
                        if (!target) return;
                        target.RemoveHp(damage);
                    });
            }
            else
            {
                target.RemoveHp(damage);
            }
        }

        public override string GetDescription(Unit parent) => $"Will Deal {parent.data.damage} damage";
        public override string GetShortDescription(Unit parent) => $"{parent.data.damage}";
    }
}
