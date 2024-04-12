using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class BtBlock : MonoBehaviour
{
    [SerializeField] SpriteRenderer bgRender;
    [SerializeField] SpriteRenderer iconRender;

    public BtBlockData data { get; private set; }

    public void Init(BtBlockData data)
    {
        this.data = data;
        iconRender.sprite = data.sprite;
    }

    public void SetColor(Color color)
    {
        bgRender.color = color;
    }

    public void SetGridRender()
    {
        int offset = 4;
        bgRender.sortingOrder -= offset;
        iconRender.sortingOrder -= offset;
    }

    public bool Collect()
    {
        StartCoroutine(CollectRoutine());
        return true;
    }

    IEnumerator CollectRoutine()
    {
        float fadeSpeed = 4f;
        float alpha = 1f;

        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            iconRender.SetAlpha(alpha);
            bgRender.SetAlpha(alpha);
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