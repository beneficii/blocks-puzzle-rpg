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
    [SerializeField] TextMeshProUGUI txtTitle;
    [SerializeField] TextMeshProUGUI txtDescription;
    [SerializeField] List<TooltipPanel> hints;

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    void Show(IHasInfo info)
    {
        txtTitle.text = info.GetTitle();
        txtDescription.text = info.GetDescription();

        var tooltips = info.GetTooltips()
            .Take(hints.Count)
            .ToList();

        int idx = 0;
        foreach (var item in hints)
        {
            if (idx < tooltips.Count)
            {
                item.Show(tooltips[idx]);
            }
            else
            {
                item.Hide();
            }
            idx++;
        }
    }

    void Show(Tile info)
    {
        txtTitle.text = info.data.title;
        txtDescription.text = info.GetDescription();
    }

    void Show(Unit unit)
    {
        var data = unit.data;

        txtTitle.text = data.title;
        txtDescription.text = unit.GetDescription();

        foreach (var item in hints) item.Hide();
        var tip = unit.GetTooltip();

        if (!string.IsNullOrEmpty(tip) && hints.Count > 0) hints[0].Show(tip);
    }

    public void Show(Collider2D collider)
    {
        if (!collider)
        {
            Hide();
            return;
        }

        if (collider.TryGetComponent<IHasInfo>(out var info))
        {
            Show(info);
            return;
        }

        if (collider.TryGetComponent<Unit>(out var unit))
        {
            Show(unit);
            return;
        }

        if (collider.TryGetComponent<Tile>(out var tile))
        {
            Show(tile);
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