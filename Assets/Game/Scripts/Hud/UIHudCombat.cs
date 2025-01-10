using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;
using GridBoard;

public class UIHudCombat : MonoBehaviour
{

    [SerializeField] UISkillButton templateButton;
    [SerializeField] GameObject content;

    public List<UISkillButton> skillButtons { get; private set; } = new();
    public bool IsOpen { get; private set; }

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
        IsOpen = true;
        content.SetActive(true);
    }


    public void Close()
    {
        IsOpen = false;
        content.SetActive(false);
    }
}