using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FancyToolkit
{
    public class UIDisplayResourceCost : MonoBehaviour
    {
        [SerializeField] int cost;
        [SerializeField] ResourceType type;

        [SerializeField] TextMeshProUGUI txtCost;
        [SerializeField] Image icon;
        Sprite spriteEnough;
        [SerializeField] Sprite spriteNotEnough;
        Color colorEnough;
        [SerializeField] Color colorNotEnough = Color.white;

        [SerializeField] GameObject effectEnough;
        [SerializeField] GameObject effectNotEnough;

        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] float alphaNotEnough = 0.5f;

        bool cachedEnough;

        bool IsEnough(int value) => cost <= value;

        void Refresh(int value)
        {
            bool enough = IsEnough(value);
            if (icon)
            {
                if (spriteNotEnough) icon.sprite = enough ? spriteEnough : spriteNotEnough;
                icon.color = enough ? colorEnough : colorNotEnough;
            }
            
            if (canvasGroup)
            {
                canvasGroup.alpha = enough ? 1 : alphaNotEnough;
            }
            cachedEnough = enough;
        }

        private void Awake()
        {
            if (!icon) return;
            if (spriteNotEnough) spriteEnough = icon.sprite;
            colorEnough = icon.color;
        }

        private void Start()
        {
            txtCost.SetText(cost.ToString());
        }

        public void Init(int cost, ResourceType type)
        {
            this.cost = cost;
            this.type = type;
            txtCost.SetText(cost.ToString());
            Refresh(ResourceCtrl.current.Get(type));
        }

        private void OnEnable()
        {
            ResourceCtrl.OnChanged += HandleResourceChanged;
            var value = ResourceCtrl.current.Get(type);
            Refresh(value);
        }

        private void OnDisable()
        {
            ResourceCtrl.OnChanged -= HandleResourceChanged;
        }

        void HandleResourceChanged(ResourceType type, int value)
        {
            if (this.type != type) return;
            if (IsEnough(value) == cachedEnough) return;

            
            //var effect = !cachedEnough ? effectEnough : effectNotEnough;
            //if (effect) StartCoroutine(BlinkEffect(effect, 0.12f));

            Refresh(value);
        }

        IEnumerator BlinkEffect(GameObject effect, float time)
        {
            effect.SetActive(true);
            yield return new WaitForSeconds(time);
            effect.SetActive(false);
        }
    }
}
