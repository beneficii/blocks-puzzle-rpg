using System.Collections;
using System.Linq;
using UnityEngine;
using FancyToolkit;

public class SkillCtrl : GenericDataCtrl<SkillData>
{
    static SkillCtrl _current;
    public static SkillCtrl current
    {
        get
        {
            if (_current == null)
            {
                var cur = new SkillCtrl();
                _current = cur;
            }

            return _current;
        }
    }

    public override void PostInit()
    {
        var sprites = Resources.LoadAll<Sprite>("SkillIcons").ToDictionary(x => x.name);
        foreach (var item in GetAll())
        {
            item.sprite = sprites.Get(item.idVisual);
        }
    }
}
