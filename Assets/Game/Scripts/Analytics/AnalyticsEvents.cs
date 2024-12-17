using System.Collections;
using UnityEngine;

namespace AnalyticsEvents
{

    public abstract class Base : Unity.Services.Analytics.Event
    {
        public Base(string name) : base(name) { }

        public int userLevel { set { SetParameter("userLevel", value); } }
        public int leveId { set { SetParameter("leveId", value); } }
        public int seed { set { SetParameter("seed", value); } }
    }

    public class DialogSelected : Base
    {
        public DialogSelected() : base("DialogSelected") { }

        public string answer { set { SetParameter("answer", value); } }
        
    }

    public class TileSelected : Base
    {
        public TileSelected() : base("TileSelected") { }

        public string TileId { set { SetParameter("TileId", value); } }
    }

    public class SkillSelected : Base
    {
        public SkillSelected() : base("skillSelected") { }

        public string skillId { set { SetParameter("skillId", value); } }
    }

    public class LevelCompletion : Base
    {
        public LevelCompletion() : base("LevelCompletion") { }

        public int health { set { SetParameter("health", value); } }
    }

}