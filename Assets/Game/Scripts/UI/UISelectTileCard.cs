using GridBoard;
using UnityEngine;
using UnityEngine.UI;
using FancyToolkit;
using DG.Tweening;
using System;

public class UISelectTileCard : MonoBehaviour, IHasNestedInfo
{
    public static event System.Action<UISelectTileCard, SelectTileType> OnSelectCard;

    [SerializeField] UIHoverInfo infoPanel;
    [SerializeField] CanvasGroup cg;
    [SerializeField] Button btnBuy;
    [SerializeField] Button btnSelect;
    [SerializeField] Image imgBg;

    [SerializeField] Sprite spriteBgSpecial;
    [SerializeField] AudioClip soundSelect;
    [SerializeField] AudioClip soundBuy;

    SelectTileType type;
    int price;
    public IHasInfo data { get; private set; }

    public void Init(SelectTileType type, IHasInfo data, int price = 0)
    {
        this.type = type;
        this.data = data;
        this.price = price;
        infoPanel.Init(data);
        if (type == SelectTileType.Shop)
        {
            infoPanel.SetCost(price);
        }
        else
        {
            infoPanel.HideCost();
        }

        btnBuy.gameObject.SetActive(type == SelectTileType.Shop);
        btnSelect.gameObject.SetActive(type == SelectTileType.Choise);

        if (data is MyTileData tileData)
        {
            if (tileData.buyAction != null) imgBg.sprite = spriteBgSpecial;
        }
        else if (data is SkillData skillData)
        {
            infoPanel.imgIcon.transform.localScale = Vector3.one * 2;
            infoPanel.imgFrame.gameObject.SetActive(false);
        }
    }

    void SelectedTile(MyTileData tileData)
    {
        if (tileData.buyAction != null)
        {
            var action = tileData.buyAction.Build();
            action.Init(tileData);
            Game.current.StartCoroutine(action.Run());
            return;
        }

        CombatCtrl.current.AddTileToSet(tileData);

        var vfxId = tileData?.type.ToString();

        infoPanel.CreateBullet(vfxId)
                .SetSpleen(UnityEngine.Random.Range(0, 1) == 1 ? Vector2.left : Vector2.right)
                .SetTarget(MainUI.current.uiBtnTiles)
                .SetAction(x =>
                {
                    x.transform.localScale = Vector3.one * 0.9f;

                    x.transform.DOScale(Vector3.one, .25f)
                        .SetEase(Ease.InOutBack);
                });
    }

    void SelectedSkill(SkillData skillData)
    {
        Game.current.AddSkill(skillData.id);
        var player = CombatArena.current.player;
        if (player)
        {
            infoPanel.CreateBullet("Spell")
                .SetSpleen(UnityEngine.Random.Range(0, 1) == 1 ? Vector2.left : Vector2.right)
                .SetTarget(player)
                .SetAction(x =>
                {
                    // ToDo: maybe learning animation
                });
        }
    }

    public void Select()
    {
        (type == SelectTileType.Shop ? soundBuy : soundSelect)?.PlayNow();

        if (data is MyTileData tileData)
        {
            SelectedTile(tileData);
        }
        else if (data is SkillData skillData)
        {
            SelectedSkill(skillData);
        }

        OnSelectCard?.Invoke(this, type);

        if (type == SelectTileType.Choise)
        {
            UIHudSelectTile.current.Close();
        }
    }

    public void Buy()
    {
        if (!ResCtrl<ResourceType>.current.Remove(ResourceType.Gold, price))
        {
            MainUI.current.ShowMessage("Not enough gold");
            return;
        }

        Select();

        Hide();
    }

    void Hide()
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public IHasInfo GetInfo() => data?.GetExtraInfo();
}