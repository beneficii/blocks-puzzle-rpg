using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FancyToolkit;

public class UIBtShapeUpgradeCard : MonoBehaviour, IUIUpgradeChoise
{
    [SerializeField] TextMeshProUGUI txtTitle;
    [SerializeField] TextMeshProUGUI txtDescription;
    [SerializeField] TextMeshProUGUI txtTooltip;
    [SerializeField] UIBtShapeIcon shapeIcon;

    GameObject TooltipParent => txtTooltip.gameObject; //txtTooltip.transform.parent.gameObject;
    BtShapeData data;

    public void Init(BtShapeData data, BtBlockInfo block)
    {
        this.data = data;
        var blockData = block.data;
        if (txtTitle) txtTitle.text = blockData.title;
        if (txtDescription) txtDescription.text = blockData.GetDescription();
        TooltipParent.SetActive(false);

        if (block.data is IHasInfo info)
        {
            var tips = info.GetTooltips();
            if (tips.Count > 0)
            {
                TooltipParent.SetActive(true);
                txtTooltip.text = string.Join("\n", tips);
            }
        }

        shapeIcon.Init(data, block);
    }

    public bool Select()
    {
        BtUpgradeCtrl.current.Select(data);
        return true;
    }
}