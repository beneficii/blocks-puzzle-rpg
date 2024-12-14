using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FancyToolkit;
using UnityEngine.Events;

public class MenuCtrl : MonoBehaviour
{
    [SerializeField] UIGenericButton templateButton;
    List<GameObject> allPanels = new();


    public string steamAppUrl = "https://store.steampowered.com/app/3126090";

    public void OpenWishlistPage()
    {
        Application.OpenURL(steamAppUrl);
    }

    void AddButton(string caption, UnityAction action)
    {
        var button = UIUtils.CreateFromTemplate(templateButton);
        button.SetText(caption);
        button.AddOnClick(action);
    }

    private void Start()
    {
        if (GameState.HasSave()) AddButton("Continue", BtnContinue);
        AddButton("New Game", BtnPlay);
        AddButton("Credits", BtnCredits);
        AddButton("Settings", BtnSettings);
        AddButton("Exit", BtnExit);
    }

    public void ClosePanels()
    {
        if (UIHudGenericInfo.current.IsOpen)
        {
            UIHudGenericInfo.current.Close();
        }
        if (UIHudSettings.current.IsOpen)
        {
            UIHudSettings.current.Close();
        }
    }

    void BtnCredits()
    {
        ClosePanels();
        UIHudGenericInfo.current.Show();
    }

    void BtnSettings()
    {
        ClosePanels();
        UIHudSettings.current.Show();
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
