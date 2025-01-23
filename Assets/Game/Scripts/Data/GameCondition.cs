using FancyToolkit;
using GameActions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameCondition
{
    public abstract bool IsMet();

    public static implicit operator bool(GameCondition obj)
    {
        return obj is null || obj.IsMet();
    }
}


namespace GameConditions
{
    public class HealthBelow : GameCondition
    {
        int value;
        public HealthBelow(int value)
        {
            this.value = value;
        }

        public override bool IsMet()
        {
            return Game.current.GetPlayerHealth().x < value;
        }

        public class Builder : FactoryBuilder<GameCondition, int>
        {
            public override GameCondition Build() => new HealthBelow(value);
        }
    }

    public class HasGlyph : GameCondition
    {
        string value;
        public HasGlyph(string value)
        {
            this.value = value;
        }

        public override bool IsMet()
        {
            return Game.current.GetGlyphs()
                .Select(x=>x.id)
                .ToList().Contains(value);
        }

        public class Builder : FactoryBuilder<GameCondition, string>
        {
            public override GameCondition Build() => new HasGlyph(value);
        }
    }

    public class VisitedAny : GameCondition
    {
        List<string> list;
        public VisitedAny(List<string> list)
        {
            this.list = list;
        }

        public override bool IsMet()
        {
            var visited = Game.current.GetVisitedStages().ToHashSet();
            foreach (var item in list)
            {
                if (visited.Contains(item)) return true;
            }

            return false;
        }

        public class Builder : FactoryBuilder<GameCondition, List<string>>
        {
            public override GameCondition Build() => new VisitedAny(value);
        }
    }


    public class VisitedAll : GameCondition
    {
        List<string> list;
        public VisitedAll(List<string> list)
        {
            this.list = list;
        }

        public override bool IsMet()
        {
            var visited = Game.current.GetVisitedStages().ToHashSet();
            foreach (var item in list)
            {
                if (!visited.Contains(item)) return false;
            }

            return true;
        }

        public class Builder : FactoryBuilder<GameCondition, List<string>>
        {
            public override GameCondition Build() => new VisitedAll(value);
        }
    }

    public class VisitedUnit : GameCondition
    {
        string id;

        public VisitedUnit(string id)
        {
            this.id = id;
        }

        public override bool IsMet()
        {
            var visited = Game.current.GetVisitedStages()
                .Select(StageCtrl.current.Get)
                .Select(x => x.units[0])
                .ToList();

            return visited.Contains(id);
        }

        public class Builder : FactoryBuilder<GameCondition, string>
        {
            public override GameCondition Build() => new VisitedUnit(value);
        }
    }

    public class Not : GameCondition
    {
        GameCondition nested;

        public Not(FactoryBuilder<GameCondition> builder)
        {
            nested = builder.Build();
        }

        public override bool IsMet()
        {
            return !nested.IsMet();
        }

        public class Builder : FactoryBuilder<GameCondition, FactoryBuilder<GameCondition>>
        {
            public override GameCondition Build() => new Not(value);
        }
    }
}