using FancyToolkit;
using GridBoard;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class UICombatReward
{
    public abstract class Data : IHasNestedInfo
    {
        public abstract void InitUI(UICombatReward ui);
        public abstract void Click(UICombatReward ui);

        public virtual IHasInfo GetInfo() => null;
        public virtual IHoverInfoTarget GetHoverInfoTarget() => null;
    }

    public class DataGold : Data
    {
        public int amount;

        public DataGold(int amount)
        {
            this.amount = amount;
        }

        public override void Click(UICombatReward ui)
        {
            ResCtrl<ResourceType>.current.Add(ResourceType.Gold, amount);
        }

        public override void InitUI(UICombatReward ui)
        {
            ui.imgIcon.sprite = ui.iconGold;
            ui.txtCaption.text = $"{amount} gold";
        }
    }

    public class DataTilesPerTurn : Data
    {
        public DataTilesPerTurn()
        {
        }

        public override void Click(UICombatReward ui)
        {
            Game.current.AddTilesPerTurn();
        }

        public override void InitUI(UICombatReward ui)
        {
            ui.imgIcon.sprite = ui.iconGold;
            ui.txtCaption.text = $"+1 tile draw";
        }
    }

    public class DataTile : Data
    {
        public Rarity rarity;

        public DataTile(Rarity rarity)
        {
            this.rarity = rarity;
        }

        public override void Click(UICombatReward ui)
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

        public override void Click(UICombatReward ui)
        {
            CombatCtrl.current.ShowSkillChoise(rarity);
        }

        public override void InitUI(UICombatReward ui)
        {
            ui.imgIcon.sprite = ui.iconSkill;
            ui.txtCaption.text = "Pick a skill";
        }
    }


    public class DataGlyph : Data
    {
        GlyphData data;

        public override IHasInfo GetInfo() => data;
        public override IHoverInfoTarget GetHoverInfoTarget() => data;

        public DataGlyph(string id)
        {
            this.data = GlyphCtrl.current.Get(id);
        }

        public DataGlyph(GlyphData data)
        {
            this.data = data;
        }

        public override void Click(UICombatReward ui)
        {
            Game.current.AddGlyph(data.id);
            var player = CombatArena.current.player;
            if (player)
            {
                ui.CreateBullet("Spell", player.transform);
            }
        }

        public override void InitUI(UICombatReward ui)
        {
            ui.imgIcon.SetSpriteAndSize(data.sprite, 2);
            ui.txtCaption.text = $"{data.name} Glyph";
        }
    }
}
