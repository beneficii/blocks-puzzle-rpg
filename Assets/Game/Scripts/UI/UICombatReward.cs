using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using FancyToolkit;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using GridBoard;
using NUnit.Framework;

public partial class UICombatReward : MonoBehaviour, IHasNestedInfo
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
        data.Click(this);

        Remove();
        sound?.PlayWithRandomPitch();
    }

    public GenericBullet CreateBullet(string vfxId = null, Transform target = null)
    {
        return Game.current.MakeBullet(imgIcon.transform.position, vfxId)
                .SetSpleen(Random.Range(0, 1) == 1 ? Vector2.left : Vector2.right)
                .SetTarget(target)
                .SetSprite(imgIcon.sprite);
    }

    public IHasInfo GetInfo() => data.GetInfo();
}

public enum RewardType
{
    None,
    Gold,
    Tile,
}