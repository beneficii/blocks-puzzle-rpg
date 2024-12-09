using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TutorialItemBase : MonoBehaviour
{
    // static stuff
    const string prefsKey = "tutorial_v0.1";

    static int currentStepIdx = -1;
    static int CurrentStepIdx
    {
        get
        {
            if (currentStepIdx < 0)
            {
                currentStepIdx = PlayerPrefs.GetInt(prefsKey, 0);
            }

            return currentStepIdx;
        }

        set
        {
            currentStepIdx = value;
            PlayerPrefs.SetInt(prefsKey, value);
        }
    }
    static TutorialStep CurrentStep => (TutorialStep)CurrentStepIdx;

    // local stuff
    [SerializeField] GameObject panel;

    bool isVisible;

    protected abstract TutorialStep Step { get; }

    void SetStepVisible(TutorialStep step, bool value)
    {
        foreach (var item in FindObjectsByType<TutorialItemBase>(FindObjectsSortMode.None)
            .Where(x => x.Step == step))
        {
            item.SetVisible(value);
        }
    }

    protected void FinishStep()
    {
        if (Step != CurrentStep) return;

        CurrentStepIdx++;

        SetStepVisible(Step, false);
        SetStepVisible(CurrentStep, true);
    }

    private void OnEnable()
    {
        SetVisible(Step == CurrentStep);
    }

    private void OnDisable()
    {
        SetVisible(false);
    }

    void SetVisible(bool value)
    {
        panel.SetActive(value);
        if (isVisible == value) return;
        isVisible = value;

        if (value)
        {
            OnEnter();
        }
        else
        {
            OnExit();
        }
    }

    protected abstract void OnEnter();
    protected abstract void OnExit();
}

public enum TutorialStep
{
    PlaceShape,
    FillLine,
    ClaimRewards,
    FillSkill,
    UseSkill,
    None,
}