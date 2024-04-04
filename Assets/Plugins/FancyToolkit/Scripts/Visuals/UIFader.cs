using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FancyToolkit
{
    [DefaultExecutionOrder(-15)]
    public class UIFader : MonoBehaviour
    {
        public static UIFader current;

        public float alpha = 0f;
        public float speed = 3.5f;
        public CanvasGroup cg;

        public bool IsDone() => cg.alpha == alpha;

        private void Awake()
        {
            current = this;
        }

        void Update()
        {
            if (!IsDone())
            {
                cg.alpha = Mathf.MoveTowards(cg.alpha, alpha, Time.deltaTime);
            }
        }

        [ContextMenu("Setup components")]
        void SetupComponents()
        {
            cg = gameObject.AddComponent<CanvasGroup>();
        }
    }
}
