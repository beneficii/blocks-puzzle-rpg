using FancyToolkit;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Assertions;


namespace FancyToolkit
{
    public class InfoDisplayCtrl : MonoBehaviour
    {
        public static System.Action<IHoverInfoTarget> OnHovered;

        static InfoDisplayCtrl _current;
        public static InfoDisplayCtrl current
        {
            get
            {
                if (!_current) _current = FindAnyObjectByType<InfoDisplayCtrl>();

                return _current;
            }
        }

        [SerializeField] RectTransform panelParent;
        [SerializeField] UIInfoPanel infoPanel;
        [SerializeField] List<UIInfoPanel> hintPanelList;

        IHoverInfoTarget cachedObject;

        IHoverInfoTarget GetHoverInfoTarget(Component obj)
        {
            if(obj.TryGetComponent<IHoverInfoTarget>(out var info))
            {
                return info;
            }

            if (obj.TryGetComponent<IHoverInfoContainer>(out var container))
            {
                return container.GetHoverInfoTarget();
            }

            return null;
        }

        IHoverInfoTarget GetInfoSourceUnderMouse()
        {
            Canvas uiCanvas = null;
            IHoverInfoTarget uiTarget = null;
            SpriteRenderer worldSource = null;
            IHoverInfoTarget worldTarget = null;

            // try to find UI element
            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var results = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointerData, results);


            if (results.Count > 0)
            {
                var obj = results[0].gameObject.transform;
                uiCanvas = obj.GetComponentInParent<Canvas>();
                
                Assert.IsTrue(uiCanvas, "Found UI element without canvas!");
                do
                {
                    var target = GetHoverInfoTarget(obj);
                    if (target?.ShouldShowHoverInfo()??false)
                    {
                        uiTarget = target;
                        if (!uiCanvas) return target;   // should not happen really
                        break;
                    }

                    obj = obj.parent;
                } while (obj);
            }
            

            // try to find world info Objects
            var hits = Physics2D.RaycastAll(Helpers.MouseToWorldPosition(), Vector2.zero, 10);

            foreach (var hit in hits)
            {
                var tr = hit.transform;
                if (!tr) continue;
                var target = GetHoverInfoTarget(tr);
                if (target?.ShouldShowHoverInfo() ?? false)
                {
                    worldTarget = target;
                    worldSource = tr.GetComponentInParent<SpriteRenderer>();
                    if (!worldSource) worldSource = tr.GetComponentInChildren<SpriteRenderer>();
                    if (!worldSource)
                    {
                        return uiCanvas ? uiTarget : target;
                    }
                    break;
                }
            }
            // check which one was higher in priority
            if (!uiCanvas) return worldTarget;
            if (worldTarget == null || !worldSource) return uiTarget;

            if (worldSource.sortingLayerID < uiCanvas.sortingLayerID) return uiTarget;
            if (worldSource.sortingLayerID > uiCanvas.sortingLayerID) return worldTarget;

            return worldSource.sortingOrder < uiCanvas.sortingOrder ? uiTarget : worldTarget;
        }

        public void InitHints(IHintContainer hintContainer)
        {
            List<IHintProvider> hintProviders = hintContainer?.GetHintProviders() ?? new();
            hintProviders = hintProviders.Where(x => x is not null).ToList();
            for (int i = 0; i < hintPanelList.Count; i++)
            {
                var ui = hintPanelList[i];

                if (i < hintProviders.Count)
                {
                    ui.gameObject.SetActive(true);
                    ui.InitHintText(hintProviders[i]);
                }
                else
                {
                    ui.gameObject.SetActive(false);
                }
            }
        }

        void Update()
        {
            var target = GetInfoSourceUnderMouse();

            if (target == cachedObject) return;
            cachedObject = target;
            if (target == null)
            {
                panelParent.gameObject.SetActive(false);
                return;
            }

            OnHovered?.Invoke(target);

            panelParent.gameObject.SetActive(true);
            if (target is IIconProvider iconProvider)
            {
                infoPanel.InitIcon(iconProvider);
            }
            else
            {
                infoPanel.InitIcon(null);
            }

            bool mainPanelVisible = false;
            if (target is IInfoTextProvider infoText)
            {
                mainPanelVisible = true;
                infoPanel.InitText(infoText);
            }
            else if (target is IHintProvider infoHint)
            {
                mainPanelVisible = true;
                infoPanel.InitHintText(infoHint);
            }
            infoPanel.gameObject.SetActive(mainPanelVisible);

            if (target is IHintContainer hintContainer)
            {
                InitHints(hintContainer);
            }
            else
            {
                InitHints(null);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(panelParent);
        }
    }

    public interface IHoverInfoTarget
    {
        bool ShouldShowHoverInfo();
    }

    public interface IHoverInfoContainer
    {
        IHoverInfoTarget GetHoverInfoTarget();
    }

    public interface IInfoTextProvider
    {
        string GetInfoText(int size);
    }

    public interface IHintProvider
    {
        string GetHintText();
    }

    public interface IHintContainer
    {
        List<IHintProvider> GetHintProviders();
    }

    public interface IIconProvider
    {
        Sprite GetIcon();
    }

    public static class TextMeshProExtensions
    {
        public static string Bold(this string text) => $"<b>{text}</b>";
        public static string Center(this string text) => $"<align=\"center\">{text}</align>";
        
        public static string Alpha(this string text, byte alpha) => $"<alpha=#{alpha:X2}>{text}</color>";

        public static string Size(this string text, int size) => $"<size={size}>{text}</size>";
    }
}