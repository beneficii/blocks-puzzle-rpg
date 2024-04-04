using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace FancyToolkit
{
    [System.Serializable]
    public class HealthComponent
    {
        public float sizePerHp = 0.01f;

        public System.Action OnDeath;
        public System.Action OnDamage;
        public System.Action OnHeal;

        public TextMeshPro txtHealth;
        public Slider slider;
        public Transform worldHealthBar;
        public GameObject parent;
        bool dead = false;

        [SerializeField] Color colorFloaterDamage = Color.red;
        [SerializeField] Color colorFloaterHeal = Color.green;

        [SerializeField] public int maxHp = 10;
        [SerializeField] int hp = 10;

        public int Health
        {
            get => hp;
            set
            {
                hp = Mathf.Clamp(value, 0, maxHp);
                Refresh();
            }
        }

        FloatingText floater;

        public bool IsAlive => !dead;
        public bool IsFullHp => hp >= maxHp;

        public void Init() => Init(maxHp);

        public void Init(int max)
        {
            if (max <= 0)
            {
                Debug.LogError($"Wrong max health! ({max})");
                return;
            }

            if(worldHealthBar)
            {
                sizePerHp = worldHealthBar.localScale.x / max;
            }
            SetMax(max, false);
            Health = max;

            if (txtHealth && txtHealth.TryGetComponent<FloatingText>(out var floater))
            {
                this.floater = floater;
            }
        }

        public void SetVisible(bool value)
        {
            parent.SetActive(value);
        }

        public void SetMax(int value, bool refresh = true)
        {
            if (slider) slider.maxValue = value;
            maxHp = value;
            if (refresh) Refresh();
        }

        public void AddMax(int value)
        {
            SetMax(maxHp + value, false);
            Health += value;
        }

        public int Remove(int amount)
        {
            int old = Health;
            Health -= amount;
            var delta = old - Health; 
            if (floater) floater.CreateFromLocal($"-{delta}", 0.2f).SetColor(colorFloaterDamage);

            OnDamage?.Invoke();

            return delta;
        }

        public int Kill()
        {
            return Remove(Health);
        }

        public int Add(int amount)
        {
            int old = Health;
            Health += amount;
            int delta = Health - old;
            if (floater) floater.CreateFromLocal($"+{delta}", 0.2f).SetColor(colorFloaterHeal);

            if (delta > 0) OnHeal?.Invoke();

            return delta;
        }

        public void Refresh()
        {
            txtHealth?.SetText($"{hp}");
            if (slider) slider.value = hp;

            if (worldHealthBar)
            {
                var scale = worldHealthBar.localScale;
                scale.x = hp * sizePerHp;
                worldHealthBar.localScale = scale;
            }

            if (!dead && hp == 0)
            {
                dead = true;
                OnDeath?.Invoke();
            }
        }

        public static implicit operator int(HealthComponent d) => d.Health;

        // Only use for += 
        public static HealthComponent operator +(HealthComponent a, int value)
        {
            a.Health += value;
            return a;
        }

        // Only use for -= 
        public static HealthComponent operator -(HealthComponent a, int value)
        {
            a.Health -= value;
            return a;
        }
    }
}
