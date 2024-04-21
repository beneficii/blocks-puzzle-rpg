using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FancyToolkit;

public class UIBtShapeUpgradeCard : MonoBehaviour, IUIUpgradeChoise
{
    [SerializeField] TextMeshProUGUI txtDescription;
    [SerializeField] UIBtShapeIcon shapeIcon;

    BtShapeData data;

    public void Init(BtShapeData data, BtBlockInfo block)
    {
        this.data = data;
        var blockData = block.data;
        if (txtDescription) txtDescription.text = blockData.GetDescription();

        shapeIcon.Init(data, block);
    }

    public bool Select()
    {
        BtUpgradeCtrl.Select(data);
        return true;
    }
}