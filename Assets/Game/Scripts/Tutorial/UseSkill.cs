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
            UISkillButton.OnUsed += HandleEvent;
        }

        protected override void OnExit()
        {
            UISkillButton.OnUsed -= HandleEvent;
        }
    }
}