using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class BtShape : MonoBehaviour
{
    public static System.Action<BtShape, bool> OnDragState;

    public System.Action<BtShape> OnUsed;
    public const float offsetDrag = 3f;

    [SerializeField] Color color = Color.white;

    public BtShapeData data { get; private set; }
    public int rotation { get; private set; }

    public BtShapeInfo GetInfo() => new BtShapeInfo(data, rotation);

    List<BtBlock> blocks;
    
    bool isDragging;

    void Clear()
    {
        if (blocks == null) return;

        foreach (var item in blocks)
        {
            Destroy(item.gameObject);
        }
        blocks.Clear();
    }

    public void Init(BtShapeData data, int rotation)
    {
        Clear();
        blocks = new List<BtBlock>();
        this.data = data;
        this.rotation = rotation;
        foreach (var item in data.GetBlocks(rotation))
        {
            var instance = Instantiate(DataManager.current.gameData.prefabBlock, transform);
            instance.transform.localPosition = item.pos.ToFloatVector();
            instance.Init(item.data);
            instance.SetColor(color);
            blocks.Add(instance);
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
        OnDragState?.Invoke(this, value);

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
        if (FancyInputCtrl.IsMouseOverUI()) return;

        SetDragState(true);
    }

    public Vector2 DragPosition()
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