using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitMoveIndicator : MonoBehaviour
{
    [SerializeField] SpriteRenderer icon;
    [SerializeField] TextMeshPro txtShortDescription;

    public void Init(Unit parent, UnitActionBase action)
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
        icon.sprite = action.sprite;
        txtShortDescription.text = action.GetShortDescription(parent);
    }
}