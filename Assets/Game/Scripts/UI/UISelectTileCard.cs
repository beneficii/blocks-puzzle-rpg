using GridBoard;
using UnityEngine;
using UnityEngine.UI;
using FancyToolkit;
using UnityEngine.LowLevel;

public class UISelectTileCard : MonoBehaviour
{
    public static event System.Action<UISelectTileCard> OnSelectTile;
    public static event System.Action<UISelectTileCard> OnBuyTile;

    [SerializeField] UIHoverInfo infoPanel;
    [SerializeField] CanvasGroup cg;
    [SerializeField] Button btnBuy;
    [SerializeField] Button btnSelect;
    [SerializeField] Image imgBg;

    [SerializeField] Sprite spriteBgSpecial;

    [SerializeField] MyTile dummyTile;

    SelectTileType type;
    int price;
    public MyTileData data { get; private set; }

    public void Init(SelectTileType type, MyTileData data, int price = 0)
    {
        this.type = type;
        this.data = data;
        this.price = price;
        dummyTile.Init(data);
        infoPanel.Init(dummyTile);
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
        if (data.buyAction != null) imgBg.sprite = spriteBgSpecial;
    }

    public void Select()
    {
        OnSelectTile?.Invoke(this);
    }

    public void Buy()
    {
        if (!ResCtrl<ResourceType>.current.Remove(ResourceType.Gold, price))
        {
            MainUI.current.ShowMessage("Not enough gold");
            return;
        }

        if (data.buyAction != null)
        {
            var action = data.buyAction.Build();
            action.Init(dummyTile);
            Game.current.StartCoroutine(action.Run());
        }
        else
        {
            OnBuyTile?.Invoke(this);
        }

        Hide();
    }

    void Hide()
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
}
