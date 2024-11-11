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
    

    [Header("Icons")]
    [SerializeField] Sprite iconGold;
    [SerializeField] Sprite iconTile;

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
        data.Click();
        OnClicked?.Invoke(this);

        Remove();
    }

    
}

public enum RewardType
{
    None,
    Gold,
    Tile,
}