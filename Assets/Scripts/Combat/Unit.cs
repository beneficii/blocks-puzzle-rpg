using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;
using TMPro;

public class Unit : MonoBehaviour, IDamagable
{
    public ValueBar health;
    [SerializeField] Animator animator;

    [SerializeField] SpriteRenderer render;

    [SerializeField] UnitMoveIndicator moveIndicator;

    [SerializeField] IntReferenceDisplay templateDisplayHealth;
    [SerializeField] IntReferenceDisplay templateDisplayArmor;
    [SerializeField] TextIntRef txtIntDisplay;


    public static System.Action<Unit> OnKilled;

    public UnitData data { get; private set; }

    public BtUpgradeRarity reward;

    public Team team { get; private set; }

    IntReference refHealth;
    IntReference refArmor;

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
        this.reward = data.reward;
        actionIdx = 0;
        SetAction(null);
        SetNextAction();

        refHealth = new IntReference(data.hp, data.hp);
        refArmor = new IntReference(0);
        health.Init(refHealth);

        refHealth.OnChanged += HandleHealthChange;

        txtIntDisplay.Init(new()
        {
            new IntReferenceDisplay(refHealth, templateDisplayHealth),
            //new IntReferenceDisplay(refArmor, templateDisplayArmor),
        });
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

    void HandleHealthChange(int value, int delta)
    {
        if (value == 0)
        {
            HandleOutOfHealth();
            return;
        }
    }

    public void SetFlip(bool value)
    {
        render.flipX = value;
    }

    public int GetArmor()
    {
        return refArmor.Value;
    }

    public void AddArmor(int value)
    {
        var bonusModifier = team == Team.Ally ? CombatModifier.Dexterity : CombatModifier.EnemyDexterity;
        var bonus = ResCtrl<CombatModifier>.current.Get(bonusModifier);
        value += bonus;

        if (value <= 0) return;

        refArmor.Add(value + bonus);
    }

    public void RemoveHp(int damage)
    {
        var bonusModifier = team == Team.Enemy ? CombatModifier.Strength : CombatModifier.EnemyStrength;
        var bonus = ResCtrl<CombatModifier>.current.Get(bonusModifier);
        damage += bonus;

        if (damage <= 0) return;

        int defense = refArmor.Value;
        refArmor.Value -= damage;
        damage -= defense;

        if (damage <= 0) return;

        AnimGetHit();
        health.Remove(damage);
        refHealth.Remove(damage);
    }

    public void SetHp(int value)
    {
        health.Set(value);
        refHealth.Value = value;
    }

    public void AddHp(int value)
    {
        health.Add(value);
        refHealth.Add(value);
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
        refArmor.Value = 0;
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


    // caption stuff
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