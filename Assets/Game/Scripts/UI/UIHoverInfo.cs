using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FancyToolkit;
using GridBoard;


public class UIHoverInfo : MonoBehaviour
{
    [SerializeField] Image imgIcon;
    [SerializeField] TextMeshProUGUI txtTitle;
    [SerializeField] TextMeshProUGUI txtDescription;
    [SerializeField] TextMeshProUGUI txtTags;
    [SerializeField] TextMeshProUGUI txtCost;
    [SerializeField] List<TooltipPanel> hints = new();

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    void Show(IHasInfo info)
    {
        if (!info.ShouldShowInfo())
        {
            Hide();
            return;
        }

        Init(info);
    }

    public void HideCost() { txtCost.text = ""; }

    public void Init(IHasInfo info)
    {
        if (imgIcon) imgIcon.sprite = info.GetIcon();
        if (txtTitle) txtTitle.text = info.GetTitle();
        if (txtTags) txtTags.text = string.Join(", ", info.GetTags());
        if (txtDescription) txtDescription.text = info.GetDescription();

        foreach (var item in hints) item.Hide();

        var hintData = info.GetTooltips()
            .Take(hints.Count)
            .ToList();
        for (var i = 0; i < hintData.Count; i++)
        {
            hints[i].Show(hintData[i]);
        }
    }

    public void SetCost(int price)
    {
        if (txtCost) txtCost.text = $"Cost: {price}";
    }

    void Show(Tile info)
    {
        if (info.data.isEmpty)
        {
            Hide();
            return;
        }
        Init(info.data);
        if (txtDescription) txtDescription.text = info.GetDescription();
    }

    void Show(Unit unit)
    {
        var data = unit.data;

        txtTitle.text = data.name;
        txtDescription.text = unit.GetDescription();

        foreach (var item in hints) item.Hide();
        var tip = unit.GetTooltip();

        if (!string.IsNullOrEmpty(tip) && hints.Count > 0) hints[0].Show(tip);
    }

    public void Show(Transform collider)
    {
        if (!collider)
        {
            Hide();
            return;
        }

        if (collider.TryGetComponent<Unit>(out var unit))
        {
            Show(unit);
            return;
        }

        if (collider.TryGetComponent<IHasInfo>(out var info))
        {
            Show(info);
            return;
        }

        if (collider.TryGetComponent<IHasNestedInfo>(out var infoContainer) && infoContainer.GetInfo() != null)
        {
            Show(infoContainer.GetInfo());
            return;
        }

        Hide();
    }

    [System.Serializable]
    public class TooltipPanel
    {
        public GameObject parent;
        public TextMeshProUGUI txtDescription;

        public void Show(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                Hide();
                return;
            }
            parent.SetActive(true);
            txtDescription.text = text;
        }

        public void Hide()
        {
            parent.SetActive(false);
        }
    }
}