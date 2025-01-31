using System.Collections;
using UnityEngine;
using TileShapes;
using GridBoard;

namespace TutorialItems
{
    public class UseSkill : TutorialItemBase
    {
        protected override TutorialStep Step => TutorialStep.UseSkill;

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
            UISkillButton.OnUsed += HandleEvent;
        }

        protected override void OnExit()
        {
            UISkillButton.OnUsed -= HandleEvent;
            FinishStep();   // in case player didn't click this and finished level
        }
    }
}