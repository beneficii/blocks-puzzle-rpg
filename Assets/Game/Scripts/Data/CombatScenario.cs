using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using TileShapes;
using FancyToolkit;
using NUnit.Framework;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.UIElements.Experimental;

public abstract class CombatScenario
{
    protected Board board;
    protected ShapePanel shapePanel;

    public IEnumerator Start()
    {
        board = Object.FindAnyObjectByType<Board>();
        shapePanel = Object.FindAnyObjectByType<ShapePanel>();

        yield return StartRoutine();
    }
    protected abstract IEnumerator StartRoutine();
}

namespace Scenarios
{
    public class Tutorial : CombatScenario
    {
        public GameObject ghostCursor;
        protected override IEnumerator StartRoutine()
        {
            CombatCtrl.current.PreventEndTurn++;
            board.LoadLayoutByName("tutorial1");
            shapePanel.GenerateFromBytes(new()
            {
                new byte[,]{
                    {1,1,1}
                }
            });
            TutorialCtrl.current.ShowText(TutorialPanel.Board, "Guide the shapes to their destined place; complete a line to unleash its power.");
            Game.current.CreateGhostCursor(shapePanel.GetShapeMidPos(), board.GetAllowedMidPos());
            yield return new EventWaiter<LineClearData>(ref LineClearer.OnCleared);
            

            board.LoadLayoutByName("tutorial2");
            shapePanel.GenerateFromBytes(new()
            {
                new byte[,]{
                    {1 },
                    {1 },
                    {1 },
                }
            });
            TutorialCtrl.current.ShowText(TutorialPanel.Board, "Unleashed swords deal damage to your foes.");
            Game.current.CreateGhostCursor(shapePanel.GetShapeMidPos(), board.GetAllowedMidPos());
            yield return new EventWaiter<LineClearData>(ref LineClearer.OnCleared);
            
            board.LoadLayoutByName("tutorial3");
            shapePanel.GenerateFromBytes(new()
            {
                new byte[,]{
                    {1,1 },
                    {1,1 },
                }
            });
            
            TutorialCtrl.current.ShowText(TutorialPanel.Board, "Enemy is about to attack, unleash shields to grant you armor");
            Game.current.CreateGhostCursor(shapePanel.GetShapeMidPos(), board.GetAllowedMidPos());
            yield return new EventWaiter<Shape, Vector2Int>(ref Shape.OnDroppedStatic);
            TutorialCtrl.current.ShowText(TutorialPanel.Board, "Your armor will fade at the start of each turn. Now finish the beast!");
            //TutorialCtrl.current.HideAll();
            CombatCtrl.current.PreventEndTurn--;
            Game.current.RemoveGhostCursor();
        }
    }
}