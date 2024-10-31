using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;
using GridBoard;

public class UIHudCombat : UIHudBase
{
    public static UIHudCombat _current;
    public static UIHudCombat current
    {
        get
        {
            if (!_current)
            {
                _current = FindFirstObjectByType<UIHudCombat>();
            }

            return _current;
        }
    }

    [SerializeField] UISkillButton templateButton;

    public List<UISkillButton> skillButtons { get; private set; } = new();

    void Clear()
    {
        foreach (var button in skillButtons)
        {
            Destroy(button.gameObject);
        }
        skillButtons.Clear();
    }


    public IEnumerator InitSkills(Board board)
    {
        Clear();
        foreach (var data in Game.current.GetSkills())
        {
            var instance = UIUtils.CreateFromTemplate(templateButton);
            instance.Init(data, board);
            skillButtons.Add(instance);
        }

        foreach (var item in skillButtons)
        {
            yield return item.CombatStarted();
        }
    }

    public void Show()
    {
        Opened();
    }
}