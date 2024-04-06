using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtBlock : MonoBehaviour
{
    [SerializeField] SpriteRenderer render;

    public void Init(BtBlockData data)
    {
        render.sprite = data.sprite;
    }

    public void SetColor(Color color)
    {
        render.color = color;
    }

    public void SetGridRender()
    {
        render.sortingOrder--;
    }

    public bool Collect()
    {
        StartCoroutine(CollectRoutine());
        return true;
    }

    IEnumerator CollectRoutine()
    {
        var color = render.color;
        float fadeSpeed = 4f;
        float alpha = 1f;

        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            color.a = alpha;
            render.color = color;
            transform.localScale = Vector3.one * (0.7f +  0.6f*(1-alpha));
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        if (Input.GetKey(KeyCode.T)) Collect();
    }
}