using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using FancyToolkit;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using GridBoard;
using NUnit.Framework;

public partial class UICombatReward : MonoBehaviour
{
    public static event System.Action<UICombatReward> OnClicked;

    [SerializeField] Image imgIcon;
    [SerializeField] TextMeshProUGUI txtCaption;
    [SerializeField] AudioClip sound;


    [Header("Icons")]
    [SerializeField] Sprite iconGold;
    [SerializeField] Sprite iconTile;
    [SerializeField] Sprite iconSkill;

    Data data;

    public void Init(Data data)
    {
        this.data = data;
        data.InitUI(this);
    }

    public void Remove()
    {
        // ToDo: maybe some disapear animation
        Destroy(gameObject);
    }

    public void Click()
    {
        OnClicked?.Invoke(this);
        data.Click();

        Remove();
        sound?.PlayWithRandomPitch();
    }

    
}

public enum RewardType
{
    None,
    Gold,
    Tile,
}