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
        public override IEnumerator Run(int multiplier = 1)
        {
            yield break;
        }
    }

    public class Defense : TileActionBase
    {
        public override string GetDescription()
            => $"Gain {Power} armor";

        public override TileStatType StatType => TileStatType.Defense;

        public override IEnumerator Run(int multiplier = 1)
        {
            var bullet = MakeDefBullet(parent, Power * multiplier)
                            .SetTarget(CombatArena.current.player)
                            .SetLaunchDelay(0.09f);
            yield return new WaitForSeconds(.07f);
        }

        public class Builder : FactoryBuilder<TileActionBase>
        {
            public override TileActionBase Build() => new Defense();
        }
    }

    public class CopyHighestDamage : ClearActionBase
    {
        public override string GetDescription()
            => $"Copy highest damage in a clear";

        public override IEnumerator Run(LineClearData match)
        {
            MakeDmgBullet(parent, match.GetValueMax(keyDamage))
                .SetTarget(CombatArena.current.enemy)
                .SetLaunchDelay(0.2f);

            yield return parent.FadeOut(10f);
        }

        public class Builder : FactoryBuilder<TileActionBase>
        {
            public override TileActionBase Build() => new CopyHighestDamage();
        }
    }

    public class RemoveArmor : TileActionBase
    {
        public override string GetDescription()
            => $"Remove {Power} armor";

        public RemoveArmor()
        {

        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var bullet = MakeBullet(parent)
                            .SetTarget(CombatArena.current.player)
                            .SetUnitAction((x) => x.RemoveArmor(Power * multiplier))
                            .SetLaunchDelay(0.09f);
            yield return new WaitForSeconds(.07f);
        }

        public class Builder : FactoryBuilder<TileActionBase>
        {
            public override TileActionBase Build() => new RemoveArmor();
        }
    }

    public class EnemyDefense : TileActionBase
    {
        public override string GetDescription()
            => $"Enemy gains {Power} armor";

        public override TileStatType StatType => TileStatType.Defense;

        public EnemyDefense()
        {
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            MakeDefBullet(parent, Power * multiplier)
                        .SetTarget(CombatArena.current.enemy)
                        .SetLaunchDelay(0.05f);
            yield return new WaitForSeconds(.1f);
        }

        public class Builder : FactoryBuilder<TileActionBase>
        {
            public override TileActionBase Build() => new EnemyDefense();
        }
    }

    public class ConsumeArmorAnd : TileActionBase
    {
        int amount;
        TileActionBase nestedAction;

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

        public ConsumeArmorAnd(int amount, TileActionBase nestedAction)
        {
            this.amount = amount;
            this.nestedAction = nestedAction;
        }

        public override void Init(MyTile tile)
        {
            base.Init(tile);
            nestedAction.Init(tile);
        }

        void Action(Component comp)
        {
            if (!comp || comp is not Unit unit) return;

            unit.AddArmor(Power);
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

            var bullet = MakeBullet(parent, player.transform.position)
                            .SetTarget(parent);
            yield return new WaitForSeconds(.25f);

            yield return nestedAction.Run(multi * multiplier);
            yield return new WaitForSeconds(.1f);
        }

        public class Builder : FactoryBuilder<TileActionBase, int, FactoryBuilder<TileActionBase>>
        {
            public override TileActionBase Build() => new ConsumeArmorAnd(value, value2.Build());
        }
    }

    public class ConsumeTagAnd : ClearActionBase
    {
        int amount;
        string tag;
        ActionTargetType targetType;
        TileActionBase nestedAction;

        public override string GetDescription()
        {
            var descr = $"Clean {GetTargetingTypeName(targetType, tag)} and {nestedAction.GetDescription()} for each";
            if (amount > 1) descr += $" {amount}";

            return descr;
        }

        public ConsumeTagAnd(int amount, string tag, ActionTargetType targetType, TileActionBase nestedAction)
        {
            this.amount = amount;
            this.tag = tag;
            this.targetType = targetType;
            this.nestedAction = nestedAction;
        }

        public override void Init(MyTile tile)
        {
            base.Init(tile);
            nestedAction.Init(tile);
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            int count = 0;
            foreach (var target in FindTileTargets(parent, targetType, (x) => x.HasTag(tag) && x.Power > 0))
            {
                count++;
                MakeBullet(target)
                    .SetTarget(parent)
                    .SetSpleen(default);
                target.Init(TileCtrl.current.emptyTile);
                yield return new WaitForSeconds(0.1f);
            }

            if (count < 1) yield break;

            yield return nestedAction.Run(multiplier * count);
            yield return new WaitForSeconds(.1f);
        }

        public class Builder : FactoryBuilder<TileActionBase, int, string, ActionTargetType, FactoryBuilder<TileActionBase>>
        {
            public override void Init(StringScanner scanner)
            {
                base.Init(scanner);
            }
            public override TileActionBase Build()
            {
                return new ConsumeTagAnd(value, value2, value3, value4.Build());
            }
        }
    }

    public class ConsumePower : TileActionBase
    {
        string tag;
        ActionTargetType targetType;

        public override string GetDescription()
        {
            var descr = $"Clean {GetTargetingTypeName(targetType, tag)} and gain their power";
            return descr;
        }

        public ConsumePower(string tag, ActionTargetType targetType)
        {
            this.tag = tag;
            this.targetType = targetType;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            //foreach (var target in FindTileTargets(parent, targetType, (x) => x.HasTag(tag) && x.Power > 0))
            foreach (var target in FindTileTargets(parent, targetType, (x) => {
                return x.HasTag(tag) && x.Power > 0;
            }))
            {
                int pwr = target.Power;
                MakeBullet(target)
                    .SetTarget(parent)
                    .SetSpleen(default)
                    .SetTileAction(x => {
                        parent.Power += pwr;
                    });
                target.Init(TileCtrl.current.emptyTile);
                yield return new WaitForSeconds(.15f);
            }
        }

        public class Builder : FactoryBuilder<TileActionBase, string, ActionTargetType>
        {
            public override TileActionBase Build() => new ConsumePower(value, value2);
        }
    }

    public class ConsumeClearedAnd : ClearActionBase
    {
        int amount;
        string tag;
        TileActionBase nestedAction;

        public override string GetDescription()
        {
            string descr = $"Consume all {tag} matched and {nestedAction.GetDescription()} for each";
            if (amount != 1) descr += $" {amount}";

            return descr;
        }

        public ConsumeClearedAnd(int amount, string tag, TileActionBase nestedAction)
        {
            this.amount = amount;
            this.tag = tag;
            this.nestedAction = nestedAction;
        }

        public override void Init(MyTile tile)
        {
            Assert.IsTrue(amount > 0);
            base.Init(tile);
            nestedAction.Init(tile);
        }

        public override IEnumerator Run(LineClearData match)
        {
            var captured = match.tiles
                .Where(x => x.HasTag(tag))
                .ToList();

            foreach (var tile in captured)
            {
                match.tiles.Remove(tile);

                yield return tile.FadeOut(8f);
                MakeBullet(tile)//, DataManager.current.vfxDict.Get("poof"))
                    .SetSpleen(default)
                    .SetSpeed(15)
                    .SetSprite(tile.data.sprite)
                    .SetTarget(parent);
                yield return new WaitForSeconds(.05f);
            }
            yield return new WaitForSeconds(.1f);

            yield return nestedAction.Run(captured.Count);
            yield return parent.FadeOut(10f);
        }

        public class Builder : FactoryBuilder<TileActionBase, int, string, FactoryBuilder<TileActionBase>>
        {
            public override TileActionBase Build() => new ConsumeClearedAnd(value, value2, value3.Build());
        }
    }

    public class ForCleared : ClearActionBase
    {
        int amount;
        string tag;
        TileActionBase nestedAction;

        public override string GetDescription()
           => $"{nestedAction.GetDescription()} for each {amount} {tag} cleared";

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
            yield return nestedAction.Run(totalCount);
            /*
            for (int i = 0; i < totalCount; i++)
            {
                yield return nestedAction.Run();
                yield return new WaitForSeconds(.1f);
            }*/

            yield return parent.FadeOut(10f);
        }

        public class Builder : FactoryBuilder<TileActionBase, int, string, FactoryBuilder<TileActionBase>>
        {
            public override TileActionBase Build() => new ForCleared(value, value2, value3.Build());
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

        public override string GetDescription()
        {
            if (count == 1)
            {
                return $"Spawn '{GetData().title}'";
            }
            else
            {
                return $"Spawn {count} '{GetData().title}'";
            }
        }

        void Spawn(MyTile tile)
        {
            tile.SetBoard(parent.board);
            tile.Init(GetData());
            tile.isActionLocked = true;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var data = GetData();
            if (data == null)
            {
                Debug.LogError($"Data with id `{tileId}` not found!");
                yield break;
            }

            for (int i = 0; i < count * multiplier; i++)
            {
                var target = parent.board.TakeEmptyTile();
                //if (!target) Debug.Log("no empty tiles");
                if (!target) yield break;

                MakeBullet(parent)
                    .SetTarget(target)
                    .SetSprite(data.sprite)
                    .SetTileAction(Spawn);

                yield return new WaitForSeconds(.05f);
            }
            yield return new WaitForSeconds(.1f);
        }

        public class Builder : FactoryBuilder<TileActionBase, int, string>
        {
            public override TileActionBase Build() => new SpawnTile(value, value2);
        }
    }

}