using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LoadingCtrl : MonoBehaviour
{
    public Slider slider;

    IEnumerator Start()
    {
        yield return LoadSceneAsync(Game.current.GetSceneToLoad());
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        slider.value = 0;

        while (!op.isDone)
        {
            slider.value = op.progress;

            yield return null;
        }
    }
}