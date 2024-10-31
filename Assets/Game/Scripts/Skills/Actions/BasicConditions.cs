using FancyToolkit;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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