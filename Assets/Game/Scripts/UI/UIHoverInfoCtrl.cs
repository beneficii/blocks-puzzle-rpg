﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FancyToolkit;

public class UIHoverInfoCtrl : MonoBehaviour
{
    public static System.Action<Transform> OnHovered;

    public static UIHoverInfoCtrl _current;
    public static UIHoverInfoCtrl current
    {
        get
        {
            if (!_current)
            {
                _current = FindFirstObjectByType<UIHoverInfoCtrl>();
            }

            return _current;
        }
    }

    [SerializeField] LayerMask layerMask;
    [SerializeField] UIHoverInfo infoPanel;

    Transform currentColider;
    Transform currentUIElement;

    private void Start()
    {
        ShowCollider(null);
    }

    public void LockOnUIElement(Transform collider)
    {
        currentUIElement = collider;
        ShowCollider(collider);
    }

    public void HideCollider(Transform collider)
    {
        if (currentColider == collider)
        {
            ShowCollider(null);
        }
    }

    public void ShowCollider(Transform collider)
    {
        currentColider = collider;
        infoPanel.gameObject.SetActive(false);
        if (collider == null)
        {
            return;
        }

        Vector3 screenPoint = Helpers.Camera.WorldToScreenPoint(collider.transform.position);

        var screenMidPoint = Screen.height / 2f;

        infoPanel.gameObject.SetActive(true);
        infoPanel.Show(collider);
        OnHovered?.Invoke(collider);
    }

    private void Update()
    {
#if !UNITY_ANDROID
        if (Input.GetMouseButtonDown(0))
        {
            ShowCollider(null);
        }

        if (Input.GetMouseButton(0))
        {
            return;
        }
#endif

        if (currentUIElement)
        {
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(Helpers.MouseToWorldPosition(), Vector2.zero, 10, layerMask);

        if (currentColider != hit.transform)
        {
            ShowCollider(hit.transform);
        }
    }
}