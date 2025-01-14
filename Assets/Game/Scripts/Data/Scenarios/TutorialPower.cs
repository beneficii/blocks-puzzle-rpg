using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using TileShapes;
using FancyToolkit;

namespace Scenarios
{
    public class TutorialPower : CombatScenario
    {
        bool enemyLow = false;

        void HandleUnitDamage(Unit unit, int damage)
        {
            if (unit != CombatArena.current.enemy) return;

            Game.current.StartCoroutine(CheckHp(unit));
        }

        IEnumerator CheckHp(Unit unit)
        {
            yield return null;
            
            if (!unit || unit.health.Value <= 450)
            {
                enemyLow = true;
            }
        }

        protected override IEnumerator StartRoutine()
        {
            var specialTile = StageCtrl.current.Data?.specialTile;
            board.LoadLayoutByName("tutorial_power", TileCtrl.current.Get(specialTile));
            TutorialCtrl.current.ShowText(TutorialPanel.Board, "Hover over tiles or units to see their effects and actions.");
            yield return new EventWaiter<Transform>(ref UIHoverInfoCtrl.OnHovered);
            
            TutorialCtrl.current.ShowText(TutorialPanel.Board, "Some tiles have Power (X) number, it is used in effects like damage or defense.");

            Unit.OnReceiveDamage += HandleUnitDamage;
            yield return new WaitUntil(() => enemyLow);
            Unit.OnReceiveDamage -= HandleUnitDamage;
            yield return new WaitWhile(() => WorldUpdateCtrl.current.IsUpdating);
            TutorialCtrl.current.HideAll();
            CombatCtrl.current.ShowCombatDialog("tutorial_power_end");
        }
    }
}