using FancyToolkit;
using GridBoard;
using GridBoard.TileActions;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace TileActions
{
    public class Damage : TileActionBase
    {
        public override string GetDescription()
            => $"Deal {Power} damage";

        public override TileStatType StatType => TileStatType.Damage;

        public Damage()
        {
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var bullet = MakeDmgBullet(parent, Power * multiplier)
                            .SetTarget(CombatArena.current.enemy)
                            .SetLaunchDelay(0.2f);

            yield return new WaitForSeconds(.2f);
        }

        public class Builder : FactoryBuilder<TileActionBase>
        {
            public override TileActionBase Build() => new Damage();
        }
    }

    public class DamageAnd : TileActionBase
    {
        TileActionBase nestedAction;

        public override string GetDescription()
            => $"Deal {Power} damage and {nestedAction.GetDescription()}";

        public override TileStatType StatType => TileStatType.Damage;

        public DamageAnd(TileActionBase nestedAction)
        {
            this.nestedAction = nestedAction;
        }

        public override void Init(MyTile tile)
        {
            base.Init(tile);
            nestedAction.Init(tile);
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var bullet = MakeDmgBullet(parent, Power * multiplier)
                            .SetTarget(CombatArena.current.enemy)
                            .SetLaunchDelay(0.05f);

            yield return new WaitForSeconds(.1f);
            yield return nestedAction.Run(multiplier);
        }

        public class Builder : FactoryBuilder<TileActionBase, FactoryBuilder<TileActionBase>>
        {
            public override TileActionBase Build() => new DamageAnd(value.Build());
        }
    }

    public class DamagePlayer : TileActionBase
    {
        public override string GetDescription()
            => $"Deal {Power} damage to the player";

        public override TileStatType StatType => TileStatType.Damage;

        public override IEnumerator Run(int multiplier = 1)
        {
            var bullet = MakeDmgBullet(parent, Power * multiplier)
                            .SetTarget(CombatArena.current.player)
                            .SetLaunchDelay(0.2f);

            yield return new WaitForSeconds(.2f);
        }

        public class Builder : FactoryBuilder<TileActionBase>
        {
            public override TileActionBase Build() => new DamagePlayer();
        }
    }

    public class DamageBoth : TileActionBase
    {
        public override string GetDescription()
            => $"Deal {Power} damage to both player and enemy";

        public override TileStatType StatType => TileStatType.Damage;

        public override IEnumerator Run(int multiplier = 1)
        {
            MakeDmgBullet(parent, Power * multiplier)
                        .SetTarget(CombatArena.current.player)
                        .SetLaunchDelay(0.2f);

            MakeDmgBullet(parent, Power * multiplier)
                        .SetTarget(CombatArena.current.enemy)
                        .SetLaunchDelay(0.2f);

            yield return new WaitForSeconds(.2f);
        }

        public class Builder : FactoryBuilder<TileActionBase>
        {
            public override TileActionBase Build() => new DamageBoth();
        }
    }

    public class HealEnemy : TileActionBase
    {
        public override string GetDescription()
            => $"Restore {Power} hp to the enemy";

        public HealEnemy()
        {
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var bullet = MakeBullet(parent)
                            .SetTarget(CombatArena.current.enemy)
                            .SetHealing(Power * multiplier)
                            .SetLaunchDelay(0.2f);

            yield return new WaitForSeconds(.2f);
        }

        public class Builder : FactoryBuilder<TileActionBase>
        {
            public override TileActionBase Build() => new HealEnemy();
        }
    }

    public class AddPower : TileActionBase
    {
        int value;
        public override string GetDescription()
            => $"{value.SignedStr()} power to this tile";

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

    public class AddPowerForEachOnBoard : TileActionBase
    {
        int value;
        string tag;
        public override string GetDescription()
            => $"Add {value.SignedStr()} power to this tile for each {tag} on board";

        public AddPowerForEachOnBoard(int value, string tag)
        {
            this.value = value;
            this.tag = tag;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            bool hadTargets = false;
            foreach (var target in FindTileTargets(parent, ActionTargetType.All, tag))
            {
                hadTargets = true;
                MakeBullet(target)
                    .SetTarget(parent)
                    //.SetSpleen(default)
                    .SetTileAction(x => {
                        parent.Power += value * multiplier;
                    });
                yield return new WaitForSeconds(.05f);
            }
            if (hadTargets) yield return new WaitForSeconds(.1f);
        }

        public class Builder : FactoryBuilder<TileActionBase, int, string>
        {
            public override TileActionBase Build() => new AddPowerForEachOnBoard(value, value2);
        }
    }

    public class ConvertPowerTo : TileActionBase
    {
        TileStatType statType;
        ActionTargetType targetType;
        string tag;

        public override string GetDescription()
        {
            if (statType == TileStatType.Damage)
            {
                return $"Power from {GetTargetingTypeName(targetType, tag)} is dealt as damage";
            }
            else if (statType == TileStatType.Defense)
            {
                return $"Power from {GetTargetingTypeName(targetType, tag)} is added to your armor";
            }
            else
            {
                return "oops! (ConvertPowerTo)";
            }
        }

        public ConvertPowerTo(TileStatType statType, ActionTargetType targetType, string tag)
        {
            this.statType = statType;
            this.targetType = targetType;
            this.tag = tag;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            int total = 0;
            foreach (var target in FindTileTargets(parent, targetType, tag))
            {
                total += target.Power;
                var bullet = MakeBullet(target)
                    .SetTarget(parent)
                    .SetTileAction(x => {
                        if (!target) return;
                        parent.Init(target.data);
                        parent.Power = target.Power;
                    });

                yield return new WaitForSeconds(.2f);
                yield return new WaitWhile(() => bullet);
            }

            switch (statType)
            {
                case TileStatType.Damage:
                    MakeDmgBullet(parent, total * multiplier)
                            .SetTarget(CombatArena.current.enemy)
                            .SetLaunchDelay(0.05f);
                    break;
                case TileStatType.Defense:
                    MakeDefBullet(parent, total * multiplier)
                            .SetTarget(CombatArena.current.player)
                            .SetLaunchDelay(0.05f);
                    break;
                default:
                    Debug.LogError("Wrong stat type for ConvertPowerTo!");
                    break;
            }

            yield return new WaitForSeconds(.2f);
        }

        public class Builder : FactoryBuilder<TileActionBase, TileStatType, ActionTargetType, string>
        {
            public override TileActionBase Build() => new ConvertPowerTo(value, value2, value3);
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
            foreach (var tile in FindTileTargets(parent, targetType, x=>x.StatType == type))
            {
                
                MakeBullet(parent)
                    .SetTarget(tile)
                    .SetSpleen(default)
                    .SetTileAction(x => x.Power += value * multiplier);

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

    public class BuffPowerTag : TileActionBase
    {
        ActionTargetType targetType;
        int value;
        string tag;
        public override string GetDescription()
            => $"{value.SignedStr()} power to {GetTargetingTypeName(targetType, tag)}";

        public BuffPowerTag(ActionTargetType targetType, int value, string tag)
        {
            this.targetType = targetType;
            this.value = value;
            this.tag = tag;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            foreach (var tile in FindTileTargets(parent, targetType, tag))
            {

                MakeBullet(parent)
                    .SetTarget(tile)
                    .SetSpleen(default)
                    .SetTileAction(x => x.Power += value * multiplier);

                yield return new WaitForSeconds(.1f);
            }
            //yield return parent.ClickAnimationRoutine();
            //yield return new WaitForSeconds(.05f);
        }

        public class Builder : FactoryBuilder<TileActionBase, ActionTargetType, int, string>
        {
            public override TileActionBase Build() => new BuffPowerTag(value, value2, value3);
        }
    }

    public class TransformAround : TileActionBase
    {
        string tag;
        string id;
        int count;

        TileData GetData() => TileCtrl.current.Get(id);

        public override string GetDescription()
        {
            if (count < 9)
            {
                return $"Transform {count} surrounding {tag} tiles into '{GetData().title}'";
            }
            else
            {
                return $"Transform surrounding {tag} tiles into '{GetData().title}'";
            }
        }

        public TransformAround(string tag, string id, int count = 9)
        {
            this.tag = tag;
            this.id = id;
            if (count > 0)
            {
                this.count = count;
            }
            else
            {
                this.count = 9;
            }
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var data = GetData();
            var targets = FindTileTargets(parent, ActionTargetType.Around, tag).ToList();
            foreach (var item in targets.Shuffled().Take(count * multiplier))
            {
                MakeBullet(parent)
                        .SetTarget(item)
                        .SetSpleen(default)
                        .SetTileAction(x =>
                        {
                            x.Init(data);
                            x.isActionLocked = true;
                        });

                yield return new WaitForSeconds(.1f);
            }
        }

        public class Builder : FactoryBuilder<TileActionBase, string, string, int>
        {
            public override TileActionBase Build() => new TransformAround(value, value2, value3);
        }
    }

    public class TransformIn : TileActionBase
    {
        ActionTargetType targetType;
        string tag;
        string id;

        TileData GetData() => TileCtrl.current.Get(id);

        public override string GetDescription()
            => $"Transform {GetTargetingTypeName(targetType, tag)} into '{GetData().title}'";

        public TransformIn(ActionTargetType targetType, string tag, string id)
        {
            this.targetType = targetType;
            this.tag = tag;
            this.id = id;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var data = GetData();
            foreach (var item in FindTileTargets(parent, targetType, tag))
            {
                var bullet = MakeBullet(parent)
                            .SetTarget(item)
                            .SetTileAction(x =>
                            {
                                x.Init(data);
                                x.isActionLocked = true;
                            });

                yield return new WaitForSeconds(.1f);
            }
        }

        public class Builder : FactoryBuilder<TileActionBase, ActionTargetType, string, string>
        {
            public override TileActionBase Build() => new TransformIn(value, value2, value3);
        }
    }

    public class SuckOutPower : TileActionBase
    {
        ActionTargetType targetType;
        string tag;
        string id;

        TileData GetData() => TileCtrl.current.Get(id);

        public override string GetDescription()
        {
            var descr = $"Steal power from {GetTargetingTypeName(targetType, tag)}";

            if (id == "empty")
            {
                descr += " and clear them";
            }
            else
            {
                descr += $" and turn them into '{GetData().title}'";
            }

            return descr;
        }

        public SuckOutPower(ActionTargetType targetType, string tag, string id)
        {
            this.targetType = targetType;
            this.tag = tag;
            this.id = id;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var data = GetData();
            foreach (var item in FindTileTargets(parent, targetType, tag))
            {
                int pwr = item.Power;
                MakeBullet(item)
                        .SetTarget(parent)
                        .SetTileAction(x =>
                        {
                            x.Power += pwr;
                        });

                item.Init(data);
                item.isActionLocked = true;

                yield return new WaitForSeconds(.2f);
            }
        }

        public class Builder : FactoryBuilder<TileActionBase, ActionTargetType, string, string>
        {
            public override TileActionBase Build() => new SuckOutPower(value, value2, value3);
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