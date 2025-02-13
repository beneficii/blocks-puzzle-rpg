using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using TileShapes;
using FancyToolkit;

namespace Scenarios
{
    public class Tutorial1 : CombatScenario
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
            TutorialCtrl.current.ShowText(TutorialPanel.Board, "Move shapes to the board. Complete a line to clear tiles and unleash damage from swords");
            Game.current.CreateGhostCursor(shapePanel.GetShapeMidPos(), board.GetAllowedMidPos());
            yield return new EventWaiter<LineClearData>(ref LineClearer.OnCleared);


            board.LoadLayoutByName("tutorial2");
            shapePanel.GenerateFromBytes(new()
            {
                /*
                new byte[,]{
                    {1 },
                    {1 },
                    {1 },
                }
                */
                new byte[,]{
                    {1,1,1}
                }
            });
            TutorialCtrl.current.ShowText(TutorialPanel.Board, "Use mouse wheel or 2nd mouse button to rotate the shape while holding it");
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

            TutorialCtrl.current.ShowText(TutorialPanel.Board, "You can see enemy intent in the icon above them. Clear shields to gain armor.");
            Game.current.CreateGhostCursor(shapePanel.GetShapeMidPos(), board.GetAllowedMidPos());
            yield return new EventWaiter<Shape, Vector2Int>(ref Shape.OnDroppedStatic);
            TutorialCtrl.current.ShowText(TutorialPanel.Board, "Armor fades each new turn. Now finish the beast! (Press \"End Turn\" to get new shapes)");
            //TutorialCtrl.current.HideAll();
            CombatCtrl.current.PreventEndTurn--;
            Game.current.RemoveGhostCursor();
        }
    }
}