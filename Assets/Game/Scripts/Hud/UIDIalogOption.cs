using System.Collections.Generic;
using FancyToolkit;
using TMPro;
using Unity.Services.Analytics;
using UnityEngine;

public class UIDIalogOption : MonoBehaviour, IHasNestedInfo
{
    [SerializeField] TextMeshProUGUI txtMessage;

    DialogData data;

    List<DialogAction> actions;

    public IHasInfo GetInfo()
    {
        foreach (DialogAction action in actions)
        {
            if (action.GetInfo() != null) return action.GetInfo();
        }

        return null;
    }

    public void Init(DialogData data)
    {
        this.data = data;
        string message = data.text;

        actions = new();
        var lines = new List<string>();
        foreach (var item in data.actions)
        {
            var action = Factory<DialogAction>.Create(item);
            if (action == null) continue;

            var descr = action.GetDescription();
            if (!string.IsNullOrEmpty(descr))
            {
                lines.Add(descr);
            }
            actions.Add(action);
        }

        if (lines.Count > 0)
        {
            message += $" [{string.Join(", ", lines)}]";
        }
        txtMessage.text = message;
    }

    public void Select()
    {
        foreach (var item in actions)
        {
            item.Execute();
        }
        UIHudDialog.current.Close();

        AnalyticsService.Instance.RecordEvent(new AnalyticsEvents.DialogSelected
        {
            answer = data.text,
            userLevel = ResCtrl<ResourceType>.current.Get(ResourceType.Level),
            seed = Game.current.GetStageSeed(),
            leveId = Game.current.GetStageNode(),
        });
    }

}
