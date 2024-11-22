using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using FancyToolkit;
using GridBoard;
using System.Linq;

namespace GameActions
{
    public class TransformInto : TileActionBase
    {
        TileTargetingType targetType;
        string tag;

        public override string GetDescription()
        {
            return $"Transform into a copy of {MyTile.GetTargetingTypeName(targetType, tag)}";
        }

        public TransformInto(TileTargetingType targetType, string tag = TileData.anyTag)
        {
            this.targetType = targetType;
            this.tag = tag;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var target = MyTile.FindTileTargets(parent, targetType, x => x.data.type != Tile.Type.Empty && x.HasTag(tag))
                .FirstOrDefault();

            if (target == null) yield break;

            MakeBullet(target)
                    .SetTarget(tileParent)
                    .SetSpleen(default)
                    .SetTileAction(x => {
                        if (!target) return;
                        x.Init(target.data);
                        x.Power = target.Power;
                    });

            yield return new WaitForSeconds(.15f);
        }

        public class Builder : FactoryBuilder<ActionBase, TileTargetingType>
        {
            string tag;
            public override void Init(StringScanner scanner)
            {
                base.Init(scanner);
                scanner.TryGet(out tag, TileData.anyTag);
            }

            public override ActionBase Build() => new TransformInto(value, tag);
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
            var targets = MyTile.FindTileTargets(parent, TileTargetingType.Around, tag).ToList();
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
        public class Builder : FactoryBuilder<ActionBase, string, string, int>
        {
            public override ActionBase Build() => new TransformAround(value, value2, value3);
        }
    }

    public class TransformIn : ActionBase
    {
        TileTargetingType targetType;
        string tag;
        string id;

        TileData GetData() => TileCtrl.current.Get(id);

        public override string GetDescription()
            => $"Transform {MyTile.GetTargetingTypeName(targetType, tag)} into '{GetData().title}'";

        public TransformIn(TileTargetingType targetType, string tag, string id)
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
                MakeBullet(parent)
                        .SetTarget(item)
                        .SetTileAction(x =>
                        {
                            x.Init(data);
                            x.isActionLocked = true;
                        });

                yield return new WaitForSeconds(.1f);
            }
        }

        public class Builder : FactoryBuilder<ActionBase, TileTargetingType, string, string>
        {
            public override ActionBase Build() => new TransformIn(value, value2, value3);
        }
    }
}