using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace FancyToolkit {
    public class UIUpgradeScreen : MonoBehaviour
    {
        public static UIUpgradeScreen Instance { get; private set; }

        [SerializeField] GameObject mainPanel;
        [SerializeField] Transform parentForItems;
        [SerializeField] UIUpgradeItem prefab;

        List<UIUpgradeItem> instantiatedItems = new();

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Assert.IsNotNull(prefab.GetComponent<IUIUpgradeChoise>());
        }

        public List<T> Open<T>(int itemCount) where T : IUIUpgradeChoise
        {
            Clear();
            mainPanel.SetActive(true);

            var items = new List<T>();

            for (int i = 0; i < itemCount; i++)
            {
                var item = Instantiate(prefab, parentForItems);
                item.onSelect += HandleItemSelected;
                items.Add(item.GetComponent<T>());
                instantiatedItems.Add(item);
            }

            return items;
        }

        void Clear()
        {
            foreach (var item in instantiatedItems)
            {
                Destroy(item.gameObject);
            }

            instantiatedItems.Clear();
        }

        public void Close()
        {
            mainPanel.SetActive(false);
            Clear();
        }

        void HandleItemSelected(UIUpgradeItem item)
        {
            var choise = item.GetComponent<IUIUpgradeChoise>();

            if (!choise.Select())
            {
                Debug.Log("ToDo: Error message, can't select choise");
                return;
            }

            Close();
        }
    }


    public interface IUIUpgradeChoise
    {
        bool Select();
    }
}
