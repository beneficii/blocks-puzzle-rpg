using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class BtShape : MonoBehaviour
{
    public static System.Action<BtShape, bool> OnDragState;

    public System.Action<BtShape, Vector2Int> OnDropped;
    public const float offsetDrag = 3f;

    [SerializeField] Color color = Color.white;

    public BtShapeData data { get; private set; }
    public int rotation { get; private set; }

    public BtShapeInfo GetInfo() => new BtShapeInfo(data, rotation);

    List<BtBlock> blocks;
    
    bool isDragging;
    Vector3 prevDragPosition;

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

    Vector2 moveSpeed;

    void CalculateMouseSpeed()
    {
        float height = Helpers.Camera.orthographicSize * 2.0f;
        float width = height * Helpers.Camera.aspect;

        float moveSpeedX = width / Screen.width;
        float moveSpeedY = height / Screen.height;
        moveSpeed = new Vector2(moveSpeedX, moveSpeedY) * 1.32f;
    }

    void SetDragState(bool value)
    {
        isDragging = value;
        OnDragState?.Invoke(this, value);

        if (value)
        {
            CalculateMouseSpeed();
            prevDragPosition = Input.mousePosition;
            transform.localScale = Vector3.one;
            transform.position = Helpers.MouseToWorldPosition() + Vector2.up * offsetDrag;
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
        //=> Helpers.MouseToWorldPosition() + Vector2.up * offsetDrag;
        => transform.position + Vector3.up * offsetDrag;


    public void OnMouseUp()
    {
        if (!isDragging) return;

        //DropAt(DragPosition());
        DropAt(transform.position);
        SetDragState(false);
    }

    private void LateUpdate()
    {
        if (!isDragging) return;
        var delta = Input.mousePosition - prevDragPosition;
        //transform.position += DragPosition();
        transform.position += new Vector3(delta.x * moveSpeed.x, delta.y * moveSpeed.y);
        prevDragPosition = Input.mousePosition;
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
            OnDropped?.Invoke(this, pos);
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