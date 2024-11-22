using GridBoard;
using UnityEngine;
using UnityEngine.UI;
using FancyToolkit;

public class UISelectTileCard : MonoBehaviour
{
    public static event System.Action<UISelectTileCard, SelectTileType> OnSelectCard;

    [SerializeField] UIHoverInfo infoPanel;
    [SerializeField] CanvasGroup cg;
    [SerializeField] Button btnBuy;
    [SerializeField] Button btnSelect;
    [SerializeField] Image imgBg;

    [SerializeField] Sprite spriteBgSpecial;

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
    }

    public void Select()
    {
        if (data is MyTileData tileData && tileData.buyAction != null)
        {
            var action = tileData.buyAction.Build();
            action.Init(tileData);
            Game.current.StartCoroutine(action.Run());
            return;
        }

        OnSelectCard?.Invoke(this, type);
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
}