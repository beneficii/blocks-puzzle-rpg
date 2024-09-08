using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace FancyToolkit
{
    public class IconIntRef : MonoBehaviour
    {
        [SerializeField] SpriteRenderer icon;
        [SerializeField] IntSpritePair levelInfo;

        IntReference source;

        int cachedLevel = -1;

        public void Init(IntReference source)
        {
            this.source = source;
            source.OnChanged += HandleChanged;
            Refresh();
        }

        private void OnDestroy()
        {
            if (source != null) source.OnChanged -= HandleChanged;
        }

        void Refresh()
        {
            int level = source.Value >= levelInfo.value ? 1 : 0;

            if (level == cachedLevel) return;
            cachedLevel = level;

            icon.sprite = level == 0 ? null : levelInfo.sprite;
        }

        void HandleChanged(int val, int dif)
        {
            Refresh();
        }

        [System.Serializable]
        public class IntSpritePair
        {
            public int value;
            public Sprite sprite;
        }
    }

}
