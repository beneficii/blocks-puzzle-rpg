using System.Collections;
using UnityEngine;
using TMPro;
using FancyToolkit;
using UnityEngine.UI;

public class TutorialCtrl : MonoBehaviour
{
    static TutorialCtrl _current;
    public static TutorialCtrl current
    {
        get
        {
            if (_current == null)
            {
                _current = FindAnyObjectByType<TutorialCtrl>();
            }

            return _current;
        }
    }

    [SerializeField] Panel panelBoard;

    public void ShowText(TutorialPanel panel, string message, bool isSpeech = false)
    {
        panelBoard.Show(message, isSpeech);
    }

    public void HideAll()
    {
        panelBoard.Hide();
    }


    [System.Serializable]
    public class Panel
    {
        public RectTransform parent;
        public TMP_Text txt;
        public GameObject speechCorner;

        public void Show(string message, bool isSpeech = false)
        {
            parent.gameObject.SetActive(true);
            txt.text = message;
            speechCorner.SetActive(isSpeech);
            LayoutRebuilder.ForceRebuildLayoutImmediate(parent);
        }

        public void Hide()
        {
            parent.gameObject.SetActive(false);
        }
    }
}


public enum TutorialPanel
{
    Board,
}

