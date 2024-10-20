using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using FancyToolkit;

public class MyTileData : TileData
{
    public int power;
    public TileStatType powerType;

    public FactoryBuilder<TileActionBase> clearAction;
    public FactoryBuilder<TileActionBase> endTurnAction;
    public FactoryBuilder<TileActionBase> enterAction;
    public FactoryBuilder<TileActionBase> passive;
}