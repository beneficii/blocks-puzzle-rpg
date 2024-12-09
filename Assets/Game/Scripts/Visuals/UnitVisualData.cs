using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;
using JetBrains.Annotations;
using UnityEngine.Rendering;

public class UnitVisualData : ScriptableObject
{
    public List<Sprite> frames;

    public int frIddle;
    public int frAttack1;
    public int frAttack2;
    public int frHit;

    public bool flipX = true;

    public Vector2 hpPos;
    public Vector2 actionPos;

    public FxData fxAttack;
    public FxData fxDeath;

    public AudioClip soundAttack;
    public AudioClip soundAbility;
    public AudioClip soundGetHitArmor;
    public AudioClip soundGetHit;
    public AudioClip soundGetHitHard;
    public AudioClip soundDeath;
}
