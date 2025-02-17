using System.Collections;
using UnityEngine;

public class UIWishlistBeforeExit : MonoBehaviour
{
    [SerializeField] GameObject panelParent;

    void ShowChoice()
    {
        panelParent.SetActive(true);
    }

    public static void HandleQuit()
    {
        var handler = FindAnyObjectByType<UIWishlistBeforeExit>();
        if (handler)
        {
            handler.ShowChoice();
            return;
        }

        Application.Quit();
    }

    public void Yes()
    {
        MenuCtrl.WishlistPage();
        Application.Quit();
    }

    public void No()
    {
        Application.Quit();
    }
}