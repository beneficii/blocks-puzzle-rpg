using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using FancyToolkit;
using System.Linq;

namespace UnitAction
{
    public abstract class Base
    {
        protected Unit parent;
        public GameObject GetIndicatorPrefab()
            => Game.current.unitActionPrefabs.Get(ActionVisualId);

        public virtual string GetShortDescription() => "";
        public virtual string GetLongDescription() => "";
        public virtual string GetTooltip() => "";

        public virtual string ActionVisualId => "Attack";
        public abstract IEnumerator Run(Unit target);

        public virtual void Init(Unit parent)
        {
            this.parent = parent;
        }

        protected GenericBullet MakeBullet(string vfxId = null)
        {
            var rand = Random.Range(0, 2) == 0;
            var bullet = Game.current.MakeBullet(parent.transform.position, vfxId)
                .SetSpleen(rand ? Vector2.left : Vector2.right);

            return bullet;
        }
    }

    public class Attack : Base
    {
        int value;
        public override string GetShortDescription()
            => $"{value * parent.damage}";
        public override string GetLongDescription()
        {
            if (value > 1)
            {
                return $"Will deal {parent.GetDamage()}x{value} damage";
            }
            else
            {
                return $"Will deal {parent.GetDamage()} damage";
            }
        }

        public Attack(int damage)
        {
            this.value = damage;
            if (value == 0) value = 1;
        }

        public override IEnumerator Run(Unit target)
        {
            parent.AnimAttack(1);
            yield return new WaitForSeconds(0.1f);
            if (!target || !parent) yield break;

            int damage = parent.GetDamage() * value;
            bool isFxFinished = false;
            Game.current.CreateFX(parent.data.visuals.fxAttack, target.transform.position, () =>
            {
                isFxFinished = true;
                if (!target) return;
                target.RemoveHp(damage);
            });

            yield return new WaitUntil(() => isFxFinished);
            yield return new WaitForSeconds(0.05f);
        }

        public class Builder : FactoryBuilder<Base, int>
        {
            public override Base Build() => new Attack(value);
        }
    }

    public class Armor : Base
    {
        int value;
        public override string GetShortDescription()
            => $"{value * parent.GetDefense()}";
        public override string GetLongDescription()
        {
            if (value > 1)
            {
                return $"Will gain {parent.GetDefense()}x{value} armor";
            }
            else
            {
                return $"Will gain {parent.GetDefense()} armor";
            }
        }
        public override string ActionVisualId => "Armor";

        public Armor(int damage)
        {
            this.value = damage;
            if (value == 0) value = 1;
        }

        public override IEnumerator Run(Unit target)
        {
            parent.AnimAttack(2);
            yield return new WaitForSeconds(0.1f);
            if (!parent) yield break;
            int defense = parent.GetDefense() * value;
            AnimCompanion fx = null; //ToDo //parent.data.visuals.fxAttack;
            if (fx)
            {
                Object.Instantiate(fx, parent.transform.position, Quaternion.identity)
                    .SetTriggerAction(() =>
                    {
                        if (!parent) return;
                        parent.AddArmor(defense);
                    });
            }
            else
            {
                parent.AddArmor(defense);
            }
        }

        public class Builder : FactoryBuilder<Base, int>
        {
            public override Base Build() => new Armor(value);
        }
    }

    public class SpawnTile : Base
    {
        int count;
        string tileId;

        TileData GetData() => TileCtrl.current.Get(tileId ?? parent.data.specialTile);

        public SpawnTile(int count, string tileId)
        {
            this.count = count > 0 ? count : 1;
            this.tileId = tileId;
        }

        public override string GetShortDescription()
            => $"{count}";

        public override string GetLongDescription()
        {
            var data = GetData();
            if (count == 1)
            {
                return $"Will spawn '{data.title}'.";
            }
            else
            {
                return $"Will spawn {count} '{data.title}'.";
            }
        }

        public override string ActionVisualId => "SpawnTile";


        void Spawn(MyTile tile)
        {
            tile.SetBoard(parent.board);
            tile.Init(GetData());
            tile.isActionLocked = true;
        }

        public override IEnumerator Run(Unit target)
        {
            var data = GetData();
            if (data == null)
            {
                Debug.LogError($"Data with id `{tileId}` not found!");
                yield break;
            }

            parent.AnimAttack(2);
            yield return new WaitForSeconds(0.1f);
            if (!parent) yield break;

            for (int i = 0; i < count; i++)
            {
                var tile = parent.board.TakeEmptyTile();
                if (!tile) yield break;

                MakeBullet()
                    .SetTarget(tile)
                    .SetSprite(data.sprite)
                    .SetTileAction(Spawn);

                yield return new WaitForSeconds(.05f);
            }
            yield return new WaitForSeconds(0.1f);
        }

        public class Builder : FactoryBuilder<Base, int, string>
        {
            public override Base Build() => new SpawnTile(value, value2);
        }
    }
}