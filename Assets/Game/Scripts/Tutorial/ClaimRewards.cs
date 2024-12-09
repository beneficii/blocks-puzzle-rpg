using System.Collections;
using UnityEngine;
using TileShapes;
using GridBoard;

namespace TutorialItems
{
    public class ClaimRewards : TutorialItemBase
    {
        protected override TutorialStep Step => TutorialStep.ClaimRewards;

        void HandleEvent(UICombatReward data)
        {
            FinishStep();
        }

        protected override void OnEnter()
        {
            UICombatReward.OnClicked += HandleEvent;
        }

        protected override void OnExit()
        {
            UICombatReward.OnClicked -= HandleEvent;
        }
    }
}


