using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FancyToolkit
{
    public class SceneSwitchScript : MonoBehaviour
    {
        [SerializeField] string sceneName;

        public void Load()
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
