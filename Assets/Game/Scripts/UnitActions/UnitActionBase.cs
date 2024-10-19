using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using FancyToolkit;
using System.Linq;
using GridBoard.TileActions;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace UnitAction
{
    public abstract class Base
    {
        public GameObject GetIndicatorPrefab()
            => DataManager.current.unitActionPrefabs.Get(ActionVisualId);

        public virtual string GetShortDescription(Unit parent) => "";
        public virtual string GetLongDescription(Unit parent) => "";
        public virtual string GetTooltip(Unit parent) => "";

        public virtual string ActionVisualId => "Attack";
        public abstract IEnumerator Run(Unit parent, Unit target);
    }

    public class Attack : Base
    {
        int value;
        public override string GetShortDescription(Unit parent)
            => $"{value}";
        public override string GetLongDescription(Unit parent)
            => $"Will deal {value} damage";

        public Attack(int damage)
        {
            this.value = damage;
        }

        public override IEnumerator Run(Unit parent, Unit target)
        {
            parent.AnimAttack(1);
            yield return new WaitForSeconds(0.1f);
            if (!target || !parent) yield break;


            DataManager.current.CreateFX(parent.data.visuals.fxAttack, target.transform.position, ()=>
            {
                if (!target) return;
                target.RemoveHp(value);
            });
        }

        public class Builder : FactoryBuilder<Base, int>
        {
            public override Base Build() => new Attack(value);
        }
    }

    public class Armor : Base
    {
        int value;
        public override string GetShortDescription(Unit parent)
            => $"{value}";
        public override string GetLongDescription(Unit parent)
            => $"Will gain {value} armor";

        public override string ActionVisualId => "Armor";

        public Armor(int damage)
        {
            this.value = damage;
        }

        public override IEnumerator Run(Unit parent, Unit target)
        {
            parent.AnimAttack(2);
            yield return new WaitForSeconds(0.1f);
            if (!parent) yield break;

            AnimCompanion fx = null; //ToDo //parent.data.visuals.fxAttack;
            if (fx)
            {
                Object.Instantiate(fx, parent.transform.position, Quaternion.identity)
                    .SetTriggerAction(() =>
                    {
                        if (!parent) return;
                        parent.AddArmor(value);
                    });
            }
            else
            {
                parent.AddArmor(value);
            }
        }

        public class Builder : FactoryBuilder<Base, int>
        {
            public override Base Build() => new Armor(value);
        }
    }
}