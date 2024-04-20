using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class ResourceListener : UIGenericResourceDisplay<MyTestResource>
{
    public void BtnAdd()
    {
        ResCtrl<MyTestResource>.current.Add(type, 1);
    }
}


public enum MyTestResource
{
    None,
    Wood,
    Stone,
    Gems,
}