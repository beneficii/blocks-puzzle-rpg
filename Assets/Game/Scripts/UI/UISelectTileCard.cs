using GridBoard;
using UnityEngine;
using UnityEngine.UI;
using FancyToolkit;
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;

public class UISelectTileCard : MonoBehaviour, IHintContainer, IHoverInfoTarget
{
    public static event System.Action<UISelectTileCard, SelectTileType> OnSelectCard;

    [SerializeField] UIInfoPanel infoPanel;
    [SerializeField] CanvasGroup cg;
    [SerializeField] Button btnBuy;
    [SerializeField] Button btnSelect;
    [SerializeField] Image imgBg;
    [SerializeField] TextMeshProUGUI txtCost;
    [SerializeField] Image imgFrame;
    [SerializeField] Image imgIcon;


    [SerializeField] Sprite spriteBgSpecial;
    [SerializeField] AudioClip soundSelect;
    [SerializeField] AudioClip soundBuy;

    SelectTileType type;
    int price;
    public IInfoTextProvider data;

    public void Init(SelectTileType type, IInfoTextProvider data, int price = 0)
    {
        this.type = type;
        this.data = data;
        this.price = price;

        infoPanel.InitText(data);
        infoPanel.InitIcon(data as IIconProvider);
        
        if (type == SelectTileType.Shop)
        {
            txtCost.text = $"{price}";
        }
        if (btnBuy) btnBuy.gameObject.SetActive(type == SelectTileType.Shop);
        if (btnSelect) btnSelect.gameObject.SetActive(type == SelectTileType.Choise);

        if (data is MyTileData tileData)
        {
            if (tileData.buyAction != null) imgBg.sprite = spriteBgSpecial;
        }
        else if (data is SkillData skillData)
        {
            imgIcon.transform.localScale = Vector3.one * 2;
            imgFrame.gameObject.SetActive(false);
        }
    }

    public GenericBullet CreateBullet(string vfxId = null)
    {
        return Game.current.MakeBullet(imgIcon.transform.position, vfxId)
            .SetSprite(imgIcon.sprite);
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

        CreateBullet(vfxId)
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
            CreateBullet("Spell")
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

    public List<IHintProvider> GetHintProviders()
    {
        if (data is IHintContainer hintContainer)
        {
            return hintContainer.GetHintProviders();
        }

        return new();
    }

    public bool ShouldShowHoverInfo() => true;
}