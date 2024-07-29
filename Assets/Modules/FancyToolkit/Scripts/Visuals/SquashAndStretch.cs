using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FancyToolkit
{
    public class SquashAndStretch : MonoBehaviour
    {
        public AnimationCurve curve;

        float endTime = -1f;
        float duration = -1f;
        //System.Func<float, float> curve = null;

        Vector3 scaleFrom;
        Vector3 scaleTo;

        public void Run(float duration, Vector3 scaleFrom)
        {
            this.duration = duration;
            //this.curve = curve;
            endTime = Time.time + duration;
            this.scaleFrom = scaleFrom;
            scaleTo = transform.localScale;
        }

        [ContextMenu("Test it")]
        public void Test()
        {
            Run(0.5f, new Vector3(0.4f, 1.6f, 1f));
        }

        void Update()
        {
            AnimateSquashAndStretch();
            /*
            if (Input.GetKeyDown(KeyCode.Y))
            {
                Test();
            }*/
        }

        void AnimateSquashAndStretch()
        {
            if (endTime < 0f) return;

            float timeLeft = ((endTime - Time.time) / duration);
            float value = curve.Evaluate(1 - timeLeft);

            transform.localScale = new Vector3
            {
                x = Mathf.LerpUnclamped(scaleFrom.x, scaleTo.x, value),
                y = Mathf.LerpUnclamped(scaleFrom.y, scaleTo.y, value),
                z = Mathf.LerpUnclamped(scaleFrom.z, scaleTo.z, value),
            };

            if (timeLeft <= 0)
            {
                endTime = -1f;
            }
        }
    }
}