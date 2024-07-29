using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjHasSave : MonoBehaviour
{
    [SerializeField] bool showIfHasSave;

    void Start()
    {
        gameObject.SetActive(showIfHasSave == GameSave.HasSave());
    }
}