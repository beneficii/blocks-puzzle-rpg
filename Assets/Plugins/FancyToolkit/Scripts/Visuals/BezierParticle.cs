using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FancyToolkit
{
    public class BezierParticle : MonoBehaviour
    {
        [SerializeField] GameObject prefabParticle;
        [SerializeField] public SpriteRenderer render;
        [SerializeField] TrailRenderer trailRenderer;

        Transform target;
        Vector2 origin;

        Vector2 midPoint1;
        Vector2 midPoint2;

        float time;
        float duration;

        bool hasParams = false;

        System.Action action;
        System.Action cleanup;

        [SerializeField] float radius = 2f;

        public BezierParticle Init(Transform target, float duration = 0.6f)
        {
            this.target = target;
            
            origin = transform.position;
            this.duration = duration;
            this.time = 0;

            return this;
        }

        public BezierParticle SetIconParams(Color color, float scale = 1f)
        {
            render.color = color;
            render.transform.localScale = Vector3.one * scale;

            return this;
        }

        public BezierParticle SetVisuals(TrailParticleVisuals visuals)
        {
            visuals.InitIcon(render);
            visuals.InitTrail(trailRenderer);

            return this;
        }

        public BezierParticle SetAction(System.Action action)
        {
            this.action = action;
            return this;
        }
        public BezierParticle SetCleanup(System.Action action)
        {
            this.cleanup = action;
            return this;
        }

        public BezierParticle SetRadius(float value)
        {
            radius = value;

            return this;
        }

        public BezierParticle SetDefaultFlight()
        {
            var targetPos = (Vector2)target.position;

            float deltaY = (origin.y > targetPos.y) ? -radius : radius;
            float deltaX = (origin.y > targetPos.y) ? -radius : radius;
            var delta = new Vector2(deltaX, deltaY);

            midPoint1 = origin + delta;
            midPoint2 = targetPos - delta;

            return this;
        }

        private void Start()
        {
            if (!hasParams) SetDefaultFlight();
        }

        private void Update()
        {
            if (!target)
            {
                cleanup?.Invoke();
                Destroy(gameObject);
                return;
            }
            time += Time.deltaTime;
            var t = time / duration;
            Vector2 newPosition = BezierCurve(origin, midPoint1, midPoint2, target.position, t);
            transform.position = newPosition;

            // Check if the particle has reached the target position
            if (time > duration)
            {
                // Destroy the object
                action?.Invoke();
                cleanup?.Invoke();
                if (prefabParticle) Instantiate(prefabParticle, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

        private Vector2 BezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            // Bezier curve calculation
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector2 p = uuu * p0; // (1-t)^3 * p0
            p += 3 * uu * t * p1; // 3(1-t)^2 * t * p1
            p += 3 * u * tt * p2; // 3(1-t) * t^2 * p2
            p += ttt * p3; // t^3 * p3

            return p;
        }
    }


    [System.Serializable]
    public class TrailParticleVisuals
    {
        public Sprite sprite;
        public Color color1 = Color.white;
        public Color color2 = Color.black;

        public TrailParticleVisuals(Sprite sprite, Color color1, Color color2)
        {
            this.sprite = sprite;
            this.color1 = color1;
            this.color2 = color2;
        }

        public void InitIcon(SpriteRenderer render)
        {
            render.sprite = sprite;
        }

        public void InitTrail(TrailRenderer render)
        {
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];

            // Set the color keys
            colorKeys[0].color = color1;
            colorKeys[0].time = 0f;
            colorKeys[1].color = color2;
            colorKeys[1].time = 1f;

            // Set the alpha keys (optional)
            alphaKeys[0].alpha = color1.a;
            alphaKeys[0].time = 0f;
            alphaKeys[1].alpha = color2.a;
            alphaKeys[1].time = 1f;

            // Assign color and alpha keys to the gradient
            gradient.SetKeys(colorKeys, alphaKeys);

            render.colorGradient = gradient;
        }
    }
}