using System.Collections;
using UnityEngine;

namespace AnalyticsEvents
{
    public class DialogSelected : Unity.Services.Analytics.Event
    {
        public DialogSelected() : base("DialogSelected") { }

        public string answer { set { SetParameter("answer", value); } }
        public int userLevel { set { SetParameter("userLevel", value); } }
        public int leveId { set { SetParameter("leveId", value); } }
        public int seed { set { SetParameter("seed", value); } }
    }

    public class TileSelected : Unity.Services.Analytics.Event
    {
        public TileSelected() : base("TileSelected") { }

        public string TileId { set { SetParameter("TileId", value); } }
        public int userLevel { set { SetParameter("userLevel", value); } }
        public int leveId { set { SetParameter("leveId", value); } }
        public int seed { set { SetParameter("seed", value); } }
    }

    public class SkillSelected : Unity.Services.Analytics.Event
    {
        public SkillSelected() : base("skillSelected") { }

        public string skillId { set { SetParameter("skillId", value); } }
        public int userLevel { set { SetParameter("userLevel", value); } }
        public int leveId { set { SetParameter("leveId", value); } }
        public int seed { set { SetParameter("seed", value); } }
    }

    public class LevelCompletion : Unity.Services.Analytics.Event
    {
        public LevelCompletion() : base("LevelCompletion") { }

        public int userLevel { set { SetParameter("userLevel", value); } }
        public int health { set { SetParameter("health", value); } }
        public int leveId { set { SetParameter("leveId", value); } }
        public int seed { set { SetParameter("seed", value); } }
    }

}