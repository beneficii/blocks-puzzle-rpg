using FancyToolkit;
using GridBoard;
using System.Collections;
using System.Globalization;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace GameActions
{
    public class ConsumeTagAnd : ActionBase
    {
        int amount;
        string tag;
        TileTargetingType targetType;
        ActionBase nestedAction;

        public override string GetDescription()
        {
            var descr = $"Clean {MyTile.GetTargetingTypeName(targetType, tag)} and {nestedAction.GetDescription()} for each";
            if (amount > 1) descr += $" {amount}";

            return descr;
        }

        public ConsumeTagAnd(int amount, string tag, TileTargetingType targetType, ActionBase nestedAction)
        {
            this.amount = amount;
            this.tag = tag;
            this.targetType = targetType;
            this.nestedAction = nestedAction;
        }

        public override void Init(IActionParent parent)
        {
            base.Init(parent);
            nestedAction.Init(parent);
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            int count = 0;
            foreach (var target in MyTile.FindTileTargets(parent, targetType, (x) => x.HasTag(tag) && x.Power > 0))
            {
                count++;
                MakeBullet(target)
                    .SetTarget(parent.transform)
                    .SetSpleen(default);
                target.Init(TileCtrl.current.emptyTile);
                yield return new WaitForSeconds(0.1f);
            }

            if (count < 1) yield break;

            yield return nestedAction.Run(multiplier * count);
            yield return new WaitForSeconds(.1f);
        }

        public class Builder : FactoryBuilder<ActionBase, int, string, TileTargetingType, FactoryBuilder<ActionBase>>
        {
            public override ActionBase Build()
            {
                return new ConsumeTagAnd(value, value2, value3, value4.Build());
            }
        }
    }

    public class TriggerPlace : ActionBase
    {
        string tag;
        TileTargetingType targetType;

        public override string GetDescription()
        {
            return $"Trigger \"Enter\" effects of {MyTile.GetTargetingTypeName(targetType, tag)}";
        }

        public TriggerPlace(string tag, TileTargetingType targetType)
        {
            this.tag = tag;
            this.targetType = targetType;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var targets = MyTile.FindTileTargets(parent, targetType, (x) =>
            {
                if (x.actionContainer?.enterAction == null) return false;
                if (!x.HasTag(tag)) return false;
                if (x.actionContainer.enterAction is TriggerPlace) return false;

                return true;
            });
            foreach (var target in targets)
            {
                yield return MakeBullet(parent)
                    .SetTarget(target)
                    .SetSpleen(default)
                    .Wait();

                if (!target) continue;
                var enterAction = target.actionContainer?.enterAction;
                if (enterAction == null) continue;
                yield return enterAction.Run(multiplier);
            }

            yield return new WaitForSeconds(.1f);
        }

        public class Builder : FactoryBuilder<ActionBase, string, TileTargetingType>
        {
            public override ActionBase Build()
                => new TriggerPlace(value, value2);
        }
    }

    public class ConsumeArmorAnd : ActionBase
    {
        int amount;
        ActionBase nestedAction;

        public override string GetDescription()
        {
            if (amount == 1)
            {
                return $"Consume all armor and {nestedAction.GetDescription()} for each point";
            }
            else
            {
                return $"Consume all armor and {nestedAction.GetDescription()} for each {amount} points";
            }
        }

        public ConsumeArmorAnd(int amount, ActionBase nestedAction)
        {
            this.amount = amount;
            this.nestedAction = nestedAction;
        }

        public override void Init(IActionParent parent)
        {
            base.Init(parent);
            nestedAction.Init(parent);
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var player = CombatArena.current.player;
            if (!player) yield break;
            var multi = player.GetArmor() / amount;
            if (multi <= 0)
            {
                yield break;
            }
            // remove armor to turn into stuff
            player.SetArmor(0);
            //player.SetArmor(player.GetArmor() - (amount * multiplier));

            MakeBullet(parent, player.transform.position)
                    .SetTarget(parent.transform);
            yield return new WaitForSeconds(.25f);

            yield return nestedAction.Run(multi * multiplier);
            yield return new WaitForSeconds(.1f);
        }

        public class Builder : FactoryBuilder<ActionBase, int, FactoryBuilder<ActionBase>>
        {
            public override ActionBase Build() => new ConsumeArmorAnd(value, value2.Build());
        }
    }

    public class ConsumeDamage : ActionBase
    {
        string tag;
        TileTargetingType targetType;

        public override string GetDescription()
        {
            var descr = $"Empty {MyTile.GetTargetingTypeName(targetType, tag)} and gain their Power";
            return descr;
        }

        public ConsumeDamage(string tag, TileTargetingType targetType)
        {
            this.tag = tag;
            this.targetType = targetType;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            //foreach (var target in MyTile.FindTileTargets(parent, targetType, (x) => x.HasTag(tag) && x.Power > 0))
            foreach (var target in MyTile.FindTileTargets(parent, targetType, (x) => {
                return x.HasTag(tag) && x.Power > 0;
            }))
            {
                int pwr = target.Power;
                MakeBullet(target)
                    .SetTarget(parent.AsComponent())
                    .SetSpleen(default)
                    .SetTileAction(x => {
                        parent.Power += pwr;
                    });
                target.Init(TileCtrl.current.emptyTile);
                yield return new WaitForSeconds(.15f);
            }
        }

        public class Builder : FactoryBuilder<ActionBase, string, TileTargetingType>
        {
            public override ActionBase Build() => new ConsumeDamage(value, value2);
        }
    }


    public class ConvertPowerTo : ActionBase
    {
        ActionStatType statType;
        TileTargetingType targetType;
        string tag;

        public override string GetDescription()
        {
            if (statType == ActionStatType.Damage)
            {
                return $"Power from {MyTile.GetTargetingTypeName(targetType, tag)} is dealt as damage";
            }
            else if (statType == ActionStatType.Defense)
            {
                return $"Power from {MyTile.GetTargetingTypeName(targetType, tag)} is added to your armor";
            }
            else
            {
                return "oops! (ConvertPowerTo)";
            }
        }

        public ConvertPowerTo(ActionStatType statType, TileTargetingType targetType, string tag)
        {
            this.statType = statType;
            this.targetType = targetType;
            this.tag = tag;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            int total = 0;
            foreach (var target in MyTile.FindTileTargets(parent, targetType, tag))
            {
                total += target.Power;
                var bullet = MakeBullet(target)
                    .SetTarget(parent.AsComponent());

                yield return new WaitForSeconds(.2f);
                yield return new WaitWhile(() => bullet);
            }

            switch (statType)
            {
                case ActionStatType.Damage:
                    SetBulletDamage(MakeBullet(parent)
                            .SetTarget(CombatArena.current.enemy)
                            .SetLaunchDelay(0.05f)
                            , total * multiplier);
                    break;
                case ActionStatType.Defense:
                    SetBulletDefense(MakeBullet(parent)
                            .SetTarget(CombatArena.current.player)
                            .SetLaunchDelay(0.05f)
                            , total * multiplier);
                    break;

                default:
                    Debug.LogError("Wrong stat type for ConvertPowerTo!");
                    break;
            }

            yield return new WaitForSeconds(.2f);
        }

        public class Builder : FactoryBuilder<ActionBase, ActionStatType, TileTargetingType, string>
        {
            public override ActionBase Build() => new ConvertPowerTo(value, value2, value3);
        }
    }

    public class SuckOutPower : ActionBase
    {
        TileTargetingType targetType;
        string tag;
        string id;

        TileData GetData() => TileCtrl.current.Get(id);

        public override string GetDescription()
        {
            var descr = $"Steal power from {MyTile.GetTargetingTypeName(targetType, tag)}";

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

        public SuckOutPower(TileTargetingType targetType, string tag, string id)
        {
            this.targetType = targetType;
            this.tag = tag;
            this.id = id;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var data = GetData();
            foreach (var item in MyTile.FindTileTargets(parent, targetType, tag))
            {
                int pwr = item.Power;
                MakeBullet(item)
                        .SetTarget(parent.AsComponent())
                        .SetTileAction(x =>
                        {
                            x.Power += pwr;
                        });

                item.Init(data);
                item.isActionLocked = true;

                yield return new WaitForSeconds(.2f);
            }
        }

        public class Builder : FactoryBuilder<ActionBase, TileTargetingType, string, string>
        {
            public override ActionBase Build() => new SuckOutPower(value, value2, value3);
        }
    }

    public class ForOnBoard : ActionBase
    {
        int amount;
        ActionBase nestedAction;
        string id;

        public override string GetDescription()
           => $"For every {(amount > 1 ? $"{amount}" : "")} `{TileCtrl.current.Get(id).title}` on board {nestedAction.GetDescription()}";

        public ForOnBoard(string id, int amount, ActionBase nestedAction)
        {
            if (amount < 1) amount = 1;
            this.amount = amount;
            this.id = id;
            this.nestedAction = nestedAction;
        }

        public override void Init(IActionParent parent)
        {
            base.Init(parent);
            nestedAction.Init(parent);
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var count = 0;
            foreach (var item in parent.board
                    .GetAllTiles(x => x.data.id == id && !x.isActionLocked))
            {
                if (parent is Tile tile && tile.data.id == id)
                {
                    item.isActionLocked = true;
                }
                count++;
            }

            count /= amount;

            if (count < 1)
            {
                yield break;
            }

            yield return nestedAction.Run();
            yield return new WaitForSeconds(.05f);
        }

        public class Builder : FactoryBuilder<ActionBase, string, int, FactoryBuilder<ActionBase>>
        {
            public override ActionBase Build() => new ForOnBoard(value, value2, value3.Build());
        }
    }
}