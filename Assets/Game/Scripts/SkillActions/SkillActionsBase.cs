using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FancyToolkit;
using GridBoard;

namespace SkillAction
{
    public abstract class Base
    {
        public abstract string GetDescription();

        public virtual bool CanUse() => false;

        public virtual void Use()
        {

        }

        protected virtual void OnCombatStarted()
        {

        }

        protected virtual void OnCombatFinished()
        {
        }
    }
}