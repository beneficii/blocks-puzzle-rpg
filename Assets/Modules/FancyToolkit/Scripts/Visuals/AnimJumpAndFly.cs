using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FancyToolkit;

namespace FancyToolkit
{
    public class AnimJumpAndFly : MonoBehaviour
    {
        [SerializeField] ParticleSystem particle;
        [SerializeField] SpriteRenderer render;

        // jump
        bool doJump = false;
        bool canJump = true;
        float jumpSpeed;
        float jumpMinSpeed;
        float jumpDeceleration;

        // rotation
        bool doRotation = false;
        float rotation;

        // target
        bool doFlight = false;
        bool canFly = true;
        bool isWorldTarget;
        RectTransform target;
        Transform worldTarget;
        Vector2 targetPos;
        float flightSpeed;

        // scale
        bool doScale = false;
        float scaleA;
        float scaleB;
        float scaleSpeed;

        // spread
        float spread;

        // gravity
        bool keepGravity = true;

        // callback
        System.Action callback;

        public AnimJumpAndFly SetDefaults()
        {
            doJump = false;
            doFlight = false;
            doScale = false;
            doRotation = false;
            canJump = true;
            canFly = true;
            callback = null;

            return this;
        }

        Vector2 GetTargetPos()
        {
            if (isWorldTarget)
            {
                if (worldTarget) return worldTarget.transform.position.RandomAround(spread);
                Destroy(gameObject);
                return Vector2.zero;
            }

            if (target) return target.TransformPoint(Vector2.zero).RandomAround(spread);
            Destroy(gameObject);
            return Vector2.zero;
        }

        public AnimJumpAndFly SetSprite(Sprite sprite)
        {
            render.sprite = sprite;
            return this;
        }

        public AnimJumpAndFly SetColor(Color color)
        {
            render.color = color;
            return this;
        }

        public AnimJumpAndFly AddCallback(System.Action callback)
        {
            this.callback = callback;
            return this;
        }

        public AnimJumpAndFly SetGravity(bool value)
        {
            keepGravity = value;
            return this;
        }

        public AnimJumpAndFly AddJump(float speed, float duration, bool blockFly = true)
        {
            doJump = true;
            if (blockFly) canFly = false;
            
            jumpSpeed = speed;
            jumpMinSpeed = -speed;
            jumpDeceleration = speed / duration;

            return this;
        }

        public AnimJumpAndFly AddRotation()
        {
            rotation = Random.Range(180, 360f) * (Random.value > 0.5f ? 1 : -1);

            return this;
        }

        public AnimJumpAndFly AddTarget(RectTransform target, float speed)
        {
            doFlight = true;
            this.target = target;
            this.isWorldTarget = false;
            this.targetPos = GetTargetPos();
            this.flightSpeed = speed;

            return this;
        }

        public AnimJumpAndFly AddTarget(Transform target, float speed)
        {
            doFlight = true;
            this.worldTarget = target;
            this.isWorldTarget = true;
            this.targetPos = GetTargetPos();
            this.flightSpeed = speed;

            return this;
        }

        public AnimJumpAndFly AddSpread(float radius)
        {
            spread = radius;
            return this;
        }


        public AnimJumpAndFly AddResize(float scaleA, float scaleB, float time)
        {
            doScale = true;
            this.scaleA = transform.localScale.x;
            this.scaleB = scaleB;
            scaleSpeed = Mathf.Abs(scaleB - this.scaleA) / time;

            return this;
        }

        public void Jump()
        {
            transform.position += jumpSpeed * Time.deltaTime * Vector3.up;
            jumpSpeed -= jumpDeceleration * Time.deltaTime;
            if (jumpSpeed < jumpMinSpeed / 2)
            {
                this.targetPos = GetTargetPos();
                canFly = true;
                if (!keepGravity) doJump = false;
            }

            if (jumpSpeed <= jumpMinSpeed)
            {
                doJump = false;
            }
        }

        public void Fly()
        {
            //if ((isWorldTarget && !worldTarget) || (!isWorldTarget && !target))
            {
            //    Destroy(gameObject);
            //    return;
            }

            if (transform.MoveTowards(targetPos, flightSpeed * Time.deltaTime))
            {
                if (particle) Instantiate(particle, transform.position, Quaternion.identity);
                callback?.Invoke();
                Destroy(gameObject);
            }
        }

        public void Rotate()
        {
            transform.Rotate(0, 0, rotation * Time.deltaTime);
        }

        public void Resize()
        {
            scaleA = Mathf.MoveTowards(scaleA, scaleB, scaleSpeed * Time.deltaTime);
            transform.localScale = Vector3.one * scaleA;

            if (scaleA == scaleB)
            {
                doScale = false;
            }
        }

        private void Update()
        {
            if (doRotation) Rotate();
            if (doJump && canJump) Jump();
            if (doFlight && canFly) Fly();
            if (doScale) Resize();
        }
    }
}
