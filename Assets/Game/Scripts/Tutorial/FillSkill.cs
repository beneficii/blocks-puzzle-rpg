using System.Collections;
using UnityEngine;
using TileShapes;
using GridBoard;
using TMPro;

namespace TutorialItems
{
    public class FillSkill : TutorialItemBase
    {
        protected override TutorialStep Step => TutorialStep.FillSkill;
        
        [SerializeField] TextMeshProUGUI txtCaption;

        bool isFilled = false;

        void HandleEventAviable(UISkillButton data)
        {
            if (isFilled) return;
            isFilled = true;
            if (!data.HasManualUse)
            {
                FinishStep();
                return;
            }

            txtCaption.text = "Click to use";
            //FinishStep();
        }

        void HandleEventUsed(UISkillButton data)
        {
            if (isFilled) return;
            isFilled = true;
            if (!data.HasManualUse)
            {
                FinishStep();
                return;
            }

            txtCaption.text = "Click to use";
            //FinishStep();
        }


        protected override void OnEnter()
        {
            if (Game.current.GetSkills().Count > 1)
            {
                FinishStep();
            }
            UISkillButton.OnAviableToUse += HandleEventAviable;
            UISkillButton.OnUsed += HandleEventUsed;
        }

        protected override void OnExit()
        {
            UISkillButton.OnAviableToUse -= HandleEventAviable;
            UISkillButton.OnUsed -= HandleEventUsed;
        }
    }
}



