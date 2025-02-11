using System.Collections.Generic;
using FancyToolkit;
using TMPro;
using Unity.Services.Analytics;
using UnityEngine;

public class UIDIalogOption : MonoBehaviour, IHasNestedInfo, IHoverInfoContainer
{
    [SerializeField] TextMeshProUGUI txtMessage;
    [SerializeField] AudioClip sound;

    DialogData data;

    List<DialogAction> actions;

    public IHoverInfoTarget GetHoverInfoTarget()
    {
        foreach (DialogAction action in actions)
        {
            var info = action.GetHoverInfoTarget();
            if (info != null) return info;
        }

        return null;
    }

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
            var error = item.CheckErrors();

            if (!string.IsNullOrEmpty(error))
            {
                MainUI.current.ShowMessage(error);
                return;
            }
        }

        foreach (var item in actions)
        {
            item.Execute();
        }
        UIHudDialog.current.Close();

        sound?.PlayNow();
        Game.current.RecordEvent(new AnalyticsEvents.DialogSelected
        {
            answer = data.text,
        });
    }

}
