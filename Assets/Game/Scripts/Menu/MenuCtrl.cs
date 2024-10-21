using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuCtrl : MonoBehaviour
{
    static string cachedTitle = "Arcane Board";
    static string cachedDescription = "In a dark land ravaged by monsters, a hopeful young mage discovers an ancient magic board. To harness its power, you must align its special tiles to fill a full line. Embarking on a journey, the mage seeks to restore light to the land.";
    static int cachedSpriteTitle = 0;


    [SerializeField] Animator arenaAnimator;
    [SerializeField] GameObject canvas;

    [SerializeField] TextMeshProUGUI txtTitle;
    [SerializeField] TextMeshProUGUI txtDescription;
    [SerializeField] Image imgTitle;
    [SerializeField] List<Sprite> spritesTitle;


    private void Start()
    {
        txtTitle.text = cachedTitle;
        txtDescription.text = cachedDescription;
        imgTitle.sprite = cachedSpriteTitle < spritesTitle.Count ? spritesTitle[cachedSpriteTitle] : spritesTitle[0];
    }

    public void BtnPlay()
    {
        BtnContinue();
    }

    public void BtnContinue()
    {
        canvas.SetActive(false);
        arenaAnimator.SetTrigger("Play");
        StartCoroutine(LoadScene());
    }

    public static void Load(string title, string description, int spriteIdx)
    {
        cachedTitle = title;
        cachedDescription = description;
        cachedSpriteTitle = spriteIdx;

        SceneManager.LoadScene("MainMenu");
    }

    public static void Load(GameOverType type)
    {
        Load(GetTitle(type), GetDescription(type), GetTitleSpriteIdx(type));
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

    public static int GetTitleSpriteIdx(GameOverType type)
    {
        return type switch
        {
            GameOverType.Victory => 1,
            GameOverType.Defeat => 2,
            _ => 0,
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


    IEnumerator LoadScene()
    {
        yield return null;

        //Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("SampleScene");
        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;
        //When the load is still in progress, output the Text and progress bar
        yield return new WaitForSeconds(1f);

        while (!asyncOperation.isDone)
        {
            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
                

            }

            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("SampleScene"));
        SceneManager.UnloadSceneAsync("MainMenu");
    }
}


public enum GameOverType
{
    None,
    Victory,
    Defeat,
}