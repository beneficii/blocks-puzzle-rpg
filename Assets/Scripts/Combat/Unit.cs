using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class Unit : MonoBehaviour
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
}