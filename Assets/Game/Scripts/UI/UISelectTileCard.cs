using GridBoard;
using UnityEngine;
using UnityEngine.UI;

public class UISelectTileCard : MonoBehaviour
{
    public static event System.Action<UISelectTileCard> OnSelectTile;
    public static event System.Action<UISelectTileCard> OnBuyTile;

    [SerializeField] UIHoverInfo infoPanel;
    [SerializeField] CanvasGroup cg;
    [SerializeField] Button btnBuy;
    [SerializeField] Button btnSelect;

    SelectTileType type;
    public TileData data { get; private set; }
    

    public void Init(SelectTileType type, TileData data)
    {
        this.type = type;
        this.data = data;
        infoPanel.Init(data);
        if (type != SelectTileType.Shop) infoPanel.HideCost();

        btnBuy.gameObject.SetActive(type == SelectTileType.Shop);
        btnSelect.gameObject.SetActive(type == SelectTileType.Choise);
    }

    public void Select()
    {
        OnSelectTile?.Invoke(this);
    }

    public void Buy()
    {
        OnBuyTile?.Invoke(this);
        Hide();
    }

    void Hide()
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
}
