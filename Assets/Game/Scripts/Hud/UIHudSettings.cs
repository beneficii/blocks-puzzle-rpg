using RogueLikeMap;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using FancyToolkit;

public class UIHudSettings : UIHudBase
{
    public static UIHudSettings _current;
    public static UIHudSettings current
    {
        get
        {
            if (!_current) _current = FindFirstObjectByType<UIHudSettings>();
            return _current;
        }
    }

    [SerializeField] Slider sliderSound;
    [SerializeField] Slider sliderMusic;

    float cachedSound;
    float cachedMusic;

    private void Start()
    {
        sliderSound.onValueChanged.AddListener(OnSoundSlider);
        sliderMusic.onValueChanged.AddListener(OnMusicSlider);
    }

    public void OnSoundSlider(float value)
    {
        AudioCtrl.current.VolumeSound = value;
    }

    public void OnMusicSlider(float value)
    {
        AudioCtrl.current.VolumeMusic = value;
    }

    public void Show()
    {
        Opened();

        cachedSound = sliderSound.value = AudioCtrl.current.VolumeSound;
        cachedMusic = sliderMusic.value = AudioCtrl.current.VolumeMusic;
    }

    public void Toggle()
    {
        if (IsOpen)
        {
            Close();
        }
        else
        {
            Show();
        }
    }

    public void Close()
    {
        Closed();
    }

    public void Cancel()
    {
        AudioCtrl.current.VolumeSound = cachedSound;
        AudioCtrl.current.VolumeMusic = cachedMusic;
        Close();
    }

    public void BtnMainMenu()
    {
        Game.current.LoadMainMenu();
    }
}