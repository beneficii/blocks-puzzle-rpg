using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using FancyToolkit;

public class MyTileData : TileData
{
    public int power;
    public TileStatType powerType;

    public FactoryBuilder<ClearAction.Base> clearAction;
    public FactoryBuilder<SimpleAction.Base> endTurnAction;
    public FactoryBuilder<SimpleAction.Base> enterAction;
    public FactoryBuilder<PassiveEffect.Base> passive;
}