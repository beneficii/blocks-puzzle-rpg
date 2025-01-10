using System.Collections;
using System.Collections.Generic;
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

    public override void PostInitSingle(SkillData data)
    {
        if (data.idVisual != null)
        {
            var visual = Resources.Load<Sprite>($"SkillIcons/{data.idVisual}");
            if (!visual)
            {
                Debug.LogError($"No sprite for {data.id} ({data.idVisual})");
                return;
            }

            data.sprite = visual;
        }
    }

    public void DebugAll()
    {
        var sb = new System.Text.StringBuilder();
        foreach (var data in GetAll())
        {
            sb.AppendLine($"{data.name} : {data.GetDescription()}");
        }
        Debug.Log(sb.ToString());
    }
}
