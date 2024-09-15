using System.Collections;
using UnityEngine;
using FancyToolkit;


public abstract class DialogAction
{
    public abstract void Execute();
    public virtual string GetHint() => null;
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

        public AddGold(StringScanner scanner)
        {
            amount = scanner.NextInt();
        }

        public override void Execute()
        {
            ResCtrl<ResourceType>.current.Add(ResourceType.Gold, amount);
        }
    }
}