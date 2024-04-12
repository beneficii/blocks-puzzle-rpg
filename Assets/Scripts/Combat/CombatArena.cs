using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatArena : MonoBehaviour
{
    static CombatArena _current;
    public static CombatArena current
    {
        get
        {
            if (!_current) 
            {
                _current = FindFirstObjectByType<CombatArena>();
            }

            return _current;
        }
    }

    public Unit hero;
    public Unit enemy;
}