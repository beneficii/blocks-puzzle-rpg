using FancyToolkit;
using System.Collections;
using UnityEngine;

namespace GameActions
{
    public class TilesPerTurn : ActionBase
    {
        int amount;

        public override string GetDescription()
            => $"{amount.SignedStr()} tiles per turn";

        public TilesPerTurn(int amount)
        {
            this.amount = amount;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            CombatCtrl.current.tilesPerTurn += amount * multiplier;

            yield return null;
        }

        public class Builder : FactoryBuilder<ActionBase, int>
        {
            public override ActionBase Build() => new TilesPerTurn(value);
        }
    }
}