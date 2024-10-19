using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FancyToolkit;
using GridBoard;

namespace TileActions
{
    public abstract class Base
    {
        protected MyTile parent;

        protected int Power => parent.Power;

        public abstract string GetDescription(MyTile parent);

        protected GenericBullet MakeBullet(Tile parent)
        {
            var rand = Random.Range(0, 2) == 0;
            var bullet = DataManager.current.gameData.prefabBullet.MakeInstance(parent.transform.position)
                .AddSpleen(rand ? Vector2.left : Vector2.right)
                .SetSprite(parent.GetIcon());

            return bullet;
        }
        /*
        protected int DamageAction(Unit src, Unit target, MyTile tile, int damage, LineClearData clearData = null)
        {
            if (tile.HasTag("sword") && src.GetModifier(Unit.Modifier.SwordAttack, out var swordAttk))
            {
                damage = Mathf.Max(0, damage + swordAttk);
            }

            if (clearData != null)
            {
                clearData.valTotalDamage += damage;
            }

            return damage;
        }

        protected int ArmorAction(Unit src, MyTile tile, int damage, LineClearData clearData = null)
        {
            if (tile.HasTag("sword") && src.GetModifier(Unit.Modifier.SwordAttack, out var swordAttk))
            {
                damage = Mathf.Max(0, damage + swordAttk);
            }

            if (clearData != null)
            {
                clearData.valTotalDamage += damage;
            }

            return damage;
        }*/

        public virtual void Init(MyTile tile)
        {
            this.parent = tile;
        }

        public virtual void Remove()
        {

        }
    }
}