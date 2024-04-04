using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

[DefaultExecutionOrder(-10)]
public class Game : MonoBehaviour
{
    public static Game current { get; private set; }

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
    }
}