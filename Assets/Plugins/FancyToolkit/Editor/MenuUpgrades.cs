#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class MenuUpgrades : MonoBehaviour
{
    [MenuItem("FancyTools/Speed/x0.5")]
    static void DbgSpeed05()
    {
        Time.timeScale = 0.5f;
    }

    [MenuItem("FancyTools/Speed/x01")]
    static void DbgSpeed1()
    {
        Time.timeScale = 1f;
    }

    [MenuItem("FancyTools/Speed/x02")]
    static void DbgSpeed2()
    {
        Time.timeScale = 2f;
    }

    [MenuItem("FancyTools/Speed/x04")]
    static void DbgSpeed4()
    {
        Time.timeScale = 4f;
    }

    [MenuItem("FancyTools/Speed/x08")]
    static void DbgSpeed8()
    {
        Time.timeScale = 8f;
    }

    [MenuItem("FancyTools/Speed/x16")]
    static void DbgSpeed16()
    {
        Time.timeScale = 16f;
    }

    [MenuItem("FancyTools/PlayerPrefs/Clear")]
    static void RemovePrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
#endif