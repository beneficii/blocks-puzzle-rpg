using FancyToolkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Unit : MonoBehaviour, IDamagable
{
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

    [SerializeField] Transform parentHp;

    public static System.Action<Unit> OnKilled;

    UnitAnimator animator;

    public UnitData2 data { get; private set; }
    public UnitVisualData visuals;

    public Team team { get; private set; } = Team.Enemy;

    IntReference refHealth;
    IntReference refArmor;

    UnitAction.Base nextAction;
    Unit target;

    public List<int> modifiers;
    public int lifetime = 0;

    public string GetDescription()
    {
        return nextAction?.GetLongDescription(this)??data.description;
    }

    public string GetTooltip()
    {
        return nextAction?.GetTooltip(this);
    }

    int actionIdx = 0;

    public void Init(UnitData2 data, Team team)
    {
        Assert.IsNotNull(data);
        this.team = team;
        this.data = data;
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

    public void SetDialog(string text = null)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            contentDialog.SetActive(false);
            return;
        }

        contentDialog.SetActive(true);
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
        data.visuals.soundDeath?.PlayNow();
        DataManager.current.CreateFX(data.visuals.fxDeath, transform.position, Destroy);
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
        if (nextAction == null) yield break;
        yield return nextAction.Run(this, target);
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

#if UNITY_EDITOR
    [EasyButtons.Button]
    void LoadVisualData()
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

    [EasyButtons.Button]
    void SaveVisualData()
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
        EditorUtility.SetDirty(visuals);

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

public enum Buffs
{
    None,
    NoBlockRemove,
    Vulnerable
}


public enum Team
{
    Enemy,
    Ally,
}