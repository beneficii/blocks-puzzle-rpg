using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using TileShapes;
using FancyToolkit;

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
            yield return new EventWaiter<LineClearData>(ref LineClearer.OnCleared);
            


            board.LoadLayoutByName("tutorial3");
            shapePanel.GenerateFromBytes(new()
            {
                new byte[,]{
                    {1,1 },
                    {1,1 },
                }
            });
            CombatCtrl.current.PreventEndTurn--;
            yield return new EventWaiter<LineClearData>(ref LineClearer.OnCleared);
            
            Debug.Log("Tutorial finished!");
        }
    }
}