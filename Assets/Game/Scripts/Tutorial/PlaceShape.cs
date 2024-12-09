using System.Collections;
using UnityEngine;
using TileShapes;

namespace TutorialItems
{
    public class PlaceShape : TutorialItemBase
    {
        protected override TutorialStep Step => TutorialStep.PlaceShape;

        void HandleShapeDrop(Shape shape, Vector2Int pos)
        {
            FinishStep();
        }

        protected override void OnEnter()
        {
            Shape.OnDroppedStatic += HandleShapeDrop;
        }

        protected override void OnExit()
        {
            Shape.OnDroppedStatic -= HandleShapeDrop;
        }
    }
}