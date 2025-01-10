using FancyToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatState
{
    public abstract void Run();
}

namespace CombatStates
{
    public class InitStageType : CombatState
    {
        StageType type;

        public InitStageType(StageType type)
        {
            this.type = type;
        }

        public override void Run()
        {
            CombatCtrl.current.Init(type);
        }
    }

    public class SelectSkill : CombatState
    {
        Rarity rarity;

        public SelectSkill(Rarity rarity)
        {
            this.rarity = rarity;
        }

        public override void Run()
        {
            CombatCtrl.current.ShowSkillChoise(rarity);
        }
    }

    public class SelectTile : CombatState
    {
        Rarity rarity;

        public SelectTile(Rarity rarity)
        {
            this.rarity = rarity;
        }

        public override void Run()
        {
            CombatCtrl.current.ShowTileChoise(rarity);
        }
    }

    public class Dialog : CombatState
    {
        string id;

        public Dialog(string id)
        {
            this.id = id;
        }

        public override void Run()
        {
            UIHudDialog.current.Show(id);
        }
    }

    public class RewardScreen : CombatState
    {
        List<UICombatReward.Data> rewards;

        public RewardScreen(List<UICombatReward.Data> rewards)
        {
            this.rewards = rewards;
        }

        public override void Run()
        {
            UIHudRewards.current.Show(rewards);
        }
    }
}