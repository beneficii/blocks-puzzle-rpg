using System.Collections;
using UnityEngine;
using FancyToolkit;
using GridBoard;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;


public abstract class DialogAction
{
    public virtual string GetDescription() => null;
    public abstract void Execute();
    public virtual string CheckErrors() => null;
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

    public class Punch : DialogAction
    {
        public Punch(StringScanner scanner)
        {
        }

        public override void Execute()
        {
            var enemy = CombatArena.current.enemy;
            if (!enemy) return;

            enemy.SetCombatVisible(true);
            enemy.gameObject.GetComponentInChildren<NumberFloatingAnimator>(true)
                .SpawnCustom("-0", false);
            enemy.FakeDamage();
        }
    }

    public class Health : DialogAction
    {
        public int amount;
        public override string GetDescription() => $"{amount.SignedStr()} hp";

        public override string CheckErrors()
        {
            var player = CombatArena.current.player;
            if (!player || player.health.Value + amount <= 0)
            {
                return "Not enough health";
            }

            return null;
        }

        public Health(StringScanner scanner)
        {
            amount = scanner.NextInt();
        }

        public override void Execute()
        {
            var player = CombatArena.current.player;
            if (!player)
            {
                Debug.LogError("No player found for healing");
                return;
            }

            if (amount > 0)
            {
                player.AddHp(amount);
            }
            else if (amount < 0)
            {
                player.RemoveHp(-amount);
            }
        }
    }

    public class Gold : DialogAction
    {
        public int amount;
        public override string GetDescription() => $"{amount.SignedStr()} gold";

        public override string CheckErrors()
        {
            if (amount < 0 && !ResourceCtrl.current.Enough(ResourceType.Gold, -amount))
            {
                return "Not enough gold";
            }

            return null;
        }

        public Gold(StringScanner scanner)
        {
            amount = scanner.NextInt();
        }

        public override void Execute()
        {
            if (amount > 0)
            {
                ResCtrl<ResourceType>.current.Add(ResourceType.Gold, amount);
            }
            else if (amount < 0)
            {
                ResCtrl<ResourceType>.current.Remove(ResourceType.Gold, amount);
            }
        }
    }

    public class SwitchType : DialogAction
    {
        StageType type;
        public override string GetDescription()
        {
            return type switch
            {
                StageType.Enemy or StageType.Elite or StageType.Boss => "Fight",
                StageType.Shop => "Shop",
                _ => null,
            };
        }

        public SwitchType(StringScanner scanner)
        {
            type = scanner.NextEnum<StageType>();
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

    public class GetGlyph : DialogAction
    {
        public string id;

        public override string GetDescription() => $"Get {GlyphCtrl.current.Get(id).name} Glyph";
        public override IHasInfo GetInfo() => GlyphCtrl.current.Get(id);

        public GetGlyph(StringScanner scanner)
        {
            id = scanner.NextString();
        }

        public override void Execute()
        {
            Game.current.AddGlyph(id);
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
            UIHudDialog.current.SetNext(StageType.Victory);
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