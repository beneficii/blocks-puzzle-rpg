using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

namespace Scenarios
{
    public abstract class ChallengeBase : CombatScenario
    {
        protected string dialogSuccess;
        protected string dialogFail;

        protected bool isSuccess;
        protected bool isFail;

        int turnsSurvived;

        protected abstract IEnumerator ChallengeRoutine();
        protected virtual void HandleTurn(int n) { }

        protected virtual void PlayerAlmostDied()
        {
            isFail = true;
        }

        protected virtual void EnemyAlmostDied()
        {

        }

        protected void HandleUnitAboutToDie(DeathEventArgs args)
        {
            var unit = args.unit;
            args.PreventDeath();
            if (unit == CombatArena.current.player)
            {
                PlayerAlmostDied();
            }
            else
            {
                EnemyAlmostDied();
            }
        }

        protected void HandleTurnFinished()
        {
            turnsSurvived++;
            HandleTurn(turnsSurvived);
        }

        protected override IEnumerator StartRoutine()
        {
            Unit.OnAboutToDie += HandleUnitAboutToDie;
            CombatCtrl.OnTurnFinished += HandleTurnFinished;
            yield return ChallengeRoutine();
            Unit.OnAboutToDie -= HandleUnitAboutToDie;
            CombatCtrl.OnTurnFinished -= HandleTurnFinished;

            if (isSuccess)
            {
                CombatCtrl.current.ShowCombatDialog(dialogSuccess);
            }
            else
            {
                CombatCtrl.current.ShowCombatDialog(dialogFail);
            }
        }
    }

    public class ChallengeDamage : ChallengeBase
    {
        int hpTarget;
        public ChallengeDamage(StringScanner scanner)
        {
            hpTarget = scanner.NextInt();
            dialogSuccess = scanner.NextString();
            dialogFail = scanner.NextString();
        }

        protected override IEnumerator ChallengeRoutine()
        {
            var enemy = CombatArena.current.enemy;
            yield return new WaitWhile(() => !isFail && enemy.health.Value > hpTarget);

            if (enemy.health.Value <= hpTarget)
            {
                isSuccess = true;
            }
        }

        protected override void EnemyAlmostDied()
        {
            isSuccess = true;
        }
    }

    public class ChallengeSurvive : ChallengeBase
    {
        int turns;
        public ChallengeSurvive(StringScanner scanner)
        {
            turns = scanner.NextInt();
            dialogSuccess = scanner.NextString();
            dialogFail = scanner.NextString();
        }

        protected override void HandleTurn(int n)
        {
            if (n >= turns) isSuccess = true;
        }

        protected override IEnumerator ChallengeRoutine()
        {
            var enemy = CombatArena.current.enemy;
            yield return new WaitUntil(() => isFail || isSuccess);

            if (isFail) isSuccess = false;
        }

        protected override void EnemyAlmostDied()
        {
            isSuccess = true;
        }
    }
}