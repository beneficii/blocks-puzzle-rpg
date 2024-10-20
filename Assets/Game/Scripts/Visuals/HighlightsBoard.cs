using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;
using FancyToolkit;
using System;

public class HighlightsBoard : MonoBehaviour, ILineClearHandler
{
    [SerializeField] List<TileTypeColor> typeColors;
    [SerializeField] List<SpriteRenderer> sideOrnaments;

    Dictionary<Tile.Type, TileTypeColor> typeDict;

    private void OnEnable()
    {
        LineClearer.AddHandler(this);
    }
    
    private void OnDisable()
    {
        LineClearer.RemoveHandler(this);
    }

    void Init()
    {
        typeDict = typeColors.ToDictionary(x=>x.type);
    }

    IEnumerator Glow(TileTypeColor data, float halfDuration = 0.8f)
    {
        if (data.render)
        {
            data.render.color = data.color;
            data.render.SetAlpha(0);
        }
        foreach (var item in sideOrnaments)
        {
            item.color = data.color;
            item.SetAlpha(0f);
        }

        // First phase: Fade in (alpha 0 -> 1)
        float elapsedTime = 0;
        while (elapsedTime < halfDuration)
        {
            float alpha = Mathf.Lerp(0, 1, elapsedTime / halfDuration);
            if (data.render) data.render.SetAlpha(alpha);
            foreach (var item in sideOrnaments) item.SetAlpha(alpha-0.2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Second phase: Fade out (alpha 1 -> 0)
        elapsedTime = 0;
        while (elapsedTime < halfDuration)
        {
            float alpha = Mathf.Lerp(1, 0, elapsedTime / halfDuration);
            if (data.render) data.render.SetAlpha(alpha);
            foreach (var item in sideOrnaments) item.SetAlpha(alpha+0.2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (data.render) data.render.SetAlpha(0f);
        foreach (var item in sideOrnaments) item.SetAlpha(0);
    }

    IEnumerator ILineClearHandler.HandleLinesCleared(LineClearData clearData)
    {
        if (typeDict == null) Init();

        if (clearData.tiles.Count == 0) yield break;

        var counter = new int[EnumUtil.GetLength<Tile.Type>()];
        foreach (var item in clearData.tiles)
        {
            counter[(int)item.data.type]++;
        }
        var (_, maxIndex) = counter.Select((n, i) => (n, i)).Max();
        if (typeDict.TryGetValue((Tile.Type)maxIndex, out var data))
        {
            StartCoroutine(Glow(data));
        }
    }

    [System.Serializable]
    public class TileTypeColor
    {
        public Tile.Type type;
        public Color color;
        public SpriteRenderer render;
    }
}
