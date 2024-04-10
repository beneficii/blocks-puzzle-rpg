using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class BtShape : MonoBehaviour
{
    public System.Action<BtShape> OnUsed;

    public const float offsetDrag = 3f;

    BtShapeData data;
    bool isDragging;

    Color color;
    int rotation;

    public void Init(BtShapeData data, Color color, int rotation)
    {
        this.color = color;
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

    public void DropAt(Vector2 pos)
    {
        var placed = BtGrid.current.PlaceShape(pos, data, rotation);
        if (placed != null)
        {
            foreach (var block in placed)
            {
                block.SetColor(color);
                block.SetGridRender();
            }
            OnUsed?.Invoke(this);
            Destroy(gameObject);
        }
        else
        {
            transform.localPosition = Vector3.zero;
        }
    }
}