using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class BtShape : MonoBehaviour
{
    public System.Action<BtShape> OnUsed;

    public const float offsetDrag = 3f;

    public BtShapeData data { get; private set; }
    public int rotation { get; private set; }
    
    bool isDragging;
    Color color;

    public void Init(BtShapeData data, int rotation)
    {
        this.color = BtGrid.current.settings.blockColors.Rand();
        this.data = data;
        this.rotation = rotation;
        foreach (var item in data.GetBlocks(rotation))
        {
            var instance = Instantiate(DataManager.current.gameData.prefabBlock, transform);
            instance.transform.localPosition = item.pos.ToFloatVector();
            instance.Init(item.data);
            instance.SetColor(color);
        }

        SetDragState(false);
    }

    public void Init(BtShapeInfo info)
    {
        Init(info.data, info.rotation);
    }

    void SetDragState(bool value)
    {
        isDragging = value;

        if (value)
        {
            transform.localScale = Vector3.one;
        }
        else
        {
            transform.localScale = Vector3.one * 0.5f;
        }
    }

    public void OnMouseDown()
    {
        SetDragState(true);
    }

    Vector2 DragPosition()
        => Helpers.MouseToWorldPosition() + Vector2.up * offsetDrag;

    public void OnMouseUp()
    {
        if (!isDragging) return;

        DropAt(DragPosition());
        SetDragState(false);
    }

    private void Update()
    {
        if (isDragging) transform.position = DragPosition();
    }

    public bool DropAt(Vector2Int pos)
    {
        var placed = BtGrid.current.PlaceShape(pos.x, pos.y, data, rotation);
        if (placed != null)
        {
            foreach (var block in placed)
            {
                block.SetColor(color);
                block.SetGridRender();
            }
            OnUsed?.Invoke(this);
            Destroy(gameObject);
            return true;
        }
        else
        {
            transform.localPosition = Vector3.zero;
            return false;
        }

    }

    public bool DropAt(Vector2 worldPos)
    {
        var gridPos = BtGrid.current.GetGridPos(worldPos);
        return DropAt(gridPos);
    }
}