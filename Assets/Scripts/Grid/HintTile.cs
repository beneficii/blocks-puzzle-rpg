using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HintTile : MonoBehaviour
{
    [SerializeField] TextMeshPro txtTitle;

    public void InitTimed(string caption, float lifetime)
    {
        txtTitle.text = caption;
        Destroy(gameObject, lifetime);
    }
}