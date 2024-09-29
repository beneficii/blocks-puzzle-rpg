using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using FancyToolkit;
using static UnityEngine.Rendering.DebugUI;
using System.Linq;
using static UnityEditor.Progress;
using GridBoard.TileActions;

namespace ClearAction
{
    public abstract class Base
    {
        public abstract string GetDescription(MyTile parent);
        public abstract void Run(MyTile parent, LineClearData match);

        protected GenericBullet MakeBullet(Tile parent, AnimCompanion fxPrefab = null)
        {
            var rand = Random.Range(0, 2) == 0;
            var bullet = DataManager.current.gameData.prefabBullet.MakeInstance(parent.transform.position)
                .AddSpleen(rand ? Vector2.left : Vector2.right)
                .SetSprite(parent.GetIcon());


            if (fxPrefab) bullet.SetFx(fxPrefab);

            return bullet;
        }
    }


    public class Damage : Base
    {
        int damage;
        public override string GetDescription(MyTile parent)
            => $"Deal {damage} damage";

        public Damage(int damage)
        {
            this.damage = damage;   
        }

        public override void Run(MyTile parent, LineClearData match)
        {
            var bullet = MakeBullet(parent)
                            .SetTarget(CombatArena.current.enemy)
                            .SetDamage(damage)
                            .SetLaunchDelay(0.2f);
        }

        public class Builder : FactoryBuilder<Base, int>
        {
            public override Base Build() => new Damage(value);
        }
    }

    public class Defense : Base
    {
        int value;
        public override string GetDescription(MyTile parent)
            => $"Gain {value} defense";

        public Defense(int value)
        {
            this.value = value;
        }

        void Action(Component comp)
        {
            if (!comp || comp is not Unit unit) return;

            unit.AddArmor(value);
        }

        public override void Run(MyTile parent, LineClearData match)
        {
            var bullet = MakeBullet(parent)
                            .SetTarget(CombatArena.current.player)
                            .SetAction(Action)
                            .SetLaunchDelay(0.2f);
        }

        public class Builder : FactoryBuilder<Base, int>
        {
            public override Base Build() => new Defense(value);
        }
    }

    public class DamageMultiSpell : Base
    {
        int damageMultiplier;

        public override string GetDescription(MyTile parent)
           => $"Deal {damageMultiplier}x swords matched in damage";

        public DamageMultiSpell(int damageMultiplier)
        {
            this.damageMultiplier = damageMultiplier;
        }

        public override void Run(MyTile parent, LineClearData match)
        {
            var captured = match.tiles
                .Where(x => x.data.type == Tile.Type.Weapon)
                .ToList();

            foreach (var tile in captured) {
                match.tiles.Remove(tile);

                MakeBullet(tile)//, DataManager.current.vfxDict.Get("poof"))
                    .AddSpleen(Vector2.zero)
                    .SetSpeed(15)
                    .SetSprite(tile.data.visuals.sprite)
                    .SetTarget(parent)
                    .SetLaunchDelay(0.1f);
            }

            MakeBullet(parent)
                .SetTarget(CombatArena.current.enemy)
                .SetDamage(captured.Count * damageMultiplier)
                .SetLaunchDelay(.6f);
        }

        public class Builder : FactoryBuilder<Base, int>
        {
            public override Base Build() => new DamageMultiSpell(value);
        }
    }


    public class SpawnTile : Base
    {
        int count;
        string tileId;

        TileData GetData() => TileCtrl.current.GetTile(tileId);

        public SpawnTile(int count, string tileId)
        {
            this.count = count;
            this.tileId = tileId;
        }

        public override string GetDescription(MyTile parent) => $"Spawns {count} '{GetData().title}' on empty tiles";

        public override void Run(MyTile parent, LineClearData match)
        {
            var data = GetData();
            if (data == null)
            {
                Debug.LogError($"Data with id `{tileId}` not found!");
                return;
            }

            for (int i = 0; i < count; i++)
            {
                if (!match.emptyTiles.TryDequeue(out var target)) return;

                MakeBullet(parent)
                    .SetTarget(target)
                    .SetSprite(data.visuals.sprite)
                    .SetAction(x => (x as Tile)?.Init(data))
                    .SetLaunchDelay(0.2f);
            }
        }

        public class Builder : FactoryBuilder<Base, int, string>
        {
            public override Base Build() => new SpawnTile(value1, "fireball");
        }
    }
}