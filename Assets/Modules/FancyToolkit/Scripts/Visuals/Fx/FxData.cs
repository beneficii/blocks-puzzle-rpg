using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Visuals/FxData")]
public class FxData : ScriptableObject
{
    public List<Sprite> frames;
    public int actionFrame = -1;   // Frame index at which the action should be triggered
}
