using FancyToolkit;
using GridBoard;
using GridBoard.TileActions;
using System.Collections;
using System.Linq;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;

namespace TileActions
{
    public class Damage : TileActionBase
    {
        public override string GetDescription()
            => $"Deal {Power} damage";

        public Damage()
        {
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var bullet = MakeBullet(parent)
                            .SetTarget(CombatArena.current.enemy)
                            .SetDamage(Power * multiplier)
                            .SetLaunchDelay(0.2f);

            yield return new WaitForSeconds(.2f);
        }

        public class Builder : FactoryBuilder<TileActionBase>
        {
            public override TileActionBase Build() => new Damage();
        }
    }

    public class DamagePlayer : TileActionBase
    {
        public override string GetDescription()
            => $"Deal {Power} damage to the player";

        public DamagePlayer()
        {
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var bullet = MakeBullet(parent)
                            .SetTarget(CombatArena.current.player)
                            .SetDamage(Power * multiplier)
                            .SetLaunchDelay(0.2f);

            yield return new WaitForSeconds(.2f);
        }

        public class Builder : FactoryBuilder<TileActionBase>
        {
            public override TileActionBase Build() => new DamagePlayer();
        }
    }

    public class AddPower : TileActionBase
    {
        int value;
        public override string GetDescription()
            => $"Increase this tile {parent.myData.powerType} by {value}";

        public AddPower(int value)
        {
            this.value = value;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            parent.Power += value;
            yield return new WaitForSeconds(.15f);
        }

        public class Builder : FactoryBuilder<TileActionBase, int>
        {
            public override TileActionBase Build() => new AddPower(value);
        }
    }


    public class TransformInto : TileActionBase
    {
        ActionTargetType targetType;
        string tag;

        public override string GetDescription()
        {
            return $"Transform into a copy of {GetTargetingTypeName(targetType, tag)}";
        }

        public TransformInto(ActionTargetType targetType, string tag = TileData.anyTag)
        {
            this.targetType = targetType;
            this.tag = tag;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var target = FindTileTargets(parent, targetType, x => x.data.type != Tile.Type.Empty && x.HasTag(tag))
                .FirstOrDefault();

            if (target == null) yield break;

            MakeBullet(target)
                    .SetTarget(parent)
                    .SetSpleen(default)
                    .SetTileAction(x => {
                        if (!target) return;
                        parent.Init(target.data);
                        parent.Power = target.Power;
                    });

            yield return new WaitForSeconds(.15f);
        }

        public class Builder : FactoryBuilder<TileActionBase, ActionTargetType>
        {
            string tag;
            public override void Init(StringScanner scanner)
            {
                base.Init(scanner);
                scanner.TryGet(out tag, TileData.anyTag);
            }

            public override TileActionBase Build() => new TransformInto(value, tag);
        }
    }


    public class BuffPower : TileActionBase
    {
        ActionTargetType targetType;
        int value;
        TileStatType type;
        public override string GetDescription()
            => $"Add {value} {type} to {GetTargetingTypeName(targetType)}";

        public BuffPower(ActionTargetType targetType, int value, TileStatType type)
        {
            this.targetType = targetType;
            this.value = value;
            this.type = type;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            foreach (var tile in FindTileTargets(parent, targetType, x=>x.data is MyTileData data && data.powerType == type))
            {
                
                MakeBullet(parent)
                    .SetTarget(tile)
                    .SetSpleen(default)
                    .SetTileAction(x=> x.Power += value * multiplier);
                //tile.Power += value * multiplier;

                yield return new WaitForSeconds(.1f);
            }
            //yield return parent.ClickAnimationRoutine();
            //yield return new WaitForSeconds(.05f);
        }

        public class Builder : FactoryBuilder<TileActionBase, ActionTargetType, int, TileStatType>
        {
            public override TileActionBase Build() => new BuffPower(value, value2, value3);
        }
    }

    public class TransformAround : TileActionBase
    {
        string tag;
        string id;

        TileData GetData() => TileCtrl.current.GetTile(id);

        public override string GetDescription()
            => $"Transform surrounding {tag} tiles into '{GetData().title}'";

        public TransformAround(string tag, string id)
        {
            this.tag = tag;
            this.id = id;
        }


        public override IEnumerator Run(int multiplier = 1)
        {
            var data = GetData();
            foreach (var item in parent.board.GetTilesAround(parent.position.x, parent.position.y))
            {
                if (item.isBeingPlaced
                    || !item.data.HasTag(tag)) continue;

                var pos = item.position;
                item.Init(data);
                item.isActionLocked = true;

                yield return new WaitForSeconds(.1f);
            }
        }

        public class Builder : FactoryBuilder<TileActionBase, string, string>
        {
            public override TileActionBase Build() => new TransformAround(value, value2);
        }
    }

    public class ForOnBoard : TileActionBase
    {
        int amount;
        TileActionBase nestedAction;

        public override string GetDescription()
           => $"For every {amount} `{parent.data.title}` on board {nestedAction.GetDescription()}";

        public ForOnBoard(int amount, TileActionBase nestedAction)
        {
            this.amount = amount;
            this.nestedAction = nestedAction;
        }

        public override void Init(MyTile tile)
        {
            base.Init(tile);
            nestedAction.Init(tile);
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            parent.isActionLocked = true;
            if (amount > 1)
            {
                string id = parent.data.id;
                int needed = amount - 1;
                var others = parent.board
                    .GetAllTiles(x => x.data.id == id && !x.isActionLocked)
                    .Take(needed)
                    .ToList();

                if (others.Count < needed) yield break;

                foreach (var item in others)
                {
                    item.isActionLocked = true;
                }
            }

            yield return nestedAction.Run();
            yield return new WaitForSeconds(.05f);
        }

        public class Builder : FactoryBuilder<TileActionBase, int, FactoryBuilder<TileActionBase>>
        {
            public override TileActionBase Build() => new ForOnBoard(value, value2.Build());
        }
    }
}