using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class Unit : MonoBehaviour, IDamagable
{
    [SerializeField] ValueBar health;
    [SerializeField] SpriteRenderer render;

    public static System.Action<Unit> OnKilled;

    public UnitData data { get; private set; }

    public void Init(UnitData data)
    {
        this.data = data;
        render.sprite = data.sprite;
        health.Init(data.hp);
        health.OnZero += HandleOutOfHealth;
    }

    void HandleOutOfHealth()
    {
        OnKilled?.Invoke(this);
        Destroy(gameObject);
    }

    public void SetFlip(bool value)
    {
        render.flipX = value;
    }

    public void RemoveHp(int value)
    {
        health.Remove(value);
    }

    public void AddHp(int value)
    {
        health.Add(value);
    }

    private IEnumerator AnimateScale()
    {
        float animationDuration = 0.25f;
        float currentTime = 0f;
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.5f; // Scale up to 150% of original size

        while (currentTime <= animationDuration)
        {
            float t = currentTime / animationDuration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            currentTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we reached the exact target scale
        transform.localScale = targetScale;

        // Wait for a short moment
        yield return new WaitForSeconds(0.1f); // Adjust this value for your needs

        // Scale back to original size
        currentTime = 0f;

        while (currentTime <= animationDuration)
        {
            float t = currentTime / animationDuration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            currentTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we reached the exact original scale
        transform.localScale = originalScale;
    }

    public void SpecialTestAction()
    {
        StartCoroutine(AnimateScale());
    }
}