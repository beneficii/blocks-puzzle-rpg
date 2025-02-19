﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FancyToolkit
{
    public class GenericBullet : MonoBehaviour
    {
        [SerializeField] float speed = 10;

        AudioClip sound;

        SpriteRenderer _render;
        public SpriteRenderer render
        {
            get
            {
                if (!_render) _render = GetComponent<SpriteRenderer>();

                return _render;
            }
        }

        AnimCompanion fxPrefab;

        System.Action<Component> action;
        Component target;
        int damage;

        float launchAt = -1f;

        Vector2 spleenPoint;
        Vector2 spleenDirection = Vector2.zero;

        bool HasSpleen => spleenDirection != Vector2.zero;

        bool faceTarget = false;

        public GenericBullet Init(Component target) => SetTarget(target);

        public GenericBullet SetTarget(Component target)
        {
            this.target = target;
            return this;
        }

        public GenericBullet SetSprite(Sprite sprite)
        {
            render.sprite = sprite;
            return this;
        }

        public GenericBullet SetFx(AnimCompanion prefab)
        {
            fxPrefab = prefab;
            return this;
        }

        public GenericBullet SetSpleen(Vector2 direction)
        {
            spleenDirection = direction;
            return this;
        }

        public GenericBullet SetLaunchDelay(float delay)
        {
            launchAt = Time.time + delay;
            return this;
        }

        public GenericBullet SetAction(System.Action<Component> action)
        {
            this.action = action;
            return this;
        }

        public GenericBullet FaceTarget()
        {
            this.faceTarget = true;
            return this;
        }

        public GenericBullet SetSpeed(float speed)
        {
            this.speed = speed;
            return this;
        }

        public GenericBullet SetDamage(int damage)
        {
            this.damage = damage;
            return this;
        }

        public GenericBullet SetHealing(int heal)
        {
            this.damage = -heal;
            return this;
        }

        public GenericBullet SetAudio(AudioClip clip)
        {
            this.sound = clip;
            return this;
        }

        public IEnumerator Wait()
        {
            return new WaitWhile(() => this);
        }

        void TargetReached()
        {
            if (!target)
            {
                Destroy(gameObject);
                return;
            }

            if (action != null)
            {
                action.Invoke(target);
            }

            if (sound) sound.PlayWithRandomPitch(.25f);

            if (target.TryGetComponent<IDamagable>(out var damagable))
            {
                if (damage > 0)
                {
                    damagable.RemoveHp(damage);
                }
                else if (damage < 0)
                {
                    damagable.AddHp(-damage);
                }
            }

            Destroy(gameObject);
        }

        private void Start()
        {
            if (!target) return;
            if (HasSpleen)
            {
                //var distance = Vector2.Distance(target.transform.position, transform.position);
                spleenPoint = (Vector2)target.transform.position + spleenDirection * 7;
            }
        }

        void Move()
        {
            var targetPos = (Vector2)target.transform.position;
            float moveSpeed = Time.fixedDeltaTime * speed;

            if (HasSpleen)
            {
                spleenPoint = Vector2.MoveTowards(spleenPoint, targetPos, moveSpeed);
                if (spleenPoint == targetPos) spleenDirection = Vector2.zero;

                if (transform.MoveTowards(spleenPoint, moveSpeed * 0.4f))
                {
                    spleenDirection = Vector2.zero;
                }
                return;
            }


            if (!transform.MoveTowards(targetPos, moveSpeed)) return;

            if (fxPrefab)
            {
                Instantiate(fxPrefab, transform.position, Quaternion.identity)
                    .SetTriggerAction(TargetReached);
                gameObject.SetActive(false);    // hide bullet during animation
            }
            else
            {
                TargetReached();
            }
        }

        private void FixedUpdate()
        {
            if (!target || !target.gameObject.activeSelf)
            {
                Destroy(gameObject);
                return;
            }

            if (Time.time < launchAt) return;

            if (faceTarget) transform.up = target.transform.position - transform.position;
            Move();
        }
    }
    
    public interface IDamagable
    {
        void RemoveHp(int value);
        void AddHp(int value);
    }
}

