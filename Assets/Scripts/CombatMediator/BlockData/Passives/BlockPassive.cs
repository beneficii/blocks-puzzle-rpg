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
    public string GetTooltip() => GetModifierTooltip(passive);

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
            CombatModifier.Poison => "Burn",
            CombatModifier.EnemyPoison => "Enemy Burn",
            CombatModifier.PassiveArmor => "Ward",
            CombatModifier.EnemyPassiveArmor => "Enemy Ward",
            _ => "Error",
        };
    }

    public static string GetModifierTooltip(CombatModifier modifier)
    {
        return modifier switch
        {
            CombatModifier.Strength => "Strength: Increases damage by X per hit",
            CombatModifier.Dexterity => "Dexterity: Increases Armor gained by X",
            CombatModifier.EnemyStrength => "Strength: Increases damage by X per hit",
            CombatModifier.EnemyDexterity => "Dexterity: Increases Armor gained by X",
            CombatModifier.Poison => "Burn: Receive damage each turn",
            CombatModifier.EnemyPoison => "Burn: Receive damage each turn",
            CombatModifier.PassiveArmor => "Ward: Receive armor every turn",
            CombatModifier.EnemyPassiveArmor => "Ward: Receive armor every turn",
            _ => "Error",
        };
    }

    public static string GetPassiveDescription(CombatModifier modifier, int value)
    {
        return $"{value.SignedStr()} {GetModifierName(modifier)}";
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