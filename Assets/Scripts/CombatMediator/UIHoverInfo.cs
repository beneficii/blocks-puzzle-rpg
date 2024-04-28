using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FancyToolkit;

public class UIHoverInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtTitle;
    [SerializeField] TextMeshProUGUI txtDescription;
    //[SerializeField] Image imgIcon;

    void Show(BtBlock block)
    {
        var data = block.data;
        if (data.type == BtBlockType.None)
        {
            gameObject.SetActive(false);
            return;
        }

        txtTitle.text = data.title;
        txtDescription.text = data.GetDescription();
    }

    void Show(Unit unit)
    {
        var data = unit.data;

        txtTitle.text = data.title;
        txtDescription.text = unit.GetDescription();
    }

    public void Show(Collider2D collider)
    {
        if (!collider)
        {
            gameObject.SetActive(false);
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

        gameObject.SetActive(false);
    }

}