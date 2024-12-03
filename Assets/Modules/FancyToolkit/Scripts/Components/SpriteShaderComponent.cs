using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteShaderComponent : MonoBehaviour
{
    private Dictionary<SpriteRenderer, Material> originalMaterials = null;

    private const string SaturationProperty = "_Saturation";


    void EnsureInit()
    {
        if (originalMaterials == null) GatherAndCloneMaterials();
    }

    private void GatherAndCloneMaterials()
    {
        var images = GetComponentsInChildren<SpriteRenderer>();
        originalMaterials = new();

        foreach (var image in images)
        {
            if (image.material != null && !originalMaterials.ContainsKey(image))
            {
                // Clone material and assign it to the Image
                Material clonedMaterial = new Material(image.material);
                originalMaterials[image] = clonedMaterial;
                image.material = clonedMaterial;
            }
        }
    }

    public void SetGrayscale(bool value)
    {
        SetSaturation(value ? 0 : 1);
    }

    private void SetSaturation(float value)
    {
        EnsureInit();
        foreach (var kvp in originalMaterials)
        {
            if (kvp.Value.HasProperty(SaturationProperty))
            {
                kvp.Value.SetFloat(SaturationProperty, value);
            }
        }
    }
}
