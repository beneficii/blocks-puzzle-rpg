using FancyToolkit;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public abstract class SkillClickCondition
{
    protected UISkillButton parent;
    public abstract string GetDescription();
    bool canUse;
    public virtual bool CanUse
    {
        get => canUse;
        set
        {
            canUse = value;
            parent.RefreshUse();
        }
    }

    public virtual string GetErrorUnusable() => "Can't use right now";
    public virtual bool StartingValue => false;

    public virtual void Init(UISkillButton parent)
    {
        this.parent = parent;
        if (parent.board)
        {
            CanUse = StartingValue;
        }
    }

    public abstract void OnClicked();
}

namespace SkillConditions
{
    public class Once : SkillClickCondition
    {
        public override string GetDescription()
            => "Use Once per combat";

        public override bool StartingValue => true;
        public override string GetErrorUnusable() => "Only once per combat";

        public override void OnClicked()
        {
            CanUse = false;
        }

        public class Builder : FactoryBuilder<SkillClickCondition>
        {
            public override SkillClickCondition Build() => new Once();
        }

    }
}