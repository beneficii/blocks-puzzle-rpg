using System.Collections;
using UnityEngine;
using TileShapes;
using GridBoard;

namespace TutorialItems
{
    public class FillLine : TutorialItemBase
    {
        protected override TutorialStep Step => TutorialStep.FillLine;

        void HandleEvent(LineClearData data)
        {
            FinishStep();
        }

        protected override void OnEnter()
        {
            LineClearer.OnCleared += HandleEvent;
        }

        protected override void OnExit()
        {
            LineClearer.OnCleared -= HandleEvent;
        }
    }
}

