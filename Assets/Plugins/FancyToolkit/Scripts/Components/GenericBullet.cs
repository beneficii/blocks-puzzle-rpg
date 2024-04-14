using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FancyToolkit
{
    public class GenericBullet : MonoBehaviour
    {
        [SerializeField] float speed = 10;

        SpriteRenderer _render;
        public SpriteRenderer render
        {
            get
            {
                if (!_render) _render = GetComponent<SpriteRenderer>();

                return _render;
            }
        }

        System.Action<Component> action;
        Component target;
        int damage;

        float launchAt = -1f;



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

        /*
        private void Update()
        {
            if (!target)
            {
                Destroy(gameObject);
                return;
            }

            transform.up = transform.position - target.transform.position;
        }*/

        private void FixedUpdate()
        {
            if (!target || !target.gameObject.activeSelf)
            {
                Destroy(gameObject);
                return;
            }

            if (Time.time < launchAt) return;

            if (transform.MoveTowards(target.transform.position, Time.fixedDeltaTime * speed))
            {
                if (action != null)
                {
                    action.Invoke(target);
                }

                if (target.TryGetComponent<IDamagable>(out var damagable))
                {
                    if (damage >= 0)
                    {
                        damagable.RemoveHp(damage);
                    }
                    else
                    {
                        damagable.AddHp(-damage);
                    }
                }

                Destroy(gameObject);
                return;
            }
        }
    }
    
    public interface IDamagable
    {
        void RemoveHp(int value);
        void AddHp(int value);
    }
}

