using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FancyToolkit;
using GridBoard;
using FancyTweens;
using System.Text;

public class UISkillButton : MonoBehaviour, IHasInfo, IActionParent
{
    [SerializeField] Button button;
    [SerializeField] UIMultiImage imgIcon;
    [SerializeField] Image imgBg;
    [SerializeField] Image imgCdFill;
    [SerializeField] UIShaderComponent shaderCtrl;

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

    public float CooldownFill
    {
        get => imgCdFill.fillAmount;
        set => imgCdFill.fillAmount = value;
    }

    public int Damage { get => Power; set => Power = value; }
    public int Defense { get => Power; set => Power = value; }

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
        shaderCtrl.SetGrayscale(!value);
        //imgIcon.SetAlpha(value ? 1 : 0.2f);
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
}