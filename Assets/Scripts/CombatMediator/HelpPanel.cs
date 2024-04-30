using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HelpPanel : MonoBehaviour
{
    static HelpPanel _current;
    public static HelpPanel current
    {
        get
        {
            if (!_current)
            {
                _current = FindFirstObjectByType<HelpPanel>();
            }

            return _current;
        }
    }

    [SerializeField] GameObject panel;
    [SerializeField] GameObject btnHint;
    [SerializeField] GameObject btnRegen;
    [SerializeField] GameObject btnClose;
    [SerializeField] TextMeshProUGUI txtTile;


    public void ShowUser()
    {
        btnClose.SetActive(true);
        panel.SetActive(true);
        btnRegen.SetActive(false);
        if (ShapePanel.current.HasHints())
        {
            txtTile.text = "How can I help you?";
            btnHint.SetActive(true);
        }
        else
        {
            txtTile.text = "Hints aviable at the start of the turn";
            btnHint.SetActive(false);
        }
    }

    public void ShowStuck()
    {
        panel.SetActive(true);
        txtTile.text = "No spots for your shapes";
        btnHint.SetActive(false);
        btnRegen.SetActive(true);
        btnClose.SetActive(false);
    }


    public void Close()
    {
        panel.SetActive(false);
    }

    public void BtnShowHint()
    {
        Close();
        ShapePanel.current.BtnShowHint();
        CombatArena.current.player.RemoveHp(2);
    }

    public void BtnGenerateNewShapes()
    {
        Close();
        ShapePanel.current.GenerateNew(false);
        CombatArena.current.player.RemoveHp(10);
    }

    public void BtnLoadSave()
    {
        Close();
        BtSave.Load();
    }

    public void BtnAutoplay()
    {
        Close();
        ShapePanel.current.BtnAutoplay();
    }

    public void BtnQuit()
    {
        Close();
        MenuCtrl.Load(GameOverType.None);
    }
}