using FancyToolkit;
using GridBoard;
using GridBoard.TileActions;
using System.Collections;
using System.Data;
using UnityEngine;

public class CheatCtrl : MonoBehaviour
{
    [SerializeField] bool enableCheats = true;
    [SerializeField] LayerMask layerMask;
    
    public string id;

    void Spawn(MyTile tile, TileData data)
    {
        tile.SetBoard(FindAnyObjectByType<Board>());
        tile.Init(data);
        tile.isActionLocked = true;
    }

    private void Update()
    {
        if (!enableCheats) return;

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(Helpers.MouseToWorldPosition(), Vector2.zero, 10, layerMask);

            if (hit.transform?.TryGetComponent<MyTile>(out var tile)??false)
            {
                var data = TileCtrl.current.Get(id);
                if (data != null)
                {
                    tile.Init(data);
                }
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            RaycastHit2D hit = Physics2D.Raycast(Helpers.MouseToWorldPosition(), Vector2.zero, 10, layerMask);

            if (hit.transform?.TryGetComponent<MyTile>(out var tile) ?? false)
            {
                id = tile.data.id;
            }
        }

    }
}
