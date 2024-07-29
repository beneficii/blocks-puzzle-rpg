using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace FancyToolkit
{
    public class CooldownComponent
    {
        public event System.Action OnChanged;

        float next;
        float randomrange = 0f;
        float cooldown;
        public float Cooldown
        {
            get => cooldown;
            set
            {
                float delta = cooldown - value;
                next += delta;
                cooldown = value;

                OnChanged?.Invoke();
            }
        }

        void SetNext(float nextCd = -1)
        {
            float cd = nextCd >= 0 ? nextCd : cooldown;
            next = Time.time + cd + Random.Range(0, randomrange);
        }

        public CooldownComponent(float cooldown, float randomrange = 0f)
        {
            this.cooldown = cooldown;
            this.randomrange = randomrange;
            SetNext();
        }

        public bool CanUse => Time.time >= next;
        public bool Expired => CanUse;

        public bool Use(float nextCooldown = -1)
        {
            if (CanUse)
            {
                SetNext(nextCooldown);
                return true;
            }

            return false;
        }

        public void Pause(bool value = true)
        {
            if (value)
            {
                next = float.MaxValue;
            }
            else
            {
                next = Time.time + cooldown;
            }
        }

        public void Restore()
        {
            next = 0f;
        }
    }
}