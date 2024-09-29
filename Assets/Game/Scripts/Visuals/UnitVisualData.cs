using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitVisualData : ScriptableObject
{
    public List<Sprite> frames;

    public int frIddle;
    public int frAttack1;
    public int frAttack2;
    public int frHit;

    public Vector2 hpPos;
    public Vector2 actionPos;
}
