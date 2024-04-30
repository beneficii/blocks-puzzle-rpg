using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FancyToolkit;

public class UIHoverInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtTitle;
    [SerializeField] TextMeshProUGUI txtDescription;
    [SerializeField] List<TooltipPanel> hints;

    private void Start()
    {
        Hide();
    }

    void Hide()
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

    void Show(BtBlock block)
    {
        if (block.data is IHasInfo info)
        {
            Show(info);
        }
        else
        {
            Hide();
        }
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

        if (collider.TryGetComponent<BtBlock>(out var block))
        {
            Show(block);
            return;
        }

        if (collider.TryGetComponent<Unit>(out var unit))
        {
            Show(unit);
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

public interface IHasInfo
{
    public string GetTitle();
    public string GetDescription();
    public List<string> GetTooltips();
}