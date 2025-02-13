using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FancyToolkit;
using GridBoard;
using FancyTweens;
using System.Text;
using TMPro;

public class UISkillButton : MonoBehaviour, IHasInfo, IActionParent, IIconProvider, IInfoTextProvider, IHintContainer, IHoverInfoTarget
{
    public static event System.Action<UISkillButton> OnUsed;
    public static event System.Action<UISkillButton> OnAviableToUse;

    [SerializeField] Button button;
    [SerializeField] UIMultiImage imgIcon;
    [SerializeField] Image imgBg;
    [SerializeField] Image imgCdFill;
    [SerializeField] UIShaderComponent shaderCtrl;
    [SerializeField] AudioClip sound;

    [SerializeField] Slider sliderProgress;
    [SerializeField] TextMeshProUGUI txtProgress;

    [SerializeField] UISliderTweener progressTweener;

    public SkillData data { get; private set; }

    SkillActionContainer actionContainer;

    public Board board { get; private set; }
    int power;
    public int Power
    {
        get => Mathf.Max(power, 0);
        set
        {
            if (power == value) return;
            power = value;
            //RefreshNumber();
        }
    }

    public string VfxId => data.VfxId;

    public bool HasManualUse => !(actionContainer.clickCondition?.AutoActivate??true);

    public float CooldownFill
    {
        get => imgCdFill.fillAmount;
        set => imgCdFill.fillAmount = value;
    }

    public void SetProgress(string caption, int value, int maxValue = 0)
    {
        txtProgress.text = caption;
        //if (maxValue > 0) sliderProgress.maxValue = maxValue;
        //sliderProgress.value = value;
        progressTweener.SetValue(value, maxValue);
    }

    public void Init(SkillData data, Board board)
    {
        this.data = data;
        this.board = board;
        this.power = data.power;
        imgIcon.sprite = data.sprite;

        button.interactable = false;

        actionContainer = new SkillActionContainer(data, this);
        actionContainer.clickCondition?.Init(this);

        if (board)
        {
            button.interactable = (actionContainer.clickCondition != null && actionContainer.onClick != null);
            foreach (var item in actionContainer.AllActions())
            {
                item.SetBoard(board);
            }
        }
    }

    public IEnumerator CombatStarted()
    {
        if (actionContainer.onStartCombat != null)
        {
            yield return actionContainer.onStartCombat.Run();
        }
    }

    public void OnClick()
    {
        if (actionContainer.clickCondition == null || actionContainer.onClick == null) return;
        if (!actionContainer.clickCondition.CanUse)
        {
            MainUI.current.ShowMessage(actionContainer.clickCondition.GetErrorUnusable());
            return;
        }

        actionContainer.clickCondition.OnClicked();
        StartCoroutine(actionContainer.onClick.Run());
        OnUsed?.Invoke(this);
        sound?.PlayNow();
    }

    public IEnumerator EndTurn()
    {
        if (actionContainer.onEndTurn != null)
        {
            yield return actionContainer.onEndTurn.Run();
        }
    }

    private void OnDestroy()
    {
        foreach (var item in actionContainer.AllActions())
        {
            item.SetBoard(null);
        }
        actionContainer.clickCondition.Destroy();
    }

    void SetUsableVisual(bool value)
    {
        if (value)
        {
            OnAviableToUse?.Invoke(this);
        }
        shaderCtrl.SetGrayscale(!value);
    }

    public void RefreshUse()
    {
        if (actionContainer.clickCondition == null) return;
        bool canUse = actionContainer.clickCondition.CanUse;

        SetUsableVisual(canUse);
    }

    public Sprite GetIcon() => data.GetIcon();
    public List<string> GetTooltips() => data.GetTooltips();
    public bool ShouldShowInfo() => data.ShouldShowInfo();
    public string GetTitle() => data.GetTitle();
    public string GetDescription() => data.GetDescription(actionContainer);

    public Component AsComponent() => this;

    public List<string> GetTags() => data.GetTags();
    public IHasInfo GetExtraInfo() => data.GetExtraInfo(actionContainer);

    public bool ShouldShowHoverInfo() => data.ShouldShowHoverInfo();

    public List<IHintProvider> GetHintProviders()
    {
        return actionContainer?.GetHintProviders();
    }

    public string GetInfoText(int size)
    {
        var sb = new StringBuilder();

        sb.AppendLine(data.name
            .Center()
            .Bold());

        sb.AppendLine();
        sb.AppendLine(GetDescription());

        return sb.ToString();
    }

#if UNITY_EDITOR
    void DebugAddCharge(int value)
    {
        actionContainer?.clickCondition?.DebugAddCharge(value);
    }

    [ContextMenu("Add 1 Charge")]
    public void Add1Charge()
    {
        DebugAddCharge(1);
    }
    [ContextMenu("Add 5 Charge")]
    public void Add5Charge()
    {
        DebugAddCharge(5);
    }
    [ContextMenu("Add 20 Charge")]
    public void Add20Charge()
    {
        DebugAddCharge(20);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            actionContainer?.clickCondition?.DebugAddCharge(1);
        }
    }

#endif
}