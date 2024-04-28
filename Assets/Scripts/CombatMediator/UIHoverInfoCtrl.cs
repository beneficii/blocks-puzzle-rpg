using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FancyToolkit;

public class UIHoverInfoCtrl : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] UIHoverInfo infoTop;
    [SerializeField] UIHoverInfo infoBottom;

    Collider2D currentColider;

    void ShowCollider(Collider2D collider)
    {
        currentColider = collider;
        infoTop.gameObject.SetActive(false);
        infoBottom.gameObject.SetActive(false);
        if (collider == null)
        {
            return;
        }

        Vector3 screenPoint = Helpers.Camera.WorldToScreenPoint(collider.transform.position);

        var screenMidPoint = Screen.height / 2f;

        var info = screenPoint.y > screenMidPoint ? infoBottom : infoTop;

        info.gameObject.SetActive(true);
        info.Show(collider);
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Helpers.MouseToWorldPosition(), Vector2.zero, 10, layerMask);

        if (currentColider != hit.collider)
        {
            ShowCollider(hit.collider);
        }
    }
}