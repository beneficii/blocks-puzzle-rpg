using System.Collections;
using UnityEngine;
using FancyToolkit;
using GridBoard;
using System.Collections.Generic;


public abstract class DialogAction
{
    public virtual string GetDescription() => null;
    public abstract void Execute();
    public virtual IHasInfo GetInfo() => null;
}

namespace DialogActions
{
    public class Dialog : DialogAction
    {
        public string nextID;

        public Dialog(StringScanner scanner)
        {
            nextID = scanner.NextString();
        }

        public override void Execute()
        {
            UIHudDialog.current.SetNext(nextID);
        }
    }

    public class AddGold : DialogAction
    {
        public int amount;
        public override string GetDescription() => $"+{amount} gold";


        public AddGold(StringScanner scanner)
        {
            amount = scanner.NextInt();
        }

        public override void Execute()
        {
            ResCtrl<ResourceType>.current.Add(ResourceType.Gold, amount);
        }
    }

    public class SwitchType : DialogAction
    {
        StageData.Type type;
        public override string GetDescription()
        {
            return type switch
            {
                StageData.Type.Enemy or StageData.Type.Elite or StageData.Type.Boss => "Fight",
                StageData.Type.Shop => "Shop",
                _ => null,
            };
        }

        public SwitchType(StringScanner scanner)
        {
            type = scanner.NextEnum<StageData.Type>();
        }

        public override void Execute()
        {
            UIHudDialog.current.SetNext(type);
        }
    }

    public class GetSkill : DialogAction
    {
        public string id;

        public override string GetDescription() => $"Learn '{SkillCtrl.current.Get(id).name}' skill";
        public override IHasInfo GetInfo() => SkillCtrl.current.Get(id);

        public GetSkill(StringScanner scanner)
        {
            id = scanner.NextString();
        }

        public override void Execute()
        {
            Game.current.AddSkill(id);
        }
    }

    public class GetTile : DialogAction
    {
        public string id;

        public override string GetDescription() => $"Get '{TileCtrl.current.Get(id).title}' tile";

        public GetTile(StringScanner scanner)
        {
            id = scanner.NextString();
        }

        public override void Execute()
        {
            Game.current.AddTileToDeck(id);
        }
    }

    public class Victory : DialogAction
    {
        public Victory(StringScanner scanner)
        {
        }

        public override void Execute()
        {
            UIHudDialog.current.SetNext(StageData.Type.Victory);
        }
    }

    public class SelectSkill : DialogAction
    {
        public Rarity rarity;

        public override string GetDescription() => $"Choose {rarity} skill";

        public SelectSkill(StringScanner scanner)
        {
            rarity = scanner.NextEnum<Rarity>();
        }

        public override void Execute()
        {
            CombatCtrl.current.AddState(new CombatStates.SelectSkill(rarity));
        }
    }

    public class DemoSelectTiles : DialogAction
    {
        int amountCommon;
        int amountUncommon;
        int amountRare;

        public DemoSelectTiles(StringScanner scanner)
        {
            this.amountCommon = scanner.NextInt();
            scanner.TryGetGeneric(out amountUncommon);
            scanner.TryGetGeneric(out amountRare);
        }

        public override string GetDescription() => $"Choose multiple tiles";


        public override void Execute()
        {
            for (int i = 0; i < amountCommon; i++)
            {
                CombatCtrl.current.AddState(new CombatStates.SelectTile(Rarity.Common));
            }

            for (int i = 0; i < amountUncommon; i++)
            {
                CombatCtrl.current.AddState(new CombatStates.SelectTile(Rarity.Uncommon));
            }

            for (int i = 0; i < amountRare; i++)
            {
                CombatCtrl.current.AddState(new CombatStates.SelectTile(Rarity.Rare));
            }
        }
    }

    public class DemoRewards : DialogAction
    {
        int amountCommon;
        int amountUncommon;
        int amountRare;
        int amountSkills;

        public DemoRewards(StringScanner scanner)
        {
            this.amountCommon = scanner.NextInt();
            scanner.TryGetGeneric(out amountUncommon);
            scanner.TryGetGeneric(out amountRare);
            scanner.TryGetGeneric(out amountSkills);
        }

        public override string GetDescription() => $"Get many rewards";


        public override void Execute()
        {
            var rewards = new List<UICombatReward.Data>();

            for (int i = 0; i < amountCommon; i++)
            {
                rewards.Add(new UICombatReward.DataTile(Rarity.Common));
            }

            for (int i = 0; i < amountUncommon; i++)
            {
                rewards.Add(new UICombatReward.DataTile(Rarity.Uncommon));
            }

            for (int i = 0; i < amountRare; i++)
            {
                rewards.Add(new UICombatReward.DataTile(Rarity.Rare));
            }

            for (int i = 0; i < amountSkills; i++)
            {
                rewards.Add(new UICombatReward.DataSkill(Rarity.Common));
            }

            CombatCtrl.current.AddState(new CombatStates.RewardScreen(rewards));
        }
    }

}