using FancyToolkit;
using System.Collections;
using UnityEngine;
using GridBoard;

public abstract class SkillClickCondition
{
    protected UISkillButton parent;
    public abstract string GetDescription();
    bool canUse;

    public virtual bool AutoActivate => false;
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

    public virtual void Destroy()
    {

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

    public class Charge : SkillClickCondition, ILineClearHandler
    {
        protected int maxCharge;
        protected int currentCharge;

        public Charge(int maxCharge)
        {
            this.maxCharge = maxCharge;
        }

        int CurrentCharge
        {
            get => currentCharge;
            set
            {
                currentCharge = Mathf.Min(value, maxCharge);
                var full = currentCharge == maxCharge;
                CanUse = full;
                var fill = full ? 0f : 1f - (currentCharge / (float)maxCharge);
                parent.CooldownFill = fill;
            }
        }

        public override string GetDescription()
        {
            if (parent)
            {
                return $"Clear lines to charge ({currentCharge}/{maxCharge})";
            }
            else
            {
                return $"Clear {maxCharge} lines to charge";
            }
        }

        public override string GetErrorUnusable() => "Clear lines to charge";

        protected virtual void Unlocked()
        {

        }

        void AddCharge(int value)
        {
            if (CanUse) return; // is already charged
            CurrentCharge = Mathf.Min(currentCharge + value, maxCharge);
            if (CanUse) Unlocked();
        }

        public override void Init(UISkillButton parent)
        {
            base.Init(parent);
            CurrentCharge = 0;

            LineClearer.AddHandler(this);
        }

        public override void Destroy()
        {
            LineClearer.RemoveHandler(this);
        }

        public override void OnClicked()
        {
            CurrentCharge = 0;
        }

        public IEnumerator HandleLinesCleared(LineClearData clearData)
        {
            var totalCharge = clearData.rowsMatched + clearData.columnsMatched;
            //var goalCharge = Mathf.Min(maxCharge, currentCharge + totalCharge);
            AddCharge(totalCharge);
            yield break;
        }

        public class Builder : FactoryBuilder<SkillClickCondition, int>
        {
            public override SkillClickCondition Build() => new Charge(value);
        }
    }

    public class AutoCharge : Charge
    {
        public override bool AutoActivate => true;

        public override string GetDescription()
        {
            if (parent)
            {
                return $"Clear lines to activate ({currentCharge}/{maxCharge})";
            }
            else
            {
                return $"Clear {maxCharge} lines to activate";
            }
        }

        public AutoCharge(int maxCharge) : base(maxCharge)
        {
        }

        protected override void Unlocked()
        {
            parent.OnClick();
        }

        public class Builder2 : FactoryBuilder<SkillClickCondition, int>
        {
            public override SkillClickCondition Build() => new AutoCharge(value);
        }
    }
}