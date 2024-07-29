using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine.UI;

namespace FancyToolkit
{
    public class UIGenericResourceDisplay<TRes> : MonoBehaviour
        where TRes : struct, System.Enum
    {
        public TRes type;
        public TMP_Text txtValue;
        public TRes secondaryType;
        public Slider slider;
        public DisplayFormat format = DisplayFormat.Default;
        public GameObject hideWhenZero;
        public float animationSpeed = 0;
        [SerializeField] FloatingText floater;
        [SerializeField] bool debug;
        CooldownComponent cooldown;

        [SerializeField] Image iconMain;
        [SerializeField] List<CriticalLimits> criticalSprites;
        [SerializeField] GameObject effectRemove;
        [SerializeField] GameObject effectAdd;

        Queue<int> popups = null;

        int cachedValue = 0;
        int cachedMaxValue = -1;
        int targetValue = 0;

        bool hasSecondary;

        bool avoidStartAnimation = true;

        public bool IsNone(TRes type)
        {
            return System.Convert.ToInt32(type) == 0;
        }

        protected virtual IEnumerator Start()
        {
            yield return null;
            if (floater)
            {
                cooldown = new CooldownComponent(floater.defaultCooldown);
                popups = new();
            }

            yield return new WaitForSeconds(0.2f);

            avoidStartAnimation = false;
        }

        protected virtual void OnEnable()
        {
            hasSecondary = IsNone(secondaryType);
            ResCtrl<TRes>.OnChanged += HandleResourceChange;
            cachedValue = targetValue = ResCtrl<TRes>.current.Get(type);
            if (debug) Debug.Log($"Enable {type}: {targetValue}");
            if (hasSecondary) cachedMaxValue = ResCtrl<TRes>.current.Get(secondaryType);
            UpdateValue(targetValue);
            if (hideWhenZero && targetValue == 0) hideWhenZero.SetActive(false);
        }

        protected virtual void OnDisable()
        {
            ResCtrl<TRes>.OnChanged -= HandleResourceChange;
        }

        string GetString(int value, int secondary = 0)
        {
            return format switch
            {
                DisplayFormat.Max => $"{value} / {secondary}",
                DisplayFormat.Bonus => $"{(value >= 0 ? "+" : "")}{value}",
                _ => $"{value}",
            };
        }

        void HandleResourceChange(TRes type, int value)
        {
            if (type.Equals(this.type))
            {
                targetValue = value;
            }

            if (type.Equals(this.secondaryType))
            {
                cachedMaxValue = value;
                UpdateValue(cachedValue);
            }
        }

        void UpdateValue(int value)
        {
            bool isZero = value == 0;
            if (hideWhenZero && ((cachedValue == 0) != (isZero))) hideWhenZero.SetActive(!isZero);
            int delta = value - cachedValue;
            if (popups != null && delta != 0) popups.Enqueue(delta);
            cachedValue = value;

            if (debug) Debug.Log($"UpdateValue {type}: {value}");
            txtValue.SetText(GetString(value, cachedMaxValue));

            if (hasSecondary && slider)
            {
                slider.maxValue = cachedMaxValue;
                slider.value = cachedValue;
            }

            if (iconMain && criticalSprites != null && criticalSprites.Count > 0)
            {
                var item = criticalSprites.FirstOrDefault(x => x.value < value);
                if (item != null) iconMain.sprite = item.sprite;
            }
        }

        IEnumerator BlinkEffect(GameObject effect, float time)
        {
            effect.SetActive(true);
            yield return new WaitForSeconds(time);
            effect.SetActive(false);
        }

        private void Update()
        {
            if (popups != null && popups.Count > 0 && cooldown.Use())
            {
                int delta = popups.Dequeue();
                string text = delta > 0 ? $"+{delta}" : $"{delta}";
                floater.Create(text, txtValue.transform.position, txtValue.transform);
            }

            if (cachedValue != targetValue)
            {
                if (!avoidStartAnimation)
                {
                    var effect = cachedValue > targetValue ? effectRemove : effectAdd;
                    if (effect) StartCoroutine(BlinkEffect(effect, 0.12f));
                }

                if (animationSpeed <= 0)
                {
                    UpdateValue(targetValue);
                }
                else
                {
                    int difference = targetValue - cachedValue;

                    float moveAmount = Mathf.Clamp(Mathf.Abs(difference) * animationSpeed * Time.deltaTime, 1, Mathf.Abs(difference));
                    int moveDirection = (int)Mathf.Sign(difference);
                    UpdateValue(cachedValue + moveDirection * Mathf.RoundToInt(moveAmount));
                }
            }
        }

        [System.Serializable]
        public enum DisplayFormat
        {
            Default,
            Single,
            Max,
            Bonus,
        }

        [System.Serializable]
        public class CriticalLimits
        {
            public int value;
            public Sprite sprite;
        }
    }
}