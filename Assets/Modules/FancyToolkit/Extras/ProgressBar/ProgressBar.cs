using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace FancyToolkit
{
    public class ProgressBar : MonoBehaviour
    {
        const float targetProgress = 1000f;

        public event System.Action OnFinished;

        //[SerializeField] SpriteRenderer backgroundSprite;
        [SerializeField] SpriteRenderer foregroundSprite;

        bool isActive = false;
        float currentProgress = 0f;
        float fillSpeed = 0f;

        public bool IsActive => isActive;

        private void UpdateProgressBar(float progress)
        {
            float normalizedProgress = Mathf.Clamp01(progress / targetProgress);
            foregroundSprite.transform.localScale = new Vector3(normalizedProgress, 1f, 1f);
        }

        private void Start()
        {
            if (!IsActive) gameObject.SetActive(false);
        }

        void Update()
        {
            if (!IsActive) return;

            currentProgress += fillSpeed * Time.deltaTime;

            UpdateProgressBar(currentProgress);

            if (currentProgress >= targetProgress)
            {
                isActive = false;
                OnFinished?.Invoke();
                gameObject.SetActive(false);
            }
        }

        public void StartProgress(float duration)
        {
            currentProgress = 0f;
            fillSpeed = targetProgress / duration;
            isActive = true;
            gameObject.SetActive(true);
        }
    }
}
