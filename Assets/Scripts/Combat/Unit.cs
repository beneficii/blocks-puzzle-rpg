using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class Unit : MonoBehaviour, IDamagable
{
    [SerializeField] ValueBar health;

    private void Awake()
    {
        health.Init(100);
        health.OnZero += HandleOutOfHealth;
    }

    void Start()
    {

    }

    void HandleOutOfHealth()
    {
        Destroy(gameObject);
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.W))
        {
            health.Add(5);
        } else if (Input.GetKeyDown(KeyCode.S))
        {
            health.Remove(5);
        }
#endif
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