﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitMoveIndicator : MonoBehaviour
{
    [SerializeField] Transform actionParent;
    [SerializeField] TextMeshPro txtShortDescription;

    GameObject currentActionVisual;

    public void Init(Unit parent, UnitAction.Base action)
    {
        if (action == null)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }

        if (currentActionVisual)
        {
            Destroy(currentActionVisual);
        }

        currentActionVisual = Instantiate(action.GetIndicatorPrefab(), actionParent);
        txtShortDescription.text = action.GetShortDescription(parent);
    }
}