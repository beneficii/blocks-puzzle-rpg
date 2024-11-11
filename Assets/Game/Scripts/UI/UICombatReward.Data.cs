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
            var stageData = StageCtrl.current.Data;

            List<MyTileData> list = TileCtrl.current.GetAllTiles()
                    .Cast<MyTileData>()
                    .Where(x => x.rarity == rarity)
                    .ToList();
            UIHudSelectTile.current.ShowChoise(list.RandN(3, Game.current.CreateStageRng()));
        }

        public override void InitUI(UICombatReward ui)
        {
            ui.imgIcon.sprite = ui.iconTile;
            ui.txtCaption.text = "Pick a tile";
        }
    }
}
