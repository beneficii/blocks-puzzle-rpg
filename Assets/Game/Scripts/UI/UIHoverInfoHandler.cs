using FancyToolkit;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverInfoHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        var hoverCtrl = UIHoverInfoCtrl.current;
        if (hoverCtrl) hoverCtrl.LockOnUIElement(transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var hoverCtrl = UIHoverInfoCtrl.current;
        if (hoverCtrl) hoverCtrl.LockOnUIElement(null);
    }
}
