using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;


[System.Serializable]
public class BlockPassive
{
    [SerializeField] CombatModifier passive;
    [SerializeField] int value;

    public string GetDescription() => GetPassiveDescription(passive, value);

    public void Calculate(BtBlock parent)
    {
        if (value > 0)
        {
            ResCtrl<CombatModifier>.current.Add(passive, value);
        }
        else if (value < 0)
        {
            ResCtrl<CombatModifier>.current.Remove(passive, -value, true);
        }
        else
        {
            Debug.LogError($"Value of passive ({passive}) is 0!");
        }
    }

    public static string GetModifierName(CombatModifier modifier)
    {
        return modifier switch
        {
            CombatModifier.Strength => "Strength",
            CombatModifier.Dexterity => "Dexterity",
            CombatModifier.EnemyStrength => "Enemy Strength",
            CombatModifier.EnemyDexterity => "Enemy Dexterity",
            CombatModifier.Poison => "Poisoned",
            CombatModifier.EnemyPoison => "Burn",
            CombatModifier.PassiveArmor => "Toughness",
            CombatModifier.EnemyPassiveArmor => "Enemy Toughness",
            _ => "Error",
        };
    }

    public static string GetModifierDescription(CombatModifier modifier)
    {
        return modifier switch
        {
            CombatModifier.Strength => "bonus to each damage instance",
            CombatModifier.Dexterity => "bonus to each armor instance",
            CombatModifier.EnemyStrength => "bonus to each damage instance",
            CombatModifier.EnemyDexterity => "bonus to each armor instance",
            CombatModifier.Poison => "Receive damage each turn",
            CombatModifier.EnemyPoison => "Enemy Receives damage each turn",
            CombatModifier.PassiveArmor => "Receive armor every turn",
            CombatModifier.EnemyPassiveArmor => "Enemy receives armor every turn",
            _ => "Error",
        };
    }

    public static string GetPassiveDescription(CombatModifier modifier, int value)
    {
        return $"{value.SignedStr()} {GetModifierName(modifier)} ({GetModifierDescription(modifier)})";
    }
}


public enum CombatModifier
{
    None,
    Strength,
    Dexterity,
    EnemyStrength,
    EnemyDexterity,
    Poison,             // end of turn damage
    EnemyPoison,        // end of turn damage
    PassiveArmor,       // end of turn armor
    EnemyPassiveArmor,  // end of turn armor
}