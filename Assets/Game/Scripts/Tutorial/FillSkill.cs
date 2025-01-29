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
        }

        protected override void OnEnter()
        {
            if (Game.current.GetSkills().Count > 1)
            {
                FinishStep();
            }
            UISkillButton.OnAviableToUse += HandleEvent;
        }

        protected override void OnExit()
        {
            UISkillButton.OnAviableToUse -= HandleEvent;
        }
    }
}



