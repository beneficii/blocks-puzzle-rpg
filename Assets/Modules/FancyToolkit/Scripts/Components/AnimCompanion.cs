using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FancyToolkit
{
    public class AnimCompanion : MonoBehaviour
    {
        [SerializeField] float lifetime = 2f;
        System.Action onTrigger;

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }

        public void SetTriggerAction(System.Action action)
        {
            onTrigger = action;
        }

        public void TriggerPoint()
        {
            onTrigger?.Invoke();
        }
    }
}
