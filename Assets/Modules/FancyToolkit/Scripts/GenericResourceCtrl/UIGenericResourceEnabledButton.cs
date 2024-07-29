using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FancyToolkit
{
    public class UIGenericResourceEnabledButton<TRes> : MonoBehaviour
    where TRes : struct, System.Enum
    {
        [SerializeField] TRes resource;
        [SerializeField] int requiredValue;

        Button btn;

        private void Awake()
        {
            btn = GetComponent<Button>();
        }

        protected virtual void OnEnable()
        {
            ResCtrl<TRes>.OnChanged += HandleResourceChange;
            HandleResourceChange(resource, ResCtrl<TRes>.current.Get(resource));
        }

        protected virtual void OnDisable()
        {
            ResCtrl<TRes>.OnChanged -= HandleResourceChange;
        }

        void HandleResourceChange(TRes type, int value)
        {
            if (!type.Equals(resource)) return;

            btn.interactable = value >= requiredValue;
        }
    }

}
