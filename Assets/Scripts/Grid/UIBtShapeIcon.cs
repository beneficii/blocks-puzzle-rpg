using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class UIBtShapeIcon : MonoBehaviour
{
    [SerializeField] UIWithIcon prefabBlock;
    [SerializeField] int blockSize = 40;

    List<UIWithIcon> blocks;

    void Clear()
    {
        if (blocks == null) return;
        foreach (var item in blocks)
        {
            Destroy(item.gameObject);
        }
        blocks.Clear();
    }

    public void Init(BtShapeData shape, BtBlockInfo highlightedBlock = null)
    {
        Clear();
        blocks = new List<UIWithIcon>();
        foreach (var info in shape.GetBlocks())
        {
            var block = Instantiate(prefabBlock, transform);
            block.GetComponent<RectTransform>().anchoredPosition = info.pos * blockSize;
            block.SetIcon(info.data.sprite);
            blocks.Add(block);

            if (highlightedBlock != null && (highlightedBlock.pos == info.pos))
            {
                block.frame.enabled = true;
            }
        }
    }
}