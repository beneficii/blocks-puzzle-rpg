using FancyToolkit;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverInfoHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    UIHoverInfoCtrl hoverInfoCtrl;

    private void Start()
    {
        hoverInfoCtrl = FindAnyObjectByType<UIHoverInfoCtrl>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverInfoCtrl.LockOnUIElement(transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverInfoCtrl.LockOnUIElement(null);
    }
}
