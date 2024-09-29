using System.Collections;
using UnityEngine;

public class StageData : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [System.Serializable]
    public enum Type
    {
        None,
        Enemy,
        Elite,
        Shop,
        Dialog,
        Boss
    }
}