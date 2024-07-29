using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FancyToolkit
{
    public class DestroyMe : MonoBehaviour
    {
        public float timeout = 2f;

        void Start()
        {
            Destroy(gameObject, timeout);
        }
    }
}
