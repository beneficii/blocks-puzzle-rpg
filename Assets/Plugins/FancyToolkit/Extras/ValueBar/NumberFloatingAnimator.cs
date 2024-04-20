using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FancyToolkit
{
    public class NumberFloatingAnimator : MonoBehaviour
    {
        [SerializeField] Color clrPositive = Color.green;
        [SerializeField] Color clrNegative = Color.red;

        [SerializeField] FloatingText prefabFloater;

        public void Spawn(int value)
        {
            Color color;
            string msg;

            if (value > 0)
            {
                color = clrPositive;
                msg = $"+{value}";
            }
            else
            {
                color = clrNegative;
                msg = value.ToString();

            }

            prefabFloater.Create(msg, transform.position, transform)
                .SetColor(color);
        }
    }
}
