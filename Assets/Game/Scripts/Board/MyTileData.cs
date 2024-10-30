using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using FancyToolkit;
using System.Text;
using TileActions;

public class MyTileData : TileData
{
    public int power;
    public TileStatType powerType;

    public FactoryBuilder<TileActionBase> clearAction;
    public FactoryBuilder<TileActionBase> endTurnAction;
    public FactoryBuilder<TileActionBase> enterAction;
    public FactoryBuilder<TileActionBase> passive;
    public FactoryBuilder<TileActionBase> buyAction;
}