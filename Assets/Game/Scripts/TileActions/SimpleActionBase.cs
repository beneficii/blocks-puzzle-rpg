using FancyToolkit;
using GridBoard;
using System.Collections;
using System.Linq;
using UnityEngine;

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

    
    public class BuffPowerAround : TileActionBase
    {
        int value;
        TileStatType type;
        public override string GetDescription()
            => $"Add {value} {type} to surrounding tiles";

        public BuffPowerAround(int value, TileStatType type)
        {
            this.value = value;
            this.type = type;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            foreach (var item in parent.board.GetTilesAround(parent.position.x, parent.position.y))
            {
                if (item is not MyTile tile 
                    || tile.myData == null
                    || tile.isBeingPlaced
                    || tile.myData.powerType != type) continue;

                tile.Power += value * multiplier;
                yield return new WaitForSeconds(.1f);

            }

            yield return new WaitForSeconds(.2f);
        }

        public class Builder : FactoryBuilder<TileActionBase, int, TileStatType>
        {
            public override TileActionBase Build() => new BuffPowerAround(value1, value2);
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
                item.Clean();
                item.Init(data);
                item.InitBoard(item.board);

                yield return new WaitForSeconds(.1f);
            }
            yield return parent.FadeOut(10f);
        }

        public class Builder : FactoryBuilder<TileActionBase, string, string>
        {
            public override TileActionBase Build() => new TransformAround(value1, value2);
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
            yield return new WaitForSeconds(.1f);
        }

        public class Builder : FactoryBuilder<TileActionBase, int, FactoryBuilder<TileActionBase>>
        {
            public override TileActionBase Build() => new ForOnBoard(value1, value2.Build());
        }
    }
}