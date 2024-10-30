using FancyToolkit;
using System.Collections;
using UnityEngine;

namespace TileActions
{
    public class HealPlayerBuy : TileActionBase
    {
        public override string GetDescription()
            => $"Heal {Power} health when bought";

        public override IEnumerator Run(int multiplier = 1)
        {
            var player = CombatArena.current.player;
            if (!player)
            {
                Debug.LogError("No player found for healing");
                yield break;
            }

            player.AddHp(Power);

            yield return null;
        }

        public class Builder : FactoryBuilder<TileActionBase>
        {
            public override TileActionBase Build() => new HealPlayerBuy();
        }
    }
}