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