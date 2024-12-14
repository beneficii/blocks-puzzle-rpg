using System.Collections;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class UIConsentForDataCollection : MonoBehaviour
{
    const string ConsentKey = "ConsentKey";

    private void Start()
    {
        if (PlayerPrefs.HasKey(ConsentKey))
        {
            bool consentGiven = PlayerPrefs.GetInt(ConsentKey) == 1;
            gameObject.SetActive(false);
            if (consentGiven)
            {
                StartCollection();
            }
        }
    }

    public void Yes()
    {
        HandleConsentAnswer(true);
        gameObject.SetActive(false);
    }

    public void No()
    {
        HandleConsentAnswer(false);
        gameObject.SetActive(false);
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