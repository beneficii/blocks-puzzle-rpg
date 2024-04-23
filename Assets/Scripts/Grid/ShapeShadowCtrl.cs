using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using FancyToolkit;


public class ShapeShadowCtrl : MonoBehaviour
{
    [SerializeField] Sprite spriteBlock;
    [SerializeField] Color color;

    List<Transform> blocks = new();
    int blocksUsed = 0;
    
    BtShape currentShape;

    Vector2Int? cachedGridPos = null;

    List<Transform> GetBlocks(int count)
    {
        // Create more blocks if needed
        while (count > blocks.Count)
        {
            var obj = new GameObject();
            var render = obj.AddComponent<SpriteRenderer>();
            render.sprite = spriteBlock;
            render.color = color;

            blocks.Add(obj.transform);
        }

        while (count > blocksUsed)
        {
            blocks[blocksUsed].gameObject.SetActive(true);
            blocksUsed++;
        }

        while (blocksUsed > count)
        {
            blocks[--blocksUsed].gameObject.SetActive(false);
        }

        return blocks.Take(count).ToList();
    }


    private void OnEnable()
    {
        BtShape.OnDragState += HandleShapeDragState;
    }

    private void OnDisable()
    {
        BtShape.OnDragState -= HandleShapeDragState;
    }

    void Clear()
    {
        GetBlocks(0);
    }

    void HandleShapeDragState(BtShape shape, bool state)
    {
        if (state)
        {
            currentShape = shape;
        }
        else
        {
            currentShape = null;
            Clear();
        }
    }

    private void Update()
    {
        if (!currentShape) return;

        var grid = BtGrid.current;
        var gridPos = grid.CheckGridPos(currentShape.transform.position);
        if (gridPos == cachedGridPos) return;
        cachedGridPos = gridPos;

        if (gridPos.HasValue)
        {
            var list = BtGrid.current.GetBlockPositions(gridPos.Value, currentShape.GetInfo(), true);
            if (list != null)
            {
                int idx = 0;
                foreach (var item in GetBlocks(list.Count))
                {
                    item.position = list[idx++];
                }
            }
            else
            {
                Clear();
            }
        }
        else
        {
            Clear();
        }
    }
}