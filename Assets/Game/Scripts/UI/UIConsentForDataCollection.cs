using System.Collections;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class UIConsentForDataCollection : MonoBehaviour
{
    [SerializeField] GameObject panelParent;
    const string ConsentKey = "ConsentKey";

    IEnumerator Start()
    {
        panelParent.SetActive(false);
        yield return new WaitUntil(()=>Game.current.initDone);
        if (PlayerPrefs.HasKey(ConsentKey))
        {
            bool consentGiven = PlayerPrefs.GetInt(ConsentKey) == 1;
            if (consentGiven)
            {
                StartCollection();
            }
        }
        else
        {
            panelParent.SetActive(true);
        }
    }

    public void Yes()
    {
        HandleConsentAnswer(true);
        panelParent.SetActive(false);
    }

    public void No()
    {
        HandleConsentAnswer(false);
        panelParent.SetActive(false);
    }

    void StartCollection()
    {
        AnalyticsService.Instance.StartDataCollection();
    }

    public void HandleConsentAnswer(bool consentGiven)
    {
        PlayerPrefs.SetInt(ConsentKey, consentGiven ? 1 : 0);
        PlayerPrefs.Save();

        if (consentGiven)
        {
            StartCollection();
        }
    }
}