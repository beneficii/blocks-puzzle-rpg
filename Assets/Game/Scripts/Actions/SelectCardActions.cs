using FancyToolkit;
using System.Collections;
using UnityEngine;

namespace GameActions
{
    public class HealPlayerBuy : ActionBase
    {
        int value;

        public HealPlayerBuy(int value)
        {
            this.value = value;
        }

        public override string GetDescription()
            => $"Heal {value} health when bought";

        public override IEnumerator Run(int multiplier = 1)
        {
            var player = CombatArena.current.player;
            if (!player)
            {
                Debug.LogError("No player found for healing");
                yield break;
            }

            player.AddHp(value);

            yield return null;
        }

        public class Builder : FactoryBuilder<ActionBase, int>
        {
            public override ActionBase Build() => new HealPlayerBuy(value);
        }
    }

}