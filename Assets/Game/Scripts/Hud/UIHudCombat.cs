using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;
using GridBoard;

public class UIHudCombat : MonoBehaviour
{

    [SerializeField] UISkillButton templateSkill;
    [SerializeField] UIGlyph templateGlyph;
    [SerializeField] GameObject content;

    public List<UISkillButton> skillButtons { get; private set; } = new();
    public List<UIGlyph> glyphs { get; private set; } = new();
    public bool IsOpen { get; private set; }


    void ClearSkills()
    {
        foreach (var button in skillButtons)
        {
            Destroy(button.gameObject);
        }
        skillButtons.Clear();
    }

    void ClearGlyphs()
    {
        foreach (var item in glyphs)
        {
            Destroy(item.gameObject);
        }
        glyphs.Clear();
    }

    public IEnumerator InitSkills(Board board)
    {
        ClearSkills();
        foreach (var data in Game.current.GetSkills())
        {
            var instance = UIUtils.CreateFromTemplate(templateSkill);
            instance.Init(data, board);
            skillButtons.Add(instance);
        }

        foreach (var item in skillButtons)
        {
            yield return item.CombatStarted();
        }
    }

    public IEnumerator InitGlyphs(Board board)
    {
        ClearGlyphs();
        foreach (var data in Game.current.GetGlyphs())
        {
            var instance = UIUtils.CreateFromTemplate(templateGlyph);
            instance.Init(data, board);
            glyphs.Add(instance);
        }

        foreach (var item in glyphs)
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