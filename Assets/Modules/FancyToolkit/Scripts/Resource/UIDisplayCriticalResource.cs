using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FancyToolkit
{
    [DefaultExecutionOrder(+10)]
    public class UIDisplayCriticalResource : MonoBehaviour
    {
        [SerializeField] int limit;
        [SerializeField] GameObject target;
        [SerializeField] ResourceType type;
        [SerializeField] float time;
        [SerializeField] CanvasGroup cg;

        bool cachedActive = false;

        bool ShouldShow(int value) => limit <= value;

        private void OnEnable()
        {
            ResourceCtrl.OnChanged += HandleResourceChanged;
            var value = ResourceCtrl.current.Get(type);
            cachedActive = ShouldShow(value);
            //HandleResourceChanged(type, value);
        }

        private void OnDisable()
        {
            ResourceCtrl.OnChanged -= HandleResourceChanged;
        }

        void HandleResourceChanged(ResourceType type, int value)
        {
            if (type != this.type) return;

            bool newActive = value <= limit;
            if (cachedActive != newActive)
            {
                cachedActive = newActive;
                StopAllCoroutines();
                target.SetActive(newActive);
                if (newActive && time > 0)
                {
                    StartCoroutine(DisableAfterTimeout());
                }
            }
        }

        IEnumerator DisableAfterTimeout()
        {
            if (cg)
            {
                float blinkSpeed = 0.4f;
                float startTime = Time.time;
                while (Time.time - startTime < time)
                {
                    cg.alpha = 0.7f + Mathf.PingPong(Time.time * blinkSpeed, 0.3f);
                    yield return null;
                }
            } else
            {
                yield return new WaitForSeconds(time);
            }
            target.SetActive(false);
        }
    }
}
