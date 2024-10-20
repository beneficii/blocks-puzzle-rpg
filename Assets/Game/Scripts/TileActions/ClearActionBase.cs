using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using FancyToolkit;
using System.Linq;
using System;
using UnityEngine.Assertions;

namespace TileActions
{
    public abstract class ClearActionBase : TileActionBase
    {
        public override IEnumerator Run()
        {
            yield break;
        }
    }


    public class Defense : TileActionBase
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

        public override IEnumerator Run()
        {
            var bullet = MakeBullet(parent)
                            .SetTarget(CombatArena.current.player)
                            .SetAction(Action)
                            .SetLaunchDelay(0.2f);
            yield return new WaitForSeconds(.2f);
        }

        public class Builder : FactoryBuilder<TileActionBase>
        {
            public override TileActionBase Build() => new Defense();
        }
    }

    public class DamageMultiSpell : ClearActionBase
    {
        int damageMultiplier;

        public override string GetDescription(MyTile parent)
           => $"Deal {damageMultiplier}x swords matched in damage";

        public DamageMultiSpell(int damageMultiplier)
        {
            this.damageMultiplier = damageMultiplier;
        }

        public override IEnumerator Run(LineClearData match)
        {
            var captured = match.tiles
                .Where(x => x.data.type == Tile.Type.Weapon)
                .ToList();

            foreach (var tile in captured) {
                match.tiles.Remove(tile);

                yield return tile.FadeOut(8f);
                MakeBullet(tile)//, DataManager.current.vfxDict.Get("poof"))
                    .AddSpleen(Vector2.zero)
                    .SetSpeed(15)
                    .SetSprite(tile.data.visuals.sprite)
                    .SetTarget(parent);
                yield return new WaitForSeconds(.05f);
            }
            yield return new WaitForSeconds(.1f);

            yield return parent.FadeOut(10f);
            MakeBullet(parent)
                .SetTarget(CombatArena.current.enemy)
                .SetDamage(captured.Count * damageMultiplier);
        }

        public class Builder : FactoryBuilder<TileActionBase, int>
        {
            public override TileActionBase Build() => new DamageMultiSpell(value);
        }
    }


    public class ForCleared : ClearActionBase
    {
        int amount;
        string tag;
        TileActionBase nestedAction;

        public override string GetDescription(MyTile parent)
           => $"{nestedAction.GetDescription(parent)} for each {amount} {tag} cleared";

        public ForCleared(int amount, string tag, TileActionBase nestedAction)
        {
            this.amount = amount;
            this.tag = tag;
            this.nestedAction = nestedAction;
        }

        public override void Init(MyTile tile)
        {
            base.Init(tile);
            nestedAction.Init(tile);
        }

        public override IEnumerator Run(LineClearData match)
        {
            int totalCount = match.list.Count(x => x.HasTag(tag)) / amount;

            if (totalCount == 0) yield break;

            for (int i = 0; i < totalCount; i++)
            {
                yield return nestedAction.Run();
                yield return new WaitForSeconds(.1f);
            }

            yield return parent.FadeOut(10f);
        }

        public class Builder : FactoryBuilder<TileActionBase, int, string, FactoryBuilder<TileActionBase>>
        {
            public override TileActionBase Build() => new ForCleared(value1, value2, value3.Build());
        }
    }


    public class SpawnTile : TileActionBase
    {
        int count;
        string tileId;

        TileData GetData() => TileCtrl.current.GetTile(tileId);

        public SpawnTile(int count, string tileId)
        {
            this.count = count;
            this.tileId = tileId;
        }

        public override string GetDescription(MyTile parent) => $"Spawn {count} '{GetData().title}'";

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

        public override IEnumerator Run()
        {
            var data = GetData();
            if (data == null)
            {
                Debug.LogError($"Data with id `{tileId}` not found!");
                yield break;
            }

            for (int i = 0; i < count; i++)
            {
                var target = parent.board.TakeEmptyTile();
                if (!target) Debug.Log("no empty tiles");
                if (!target) yield break;

                MakeBullet(parent)
                    .SetTarget(target)
                    .SetSprite(data.visuals.sprite)
                    .SetAction(Spawn);

                yield return new WaitForSeconds(.05f);
            }
            yield return new WaitForSeconds(.1f);
        }

        public class Builder : FactoryBuilder<TileActionBase, int, string>
        {
            public override TileActionBase Build() => new SpawnTile(value1, value2);
        }
    }
}