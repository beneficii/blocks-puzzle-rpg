using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAction
{
    [CreateAssetMenu(menuName = "Game/UnitAction/DefinedDefend")]
    public class DefinedDefend : UnitActionBase
    {
        [SerializeField] int value;

        public override IEnumerator Execute(Unit parent, Unit target)
        {
            if (!target) yield break;

            parent.AnimAttack(2);
            parent.AddArmor(value);
        }

        public override string GetDescription(Unit parent) => $"Will Gain {value} armor";
        public override string GetShortDescription(Unit parent) => $"{value}";
    }
}
