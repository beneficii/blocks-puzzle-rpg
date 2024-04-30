using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;
using System;

public class HintCtrl : MonoBehaviour
{
    static HintCtrl _current;
    public static HintCtrl current
    {
        get
        {
            if (!_current)
            {
                _current = FindFirstObjectByType<HintCtrl>();
            }

            return _current;
        }
    }

    [SerializeField] HintTile prefabTile;
    [SerializeField] float displayDelay = 0.5f;
    [SerializeField] float fadeDelay = 0.15f;


    public void Show(Queue<BtHint> hints)
    {
        if (hints == null) return;

        StartCoroutine(HintRoutine(hints));
    } 

    IEnumerator HintRoutine(Queue<BtHint> hints)
    {
        int idx = 1;
        var arr = hints.ToArray();
        foreach (var item in arr)
        {
            if (!Show(item.info, item.pos, idx++)) yield break;
            yield return new WaitForSeconds(displayDelay);
        }
    }

    public bool Show(BtShapeInfo info, Vector2Int gridPos, int idx)
    {
        var list = BtGrid.current.GetBlockPositions(gridPos, info, false);
        if (list != null)
        {
            foreach (var pos in list)
            {
                Instantiate(prefabTile, pos, Quaternion.identity)
                    .InitTimed($"{idx}", displayDelay + fadeDelay);
            }
            return true;
        } else
        {
            Debug.LogError("Could not get hint position");
            return false;
        }
    }
}

public class BtHint
{
    public BtShapeInfo info;
    public Vector2Int pos;

    public BtHint(BtShapeInfo info, Vector2Int pos)
    {
        this.info = info;
        this.pos = pos;
    }

    public bool Matches(BtShapeInfo info, Vector2Int pos)
    {
        return info.data == this.info.data
            && info.rotation == this.info.rotation
            && pos == this.pos;
    }
}