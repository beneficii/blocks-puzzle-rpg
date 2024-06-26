﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class Unit : MonoBehaviour, IDamagable
{
    [SerializeField] ValueBar health;
    [SerializeField] ValueCounter armor;
    [SerializeField] Animator animator;

    [SerializeField] SpriteRenderer render;

    [SerializeField] UnitMoveIndicator moveIndicator;

    public static System.Action<Unit> OnKilled;

    public UnitData data { get; private set; }

    public BtUpgradeRarity reward;

    public Team team { get; private set; }

    //HashSet<Buffs> buffs = new();

    UnitActionBase nextAction;
    Unit target;

    public int lifetime = 0;

    public string GetDescription()
    {
        if (nextAction) return nextAction.GetDescription(this);

        return data.description;
    }

    public string GetTooltip()
    {
        return nextAction?.GetTooltip(this);
    }

    int actionIdx = 0;

    public void Init(UnitData data, Team team)
    {
        this.team = team;
        this.data = data;
        animator.runtimeAnimatorController = data.animations;
        health.Init(data.hp);
        health.OnZero += HandleOutOfHealth;
        armor.Value = 0;
        armor.OnChanged += HandleArmorChanged;
        this.reward = data.reward;
        actionIdx = 0;
        SetAction(null);
        SetNextAction();
    }

    public void SetTarget(Unit target)
    {
        this.target = target;
    }

    public void SetNextAction()
    {
        if (data.actionQueue.Count == 0) return;
        var action = data.actionQueue[actionIdx];

        SetAction(action);

        actionIdx = (actionIdx + 1) % data.actionQueue.Count;
    }

    public void SetAction(UnitActionBase action)
    {
        moveIndicator.Init(this, action);
        nextAction = action;
    }

    void Destroy()
    {
        if (!gameObject) return;

        Destroy(gameObject);
    }

    void HandleOutOfHealth()
    {
        OnKilled?.Invoke(this);
        data.soundDeath?.PlayNow();
        var fxDeath = data.fxDeath;
        if (fxDeath)
        {
            Instantiate(fxDeath, transform.position, Quaternion.identity)
                .SetTriggerAction(Destroy);
        }
        else
        {
            Destroy();
        }
    }

    public void SetFlip(bool value)
    {
        render.flipX = value;
    }

    public int GetArmor()
    {
        return armor.Value;
    }

    public void AddArmor(int value)
    {
        var bonusModifier = team == Team.Ally ? CombatModifier.Dexterity : CombatModifier.EnemyDexterity;
        var bonus = ResCtrl<CombatModifier>.current.Get(bonusModifier);
        value += bonus;

        if (value <= 0) return;

        armor.Add(value + bonus);
    }

    void HandleArmorChanged(int oldValue, int newValue)
    {
        bool newIsZero = newValue == 0;
        if ((oldValue == 0) == newIsZero) return;
    }

    public void RemoveHp(int damage)
    {
        var bonusModifier = team == Team.Enemy ? CombatModifier.Strength : CombatModifier.EnemyStrength;
        var bonus = ResCtrl<CombatModifier>.current.Get(bonusModifier);
        damage += bonus;

        if (damage <= 0) return;

        int defense = armor.Value;
        if (defense > 0)
        {
            defense -= damage;
            if (defense >= 0)
            {
                armor.Remove(damage);
                damage = 0;
            }
            else
            {
                armor.Remove(armor.Value);
                damage = -defense;
            }
        }

        if (damage == 0) return;

        AnimGetHit();
        health.Remove(damage);
    }

    public void AddHp(int value)
    {
        health.Add(value);
    }

    public IEnumerator RoundActionPhase()
    {
        lifetime++;
        if (!nextAction) yield break;
        yield return nextAction.Execute(this, target);
        SetNextAction();
    }

    public void CombatFinished()
    {
        armor.Value = 0;
    }

    public void RoundFinished()
    {
        /*
        if (buffs.Contains(Buffs.NoBlockRemove))
        {
            buffs.Remove(Buffs.NoBlockRemove);
        }
        else
        {
            armor.Value = 0;
        }*/
    }

    public void AnimAttack(int id)
    {
        animator.SetTrigger($"attack{id}");

        if (id == 1)
        {
            data.soundAttack?.PlayNow();
        }
        if (id == 2)
        {
            data.soundAbility?.PlayNow();
        }
    }

    public void AnimGetHit()
    {
        animator.SetTrigger($"hit");
    }
}

public enum Buffs
{
    None,
    NoBlockRemove,
    Vulnerable
}


public enum Team
{
    Ally,
    Enemy,
}