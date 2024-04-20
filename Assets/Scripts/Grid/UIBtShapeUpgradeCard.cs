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

    public void Init(BtShapeData data, BtBlockData block)
    {
        this.data = data;
        if (txtDescription) txtDescription.text = block.GetDescription();

        shapeIcon.Init(data);
    }

    public bool Select()
    {
        BtUpgradeCtrl.Select(data);
        return true;
    }
}