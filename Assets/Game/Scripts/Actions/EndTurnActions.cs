using FancyToolkit;
using System.Collections;
using UnityEngine;

namespace GameActions
{
    public class EveryNTurns : ActionBase
    {
        int turns;
        ActionBase nestedAction;

        int turnsLeft;

        public override bool OverrideDescriptionKey => true;

        public override string GetDescription()
        {
            var descr = $"Every {turns} turns {nestedAction.GetDescription()}";
            if (board)
            {
                if (turnsLeft > 1)
                {
                    descr += $" (in {turnsLeft})";
                }
                else
                {
                    descr += " (this turn)";
                }
            }
            return descr;
        }

        public EveryNTurns(int turns, ActionBase nestedAction)
        {
            this.turns = turns;
            this.nestedAction = nestedAction;
        }

        public override void Init(IActionParent parent)
        {
            base.Init(parent);
            nestedAction.Init(parent);
            turnsLeft = turns;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            turnsLeft--;
            if (turnsLeft > 0) yield break;
            turnsLeft = turns;

            yield return nestedAction.Run(multiplier);
        }

        public class Builder : FactoryBuilder<ActionBase, int, FactoryBuilder<ActionBase>>
        {
            public override ActionBase Build() => new EveryNTurns(value, value2.Build());
        }
    }
}

