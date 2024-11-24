using FancyToolkit;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHudGenericInfo : UIHudBase
{
    public static UIHudGenericInfo _current;
    public static UIHudGenericInfo current
    {
        get
        {
            if (!_current) _current = FindFirstObjectByType<UIHudGenericInfo>();
            return _current;
        }
    }

    [SerializeField] TextMeshProUGUI txtTitle;
    [SerializeField] TextMeshProUGUI txtDescription;

    float cachedSound;
    float cachedMusic;

    public void OnSoundSlider(float value)
    {
        AudioCtrl.current.VolumeSound = value;
    }

    public void OnMusicSlider(float value)
    {
        AudioCtrl.current.VolumeMusic = value;
    }

    public void Show(string title, string description)
    {
        Opened();
        txtTitle.text = title;
        txtDescription.text = description;
    }

    public void Show()
    {
        Opened();
    }

    public void Close()
    {
        Closed();
    }
}