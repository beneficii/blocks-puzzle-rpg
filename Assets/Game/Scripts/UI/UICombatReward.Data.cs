using FancyToolkit;
using GridBoard;
using System.Collections.Generic;
using System.Linq;

public partial class UICombatReward
{
    public abstract class Data
    {
        public abstract void InitUI(UICombatReward ui);
        public abstract void Click();
    }

    public class DataGold : Data
    {
        public int amount;

        public DataGold(int amount)
        {
            this.amount = amount;
        }

        public override void Click()
        {
            ResCtrl<ResourceType>.current.Add(ResourceType.Gold, amount);
        }

        public override void InitUI(UICombatReward ui)
        {
            ui.imgIcon.sprite = ui.iconGold;
            ui.txtCaption.text = $"{amount} gold";
        }
    }

    public class DataTile : Data
    {
        public Rarity rarity;

        public DataTile(Rarity rarity)
        {
            this.rarity = rarity;
        }

        public override void Click()
        {
            CombatCtrl.current.ShowTileChoise(rarity);
        }

        public override void InitUI(UICombatReward ui)
        {
            ui.imgIcon.sprite = ui.iconTile;
            ui.txtCaption.text = "Pick a tile";
        }
    }

    public class DataSkill : Data
    {
        public Rarity rarity;

        public DataSkill(Rarity rarity)
        {
            this.rarity = rarity;
        }

        public override void Click()
        {
            CombatCtrl.current.ShowSkillChoise(rarity);
        }

        public override void InitUI(UICombatReward ui)
        {
            ui.imgIcon.sprite = ui.iconSkill;
            ui.txtCaption.text = "Pick a skill";
        }
    }
}
