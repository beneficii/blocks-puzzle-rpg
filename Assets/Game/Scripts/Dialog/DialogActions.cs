using System.Collections;
using UnityEngine;
using FancyToolkit;
using GridBoard;


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

        public override string GetDescription() => $"Get '{TileCtrl.current.GetTile(id).title}' tile";

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

}