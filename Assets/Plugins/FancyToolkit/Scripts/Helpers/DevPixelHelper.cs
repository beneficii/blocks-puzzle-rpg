#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevPixelHelper : MonoBehaviour
{
    public GameObject prefab;
    public int pixelSize = 32;
    public float firstPos = -0.515625f;
    public int count = 12;
    public int gap = 4;

    [ContextMenu("Place Items")]
    public void PlaceItems()
    {
        float gapSize = gap / 32f;
        for (int i = 0; i < count; i++)
        {
            float x = firstPos + (gapSize * i);
            Instantiate(prefab, transform.position + new Vector3(x, 0, 0), Quaternion.identity, transform.parent).name = $"cell{i:D2}";

        }
    }
}
#endif