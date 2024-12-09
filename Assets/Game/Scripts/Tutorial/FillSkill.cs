using System.Collections;
using UnityEngine;
using TileShapes;
using GridBoard;

namespace TutorialItems
{
    public class FillSkill : TutorialItemBase
    {
        protected override TutorialStep Step => TutorialStep.FillSkill;

        void HandleEvent(UISkillButton data)
        {
                FinishStep();
            /*if (data.HasManualUse)
            {
            }
            */
        }

        protected override void OnEnter()
        {
            UISkillButton.OnAviableToUse += HandleEvent;
        }

        protected override void OnExit()
        {
            UISkillButton.OnAviableToUse -= HandleEvent;
        }
    }
}



