using FancyToolkit;
using TMPro;
using UnityEngine;

public class UIDIalogOption : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtMessage;

    DialogData data;

    public void Init(DialogData data)
    {
        this.data = data;
        txtMessage.text = data.text;
        // ToDo: maybe give action hint
    }

    public void Select()
    {
        //ToDo: execute action
        foreach (var item in data.actions)
        {
            var action = Factory<DialogAction>.Create(item);
            if (action == null) continue;

            action.Execute();
        }
        UIHudDialog.current.Close();
    }

}
