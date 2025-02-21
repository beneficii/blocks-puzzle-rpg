using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FancyToolkit
{
    public class ScreenSizeCtrl : MonoBehaviour
    {
        const string fullscreenPrefKey = "ScreenSizeCtrl_Fullscreen";
        public static ScreenSizeCtrl current { get; private set; }

        public bool GetFullscreenState() => PlayerPrefs.GetInt(fullscreenPrefKey, 1) == 1;

        void Awake()
        {
            if (current)
            {
                Destroy(gameObject);
                return;
            }

            current = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            Screen.fullScreen = GetFullscreenState();
        }

        public void OnToggleValueChanged(bool isFullscreen)
        {
            if (isFullscreen)
            {
                Resolution nativeRes = Screen.currentResolution;
                Screen.SetResolution(nativeRes.width, nativeRes.height, FullScreenMode.ExclusiveFullScreen, nativeRes.refreshRateRatio);
                //Screen.SetResolution()
            }
            else
            {
                Screen.fullScreen = false;
            }
            PlayerPrefs.SetInt(fullscreenPrefKey, isFullscreen ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}