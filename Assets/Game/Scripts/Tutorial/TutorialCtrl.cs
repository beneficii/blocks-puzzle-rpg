using System.Collections;
using UnityEngine;
using TMPro;

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

    public void ShowText(TutorialPanel panel, string message)
    {
        panelBoard.Show(message);
    }

    public void HideAll()
    {
        panelBoard.Hide();
    }


    [System.Serializable]
    public class Panel
    {
        public GameObject parent;
        public TMP_Text txt;

        public void Show(string message)
        {
            parent.SetActive(true);
            txt.text = message;
        }

        public void Hide()
        {
            parent.SetActive(false);
        }
    }
}


public enum TutorialPanel
{
    Board,
}

