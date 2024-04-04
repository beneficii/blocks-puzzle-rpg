using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace FancyToolkit
{
    public class FloatingText : MonoBehaviour
    {
        public TMP_Text txt;
        public TMP_Text txtSecondary;
        public float lifetime = 0.6f;
        public float distance = 0.4f;
        public float defaultCooldown = 0.15f;

        public Color defaultColor = Color.white;

        float startTime = 0f;
        Vector3 startPosition;

        //buffer stuff
        Color color;


        bool started;

        public Color TextColor
        {
            set
            {
                txt.color = value;
                if (txtSecondary) txtSecondary.color = value;
            }
        }

        public string TextText
        {
            set
            {
                txt.text = value;
                if (txtSecondary) txtSecondary.text = value;
            }
        }

        private void Start()
        {
            startTime = Time.time;
            startPosition = transform.localPosition;
        }

        public void SetColor(Color color)
        {
            TextColor = color;

            this.color = color;
        }

        public void SetLifetime(float lifetime)
        {
            this.lifetime = lifetime;
        }

        public static string Colorize(string text, Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
        }

        // Use it from prefab
        public FloatingText Create(string msg, Vector3 position, Transform parent = null)
        {
            return Create(this, msg, position, parent);
        }

        // use from template
        public FloatingText CreateFromLocal(string msg, float yDelta = 0f)
        {
            var instance = Create(this, msg, transform.position + Vector3.up * yDelta, transform.parent);
            instance.gameObject.SetActive(true);
            return instance;
        }

        public static FloatingText Create(FloatingText prefab, string msg, Vector3 position, Transform parent = null)
        {
            var instance = Instantiate(prefab, position, Quaternion.identity, parent);
            instance.TextText = msg;
            instance.SetColor(instance.defaultColor);
            instance.started = true;
            return instance;
        }

        public static FloatingText Create(FloatingText prefab, int value, Vector3 position, Transform parent = null)
        {
            return Create(prefab, value.ToString(), position, parent);
        }

        private void Update()
        {
            if (!started) return;
            float delta = (Time.time - startTime) / lifetime;

            if (delta > 1f)
            {
                Destroy(gameObject);
                return;
            }

            color.a = 1 - Easings.InCubic(delta);
            TextColor = color;
            transform.localPosition = startPosition + Vector3.up * Easings.OutCubic(delta) * distance;
        }
    }
}
