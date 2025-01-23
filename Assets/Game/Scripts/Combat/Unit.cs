using FancyToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Unit : MonoBehaviour, IDamagable
{
    public static event System.Action<Unit, int> OnReceiveDamage;
    public static event System.Action<DeathEventArgs> OnAboutToDie;
    public static System.Action<Unit> OnKilled;

    public ValueBar health;
    [SerializeField] SpriteRenderer render;
    [SerializeField] SpriteRenderer shadow;

    [SerializeField] UnitMoveIndicator moveIndicator;

    [SerializeField] IntReferenceDisplay templateDisplayHealth;
    [SerializeField] IntReferenceDisplay templateDisplayArmor;
    [SerializeField] TextIntRef txtHealth;
    [SerializeField] TextIntRef txtArmor;
    [SerializeField] IconIntRef iconArmor;

    [SerializeField] TextMeshPro txtDialog;
    [SerializeField] GameObject contentDialog;
    [SerializeField] SpriteRenderer bgDialog;
    [SerializeField] Sprite bgDialogBubble;
    [SerializeField] Sprite bgDialogNarration;


    [SerializeField] Transform parentHp;


    UnitAnimator animator;

    public UnitData data { get; private set; }
    public UnitVisualData visuals;

    public int damage;
    public int defense;

    public int GetDefense() => defense > 1 ? defense : 1;
    public int GetDamage() => damage > 1 ? damage : 1;

    public Team team { get; private set; } = Team.Enemy;

    IntReference refHealth;
    IntReference refArmor;

    UnitAction.Base nextAction;
    Unit target;

    public GridBoard.Board board;

    public List<int> modifiers;
    public int lifetime = 0;

    public bool modifierDontResetArmorOnce;

    bool isCombatVisible = true;
    bool isInvulnerable = false;

    public string GetDescription()
    {
        var lines = new List<string>();
        if (!string.IsNullOrWhiteSpace(data.description)) lines.Add(data.description);

        if (isCombatVisible)
        {
            if (nextAction != null) lines.Add(nextAction.GetLongDescription());
            lines.Add("");
            if (damage > 0) lines.Add($"Damage: {damage}");
            if (defense > 0) lines.Add($"Defense: {defense}");
        }

        return string.Join("\n", lines);
    }

    public string GetTooltip()
    {
        return nextAction?.GetTooltip();
    }

    int actionIdx = 0;

    public void Init(UnitData data, Team team)
    {
        Assert.IsNotNull(data);
        this.team = team;
        this.data = data;
        this.damage = data.damage;
        this.defense = data.defense;
        visuals = data.visuals;
        actionIdx = 0;
        SetAction(null);
        SetNextAction();

        refHealth = new IntReference(data.hp, data.hp);
        refArmor = new IntReference(0);
        health.Init(refHealth);

        refHealth.OnChanged += HandleHealthChange;

        txtHealth.Init(new()
        {
            new IntReferenceDisplay(refHealth, templateDisplayHealth),
            //new IntReferenceDisplay(refArmor, templateDisplayArmor),
        });

        txtArmor.Init(new()
        {
            new IntReferenceDisplay(refArmor, templateDisplayArmor),
        });

        iconArmor.Init(refArmor);
        SetDialog(null);
        LoadVisualData();
        animator = GetComponent<UnitAnimator>();
        animator.Init(visuals);
        modifiers = Enumerable
            .Repeat(0, EnumUtil.GetLength<Modifier>())
            .ToList();
    }

    public void SetCombatVisible(bool value)
    {
        isCombatVisible = value;
        moveIndicator.gameObject.SetActive(value);
        parentHp.gameObject.SetActive(value);
    }


    public void SetDialog(string text = null, bool narration = false)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            contentDialog.SetActive(false);
            return;
        }

        contentDialog.SetActive(true);
        bgDialog.sprite = narration ? bgDialogNarration : bgDialogBubble;
        txtDialog.text = text;
    }

    public void SetTarget(Unit target)
    {
        this.target = target;
    }

    public int GetModifier(Modifier type)
    {
        return modifiers[(int)type];
    }

    public bool GetModifier(Modifier type, out int value)
    {
        value = modifiers[(int)type];
        return value != 0;
    }

    public void SetNextAction()
    {
        if (data.actions.Count == 0) return;
        var action = data.actions[actionIdx];

        SetAction(action.Build());

        actionIdx = (actionIdx + 1) % data.actions.Count;
    }

    public void SetAction(UnitAction.Base action)
    {
        action?.Init(this);
        moveIndicator.Init(this, action);
        nextAction = action;
    }

    public void RefreshAction()
    {
        moveIndicator.RefreshShortDescription(nextAction);
    }

    void Destroy()
    {
        if (!gameObject) return;

        Destroy(gameObject);
    }

    void HandleOutOfHealth()
    {
        var args = new DeathEventArgs(this);
        OnAboutToDie?.Invoke(args);
        if (args.IsDeathPrevented)
        {
            isInvulnerable = true;
            health.Set(1);
            return;
        }
        OnKilled?.Invoke(this);
        data.visuals.soundDeath?.PlayNow();
        Game.current.CreateFX(data.visuals.fxDeath, transform.position, Destroy);
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
        shadow.flipX = value;
    }

    public int GetArmor()
    {
        return refArmor.Value;
    }

    public void AddArmor(int value)
    {
        if (value <= 0) return;

        refArmor.Add(value);
    }

    public void RemoveArmor(int value)
    {
        if (value <= 0) return;

        refArmor.Remove(value);
    }

    public void SetArmor(int value)
    {
        refArmor.Value = value;
    }

    public void FakeDamage()
    {
        data.visuals.soundGetHit?.PlayWithRandomPitch(.2f);
    }

    public void RemoveHp(int damage)
    {
        if (isInvulnerable) return;
        if (damage <= 0) return;

        int defense = refArmor.Value;
        refArmor.Value -= damage;
        damage -= defense;

        if (damage <= 0)
        {
            data.visuals.soundGetHitArmor?.PlayWithRandomPitch(.2f);
            return;
        }

        OnReceiveDamage?.Invoke(this, damage);
        if (damage > 20)
        {
            data.visuals.soundGetHitHard?.PlayWithRandomPitch(.2f);
        }
        else
        {
            data.visuals.soundGetHit?.PlayWithRandomPitch(.2f);
        }

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
        if (nextAction == null) yield break;
        yield return nextAction.Run(target);
        SetNextAction();
    }

    public IEnumerator EndOfTurn()
    {
        yield return null;
        if (modifierDontResetArmorOnce)
        {
            modifierDontResetArmorOnce = false;
        }
        else
        {
            SetArmor(0);
        }
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
        //animator.SetTrigger($"attack{id}");
        animator.Play(id == 1 ? AnimType.Attack1 : AnimType.Attack2);

        if (id == 1)
        {
            data.visuals.soundAttack?.PlayNow();
        }
        if (id == 2)
        {
            data.visuals.soundAbility?.PlayNow();
        }
    }

    public void AnimGetHit()
    {
        animator.Play(AnimType.Hit);
        //animator.SetTrigger($"hit");
    }

    [EasyButtons.Button]
    public void LoadVisualData()
    {
        if (!visuals)
        {
            Debug.LogError("Visuals not set!");
            return;
        }

        render.sprite = visuals.frames[0];
        SetFlip((team == Team.Ally) != visuals.flipX);
        if (visuals.hpPos != default) parentHp.localPosition = visuals.hpPos;
        if (visuals.actionPos != default) moveIndicator.transform.localPosition = visuals.actionPos;
        /*if (visuals.shadow != default)
        {
            shadow.localPosition = new(visuals.shadow.x, 0, 0);
            shadow.localScale = new(visuals.shadow.y, visuals.shadow.y/10, 1);
        }*/
    }

#if UNITY_EDITOR
    [EasyButtons.Button]
    public void SaveVisualData()
    {
        if (!visuals)
        {
            Debug.LogError("Visuals not set!");
            return;
        }

        visuals.hpPos = parentHp.localPosition;
        visuals.actionPos = moveIndicator.transform.localPosition;
        visuals.flipX = render.flipX;
        /*visuals.shadow = new(
            shadow.localPosition.x,
            shadow.localScale.x
        );*/
        UnityEditor.EditorUtility.SetDirty(visuals);

    }
#endif

    [System.Serializable]
    public enum AnimType
    {
        None,
        Iddle,
        Attack1,
        Attack2,
        Hit,
    }

    public enum Modifier
    {
        None,
        SwordAttack,
        AttackPerTurn,
        RingHeals,
    }
}

public enum Team
{
    Enemy,
    Ally,
}

public class DeathEventArgs : EventArgs
{
    public Unit unit;
    public bool IsDeathPrevented { get; private set; }

    public DeathEventArgs(Unit unit)
    {
        this.unit = unit;
    }

    public void PreventDeath()
    {
        IsDeathPrevented = true;
    }
}