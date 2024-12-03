using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;
using TMPro;

public class UIHudGameOver : UIHudBase
{
    public static UIHudGameOver _current;
    public static UIHudGameOver current
    {
        get
        {
            if (!_current) _current = FindFirstObjectByType<UIHudGameOver>();
            return _current;
        }
    }

    [SerializeField] TextMeshProUGUI txtTitle;
    [SerializeField] TextMeshProUGUI txtDescription;

    [SerializeField] GameObject ornamentVictory;
    [SerializeField] GameObject ornamentDefeat;

    public void Show(bool victory, string description = null)
    {
        txtTitle.text = victory ? "Victory!" : "Defeat";
        ornamentVictory.SetActive(victory);
        ornamentDefeat.SetActive(!victory);
        if (string.IsNullOrWhiteSpace(description))
        {
            txtDescription.text = "";
        }
        else
        {
            txtDescription.text = description;
        }

        if (victory)
        {
            txtDescription.text = "You’ve conquered the Arcane Board demo! Wishlist the full game on Steam to continue your journey and face even greater challenges!";
        }

        Opened();
    }

    public void Close()
    {
        Closed();
        Game.current.LoadScene();
    }
}
