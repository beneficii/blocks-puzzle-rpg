using System.Collections.Generic;

namespace FancyToolkit
{
    public static class ResourceUtil
    {
        public static string Possix(ResourceType type)
        {
            return $" {type}";
            //return $" {type}";//$"<sprite name=\"{type}\">";
        }

        public static string GetString(ResourceType type, int amount)
        {
            return $"{amount}{Possix(type)}";
        }
    }

    public enum ResourceType
    {
        None,
        Iron,
        Wood,
        Food,
        Unused1,
        Mana,
        GoldIncome,
        Health,
        MaxTurns,
        GoalGold,
        BonusReward,
        StartingWater,
        DrawPileSize,
        DiscardPileSize,
        RedrawCost,
        DefaultRedrawCost,
        TotalGoldEarned,
        CardsPerTurn,
        BonusWater,
        CardsUsedThisTurn,
        DefaultCardRewards,
        Enemies,
        Gold,
        MaxGold,
        Exp,
        Level,
    }

    [System.Serializable]
    public struct ResourceInfo
    {
        public ResourceType type;
        public int value;

        public ResourceInfo(ResourceType type, int value)
        {
            this.type = type;
            this.value = value;
        }

        public override string ToString()
        {
            return ResourceUtil.GetString(type, value);
        }

        [CreateFromString]
        public static ResourceInfo FromString(string str)
        {
            if (string.IsNullOrEmpty(str)) return new ResourceInfo();
            var tokens = str.Split();

            if(tokens.Length < 2) return new ResourceInfo();

            return new ResourceInfo
            {
                value = tokens.IntAt(0),
                type = EnumUtil.Parse<ResourceType>(tokens[1])
            };
        }

        public static List<ResourceInfo> ListFromString(string str)
        {
            var tokens = str.Split();

            var list = new List<ResourceInfo>();
            for (int i = 0; i+1 < tokens.Length; i+=2)
            {
                list.Add(new ResourceInfo
                {
                    value = tokens.IntAt(i),
                    type = EnumUtil.Parse<ResourceType>(tokens[i+1])
                });
            }

            return list;
        }

        public static ResourceInfo Extract(StringScanner scanner)
        {
            return new ResourceInfo
            {
                value = scanner.NextInt(),
                type = scanner.NextEnum<ResourceType>(),
            };
        }
    }
}