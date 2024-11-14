using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FancyToolkit;
using UnityEngine.Events;

public class MenuCtrl : MonoBehaviour
{
    [SerializeField] Button templateButton;
    [SerializeField] GameObject panelCredits;
    [SerializeField] GameObject panelSettings;

    List<GameObject> allPanels = new();

    void AddButton(string caption, UnityAction action)
    {
        var button = UIUtils.CreateFromTemplate(templateButton);
        button.GetComponentInChildren<TextMeshProUGUI>().text = caption;
        button.onClick.AddListener(action);
    }

    private void Start()
    {
        allPanels.Add(panelCredits);
        allPanels.Add(panelSettings);

        if (GameState.HasSave()) AddButton("Continue", BtnContinue);
        AddButton("New Game", BtnPlay);
        AddButton("Credits", BtnCredits);
        AddButton("Settings", BtnSettings);
        AddButton("Exit", BtnExit);
    }

    void ClosePanels()
    {
        foreach (var item in allPanels)
        {
            item.gameObject.SetActive(false);
        }
    }

    void BtnCredits()
    {
        ClosePanels();
        panelCredits.SetActive(true);
    }

    void BtnSettings()
    {
        ClosePanels();
        panelSettings.SetActive(true);
    }

    void BtnPlay()
    {
        Game.current.NewGame();
        Game.current.LoadScene();
    }

    void BtnContinue()
    {
        Game.current.Continue();
        Game.current.LoadScene();
    }

    void BtnExit()
    {
        Application.Quit();
    }
}
