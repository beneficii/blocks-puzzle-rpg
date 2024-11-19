using FancyToolkit;
using GridBoard;
using System.Collections;
using UnityEngine;
using TileActions;

namespace SkillActions
{
    public class SpawnTile : SkillActionBase
    {
        int count;
        string tileId;

        TileData GetData() => TileCtrl.current.Get(tileId);

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
                if (!target) yield break;

                MakeBullet()
                    .SetTarget(target)
                    .SetSprite(data.sprite)
                    .SetTileAction(Spawn);

                yield return new WaitForSeconds(.05f);
            }
        }

        public class Builder : FactoryBuilder<SkillActionBase, int, string>
        {
            public override SkillActionBase Build() => new SpawnTile(value, value2);
        }

    }

    public class ConsumeArmorAnd : SkillActionBase
    {
        int amount;
        SkillActionBase nestedAction;

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

        public ConsumeArmorAnd(int amount, SkillActionBase nestedAction)
        {
            this.amount = amount;
            this.nestedAction = nestedAction;
        }

        public override void Init(UISkillButton parent)
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

            var bullet = MakeBullet(player.transform.position)
                            .SetSprite(parent.GetIcon())
                            .SetTarget(parent);
            yield return new WaitWhile(() => bullet);

            yield return nestedAction.Run(multiplier * multi);
            yield return new WaitForSeconds(.1f);
        }

        public class Builder : FactoryBuilder<SkillActionBase, int, FactoryBuilder<SkillActionBase>>
        {
            public override SkillActionBase Build() => new ConsumeArmorAnd(value, value2.Build());
        }
    }

    public class MultiplyPowerTag : SkillActionBase
    {
        TileTargetingType targetType;
        int value;
        string tag;
        public override string GetDescription()
            => $"Multiply Power of {MyTile.GetTargetingTypeName(targetType, tag)} by {value}";

        public MultiplyPowerTag(TileTargetingType targetType, int value, string tag)
        {
            this.targetType = targetType;
            this.value = value;
            this.tag = tag;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            foreach (var tile in MyTile.FindTileTargets(targetType, tag))
            {

                MakeBullet()
                    .SetTarget(tile)
                    .SetSprite(parent.GetIcon())
                    //.SetSpleen(default)
                    .SetTileAction(x => x.Power *= value * multiplier);

                yield return new WaitForSeconds(.1f);
            }
        }

        public class Builder : FactoryBuilder<SkillActionBase, TileTargetingType, int, string>
        {
            public override SkillActionBase Build() => new MultiplyPowerTag(value, value2, value3);
        }
    }

}