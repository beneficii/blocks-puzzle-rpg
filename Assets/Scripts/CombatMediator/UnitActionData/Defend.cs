using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

namespace EnemyAction
{
    [CreateAssetMenu(menuName = "Game/UnitAction/Defend")]
    public class Defend : UnitActionBase
    {
        public override IEnumerator Execute(Unit parent, Unit target)
        {
            throw new System.NotImplementedException();
        }

        public override string GetDescription(Unit parent)
        {
            throw new System.NotImplementedException();
        }

        public override string GetShortDescription(Unit parent)
        {
            throw new System.NotImplementedException();
        }
    }
}
