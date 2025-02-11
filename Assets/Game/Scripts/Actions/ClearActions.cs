using FancyToolkit;
using GridBoard;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameActions
{
    public abstract class ClearAction : ActionBaseWithNested
    {
        public sealed override IEnumerator Run(int multiplier = 1)
        {
            var match = LineClearData.current;
            if (match == null)
            {
                Debug.LogError("No clear data present");
                yield break;
            }

            yield return Run(match, multiplier);
        }

        protected abstract IEnumerator Run(LineClearData match, int multiplier = 1);
    }

    public class ConsumeClearedAnd : ClearAction
    {
        int amount;
        string tag;

        public override string GetDescription()
        {
            string descr = $"Erase all {tag} matched and {nestedAction.GetDescription()} for each";
            if (amount != 1) descr += $" {amount}";

            return descr;
        }

        public override IEnumerable<IHintProvider> GetHints()
        {
            foreach (var item in base.GetHints())
            {
                yield return item;
            }

            yield return new ActionHint.Erase();
        }

        public ConsumeClearedAnd(int amount, string tag, ActionBase nestedAction)
        {
            this.amount = amount;
            this.tag = tag;
            this.nestedAction = nestedAction;
        }

        protected override IEnumerator Run(LineClearData match, int multiplier = 1)
        {
            var captured = match.tiles
                .Where(x => x.HasTag(tag))
                .ToList();

            foreach (var item in captured)
            {
                var tile = item as MyTile;
                match.tiles.Remove(tile);

                MakeBullet(tile)
                    .SetSpleen(default)
                    .SetSpeed(15)
                    .SetSprite(tile.data.sprite)
                    .SetAudio(AudioCtrl.current?.clipPop)
                    .SetTarget(parent.transform);
                yield return tile.FadeOut(8f);
                yield return new WaitForSeconds(.05f);
            }
            yield return new WaitForSeconds(.1f);

            yield return nestedAction.Run(captured.Count * multiplier);
        }

        public class Builder : FactoryBuilder<ActionBase, int, string, FactoryBuilder<ActionBase>>
        {
            public override ActionBase Build() => new ConsumeClearedAnd(value, value2, value3.Build());
        }
    }

    public class ForCleared : ClearAction
    {
        int amount;
        string tag;

        public override IEnumerable<IHintProvider> GetHints()
        {
            foreach (var item in base.GetHints())
            {
                yield return item;
            }

            yield return new ActionHint.Cleared();
        }

        public override string GetDescription()
           => $"{nestedAction.GetDescription()} for each {amount} {tag} cleared";

        public ForCleared(int amount, string tag, ActionBase nestedAction)
        {
            this.amount = amount;
            this.tag = tag;
            this.nestedAction = nestedAction;
        }

        protected override IEnumerator Run(LineClearData match, int multiplier = 1)
        {
            int totalCount = match.list.Count(x => x.HasTag(tag)) / amount;

            if (totalCount == 0) yield break;
            yield return nestedAction.Run(totalCount * multiplier);
        }

        public class Builder : FactoryBuilder<ActionBase, int, string, FactoryBuilder<ActionBase>>
        {
            public override ActionBase Build() => new ForCleared(value, value2, value3.Build());
        }
    }

    public class CopyHighestDamage : ClearAction
    {
        public override string GetDescription()
            => $"Copy highest damage in a clear";

        protected override IEnumerator Run(LineClearData match, int multiplier = 0)
        {
            SetBulletDamage(MakeBullet(parent)
                .SetTarget(CombatArena.current.enemy)
                .SetLaunchDelay(0.2f)
                , match.GetValueMax(MyTile.keyDamage));
            yield return new WaitForSeconds(.1f);
        }

        public class Builder : FactoryBuilder<ActionBase>
        {
            public override ActionBase Build() => new CopyHighestDamage();
        }
    }
}