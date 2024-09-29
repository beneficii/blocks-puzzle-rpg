using FancyToolkit;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Unit : MonoBehaviour, IDamagable
{
    public ValueBar health;
    [SerializeField] SpriteRenderer render;

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

    public UnitData data { get; private set; }
    public UnitVisualData visuals;

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
        this.reward = data.reward;
        actionIdx = 0;
        SetAction(null);
        SetNextAction();
        // ToDo: set visual data

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
        //animator.SetTrigger($"attack{id}");
        animator.Play(id == 1 ? AnimType.Attack1 : AnimType.Attack2);

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
        if (visuals.hpPos != default) parentHp.localPosition = visuals.hpPos;
        if (visuals.actionPos != default) moveIndicator.transform.localPosition = visuals.actionPos;
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