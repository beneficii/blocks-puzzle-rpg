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
    public abstract class Base : TileActions.Base
    {
        public abstract void Run(LineClearData match);
    }

    public class Damage : Base
    {
        public override string GetDescription(MyTile parent)
            => $"Deal {Power} damage";

        public Damage()
        {
        }

        public override void Run(LineClearData match)
        {
            var bullet = MakeBullet(parent)
                            .SetTarget(CombatArena.current.enemy)
                            .SetDamage(Power)
                            .SetLaunchDelay(0.2f);
        }

        public class Builder : FactoryBuilder<Base>
        {
            public override Base Build() => new Damage();
        }
    }

    public class Defense : Base
    {
        public override string GetDescription(MyTile parent)
            => $"Gain {Power} defense";

        public Defense()
        {
        }

        void Action(Component comp)
        {
            if (!comp || comp is not Unit unit) return;

            unit.AddArmor(Power);
        }

        public override void Run(LineClearData match)
        {
            var bullet = MakeBullet(parent)
                            .SetTarget(CombatArena.current.player)
                            .SetAction(Action)
                            .SetLaunchDelay(0.2f);
        }

        public class Builder : FactoryBuilder<Base>
        {
            public override Base Build() => new Defense();
        }
    }

    public class Heal : Base
    {
        int value;
        public override string GetDescription(MyTile parent)
            => $"Heal {value} hp";

        public Heal(int value)
        {
            this.value = value;
        }

        void Action(Component comp)
        {
            if (!comp || comp is not Unit unit) return;

            DataManager.current.CreateFX("heal1", unit.transform.position);
            unit.AddHp(value);
        }

        public override void Run(LineClearData match)
        {
            var bullet = MakeBullet(parent)
                            .SetTarget(CombatArena.current.player)
                            .SetAction(Action)
                            .SetLaunchDelay(0.2f);
        }

        public class Builder : FactoryBuilder<Base, int>
        {
            public override Base Build() => new Heal(value);
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

        public override void Run(LineClearData match)
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

        void Spawn(Component component)
        {
            if (component is not Tile tile)
            {
                Debug.LogError("SpawnTile: Component not a tile");
                return;
            }
            tile.Init(GetData());
            tile.InitBoard(parent.board);
        }

        public override void Run(LineClearData match)
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
                    .SetAction(Spawn)
                    .SetLaunchDelay(0.2f);
            }
        }

        public class Builder : FactoryBuilder<Base, int, string>
        {
            public override Base Build() => new SpawnTile(value1, "fireball");
        }
    }
}