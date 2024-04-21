using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

namespace CombatBlock
{
    public class Base : BtBlockData
    {
        public override BtBlockType type => BtBlockType.Basic;
        protected CombatArena Arena => CombatArena.current;

        public List<BlockTag> tags;

        public bool HasTag(BlockTag tag) => tags.Contains(tag);

        protected GenericBullet MakeBullet(Vector2 origin)
        {
            var bullet = DataManager.current.gameData.prefabBullet.MakeInstance(origin)
                    .SetSprite(sprite);

            return bullet;
        }
    }
}


public enum BlockTag
{
    None,
    Sword,
    Shield,
    Staff,
    Spell
}
