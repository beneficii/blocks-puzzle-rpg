using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

namespace BlockAction
{
    [CreateAssetMenu(menuName = "Game/BlockAction/Armor")]
    public class Armor : BlockActionBase
    {
        [SerializeField] int value;
        protected virtual int Value => value;

        public override string GetDescription() => $"Add {Value} armor";

        void AddArmor(Component target)
        {
            if (!target.TryGetComponent<Unit>(out var unit)) return;

            unit.AddArmor(value);
        }

        public override void HandleMatch(BtBlock parent, BtLineClearInfo info)
        {
            ResCtrl<MatchStat>.current.SetIfMore(MatchStat.MaxArmor, value);
            MakeBullet(parent)
                .SetTarget(CombatArena.current.player)
                .SetAction(AddArmor)
                .SetLaunchDelay(0.2f);
        }
    }
}
