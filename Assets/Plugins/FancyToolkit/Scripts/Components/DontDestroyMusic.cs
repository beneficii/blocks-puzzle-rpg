using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FancyToolkit
{
    public class DontDestroyMusic : MonoBehaviour
    {
        static DontDestroyMusic current;
        private void Awake()
        {
            if (current == null)
            {
                current = this;
                DontDestroyOnLoad(gameObject);
                return;
            }

            Destroy(gameObject);
        }
    }
}