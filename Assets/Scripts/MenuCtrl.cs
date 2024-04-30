using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuCtrl : MonoBehaviour
{
    static string cachedTitle = "Arcane Board";
    static string cachedDescription = "In a dark land ravaged by monsters, a hopeful young mage discovers an ancient magic board. To harness its power, you must align its special tiles to fill a full line. Embarking on a journey, the mage seeks to restore light to the land.";


    [SerializeField] Animator arenaAnimator;
    [SerializeField] GameObject canvas;

    [SerializeField] TextMeshProUGUI txtTitle;
    [SerializeField] TextMeshProUGUI txtDescription;


    private void Start()
    {
        txtTitle.text = cachedTitle;
        txtDescription.text = cachedDescription;
    }

    public void BtnPlay()
    {
        canvas.SetActive(false);
        arenaAnimator.SetTrigger("play");
        StartCoroutine(RoutineStartLevel());
    }

    IEnumerator RoutineStartLevel()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("SampleScene");
    }

    public static void Load(string title, string description)
    {
        cachedTitle = title;
        cachedDescription = description;

        SceneManager.LoadScene("MainMenu");
    }

    public static void Load(GameOverType type)
    {
        Load(GetTitle(type), GetDescription(type));
    }

    public static string GetTitle(GameOverType type)
    {
        return type switch
        {
            GameOverType.Victory => "Victory!",
            GameOverType.Defeat => "Defeat!",
            _ => "Arcane Board",
        };
    }

    public static string GetDescription(GameOverType type)
    {
        return type switch
        {
            GameOverType.Victory => "After the clash, the young mage emerges victorious. Though monsters still lurk, a glimmer of hope now shines. It's a small win, but a promising start in the fight against darkness.",
            GameOverType.Defeat => "Despite his hardest efforts, the young mage retreats, leaving a land in shadow. The hope vains, but has not perished yet.",
            _ => "In a dark land ravaged by monsters, a hopeful young mage discovers an ancient magic board. To harness its power, you must align its special tiles to fill a full line. Embarking on a journey, the mage seeks to restore light to the land.",
        };
    }

    
}


public enum GameOverType
{
    None,
    Victory,
    Defeat,
}