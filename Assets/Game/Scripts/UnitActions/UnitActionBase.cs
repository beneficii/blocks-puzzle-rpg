using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using FancyToolkit;
using static UnityEngine.Rendering.DebugUI;
using System.Linq;
using static UnityEditor.Progress;
using GridBoard.TileActions;

namespace UnitAction
{
    public abstract class Base
    {
        public virtual string ActionVisual() => "Attack";
        public virtual string GetDescription(Unit parent) => "";
        public abstract void Run(Unit parent);
    }

    public class Damage : Base
    {
        int damage;
        public override string GetDescription(Unit parent)
            => $"Deal {damage} damage";

        public Damage(int damage)
        {
            this.damage = damage;
        }

        public override void Run(Unit parent)
        {

        }

        public class Builder : FactoryBuilder<Base, int>
        {
            public override Base Build() => new Damage(value);
        }
    }
}