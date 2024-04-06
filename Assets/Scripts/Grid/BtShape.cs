using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class BtShape : MonoBehaviour
{
    public System.Action<BtShape> OnUsed;

    BtShapeData data;
    bool isDragging;

    Color color;

    public void Init(BtShapeData data, Color color)
    {
        this.color = color;
        this.data = data;
        foreach (var item in data.GetBlocks())
        {
            var instance = Instantiate(DataManager.current.gameData.prefabBlock, transform);
            instance.transform.localPosition = new Vector3(item.x, item.y);
            instance.Init(item.data);
            instance.SetColor(color);
        }
    }

    public void OnMouseDown()
    {
        isDragging = true;
    }

    public void OnMouseUp()
    {
        if (!isDragging) return;

        DropAt(Helpers.MouseToWorldPosition());
        isDragging = false;
    }

    private void Update()
    {
        if (isDragging) transform.position = Helpers.MouseToWorldPosition();
    }

    public void DropAt(Vector2 pos)
    {
        var placed = BtGrid.current.PlaceShape(pos, data);
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