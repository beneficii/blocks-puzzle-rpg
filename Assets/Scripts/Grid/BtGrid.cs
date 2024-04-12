using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using FancyToolkit;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BtGrid : MonoBehaviour
{
    public static BtGrid current { get; private set; }

    public static System.Action<List<BtBlock>, int> OnLinesCleared;

    public int width { get; private set; }
    public int height { get; private set; }

    [SerializeField] Sprite spriteTile;

    [SerializeField] Color colorTile1;
    [SerializeField] Color colorTile2;

    BtBlock[,] blocks;
    SpriteRenderer[,] tiles;

    int[] columnBlockCount;
    int[] rowBlockCount;

    bool calculateGrid = false;

    List<Vector2Int> adjDeltas = new()
    {
        new(-1, 0),
        new (0, +1),
        new (+1, 0),
        new (0, -1),
    };


    private void Awake()
    {
        current = this;
    }

    SpriteRenderer CreateTileSprite(int x, int y)
    {
        var obj = new GameObject($"Tile[{x},{y}]");
        var render = obj.AddComponent<SpriteRenderer>();
        render.sprite = spriteTile;

        obj.transform.parent = transform;
        obj.transform.localPosition = new Vector2(x + 0.5f, y + 0.5f);
        render.color = (x % 2 != y % 2) ? colorTile1 : colorTile2;

        return render;
    }

    void Start()
    {
        var gridSize = DataManager.current.gameData.gridSize;
        width = gridSize.x;
        height = gridSize.y;

        columnBlockCount = new int[width];
        rowBlockCount = new int[height];

        tiles = new SpriteRenderer[width, height];
        blocks = new BtBlock[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x,y] = CreateTileSprite(x, y);
            }
        }
    }

    (int, int) GetXY(Vector2 pos)
    {
        int x = (int)(pos.x - transform.position.x);
        int y = (int)(pos.y - transform.position.y);

        return (x, y);
    }

    bool InBounds(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return false;

        return true;
    }

    public BtBlock GetItem(int x, int y)
    {
        if (!InBounds(x,y)) return null;

        var cell = blocks[x, y];

        return cell;
    }

    public BtBlock GetItem(Vector2 pos)
    {
        var (x, y) = GetXY(pos);

        return GetItem(x, y);
    }

    public BtBlock PlaceBlock(int x, int y, BtBlockData data)
    {
        var instance = Instantiate(DataManager.current.gameData.prefabBlock, tiles[x,y].transform);
        instance.transform.localPosition = Vector3.zero;
        instance.Init(data);
        blocks[x, y] = instance;

        columnBlockCount[x]++;
        rowBlockCount[y]++;

        Assert.IsTrue(columnBlockCount[x] <= height, "Column block count overflow!");
        Assert.IsTrue(rowBlockCount[x] <= width, "Row block count overflow!");

        calculateGrid = true;

        return instance;
    }

    public List<BtBlock> PlaceShape(int x, int y, BtShapeData data, int rotation)
    {
        foreach (var item in data.GetBlocks(rotation))
        {
            var pos = item.pos + new Vector2Int(x,y);
            if (!InBounds(pos.x, pos.y)) return null;

            var other = GetItem(pos.x, pos.y);
            if (other) return null;
        }

        var result = new List<BtBlock>();
        foreach (var item in data.GetBlocks(rotation))
        {
            var pos = item.pos + new Vector2Int(x, y);

            var block = PlaceBlock(pos.x, pos.y, item.data);
            result.Add(block);
        }

        return result;
    }

    public List<BtBlock> PlaceShape(Vector2 worldPos, BtShapeData data, int rotation)
    {
        var (x, y) = GetXY(worldPos);

        return PlaceShape(x, y, data, rotation);
    }

    public IEnumerable<BtBlock> GetAdjacentItems(int x, int y)
    {
        foreach (var delta in adjDeltas)
        {
            var dx = x + delta.x;
            var dy = y + delta.y;

            var item = GetItem(dx, dy);

            if (item) yield return item;
        }
    }

    BtBlock CollectBlock(int x, int y)
    {
        var block = blocks[x, y];
        if (!block) return null;

        if (block.Collect())
        {
            blocks[x, y] = null;
            columnBlockCount[x]--;
            rowBlockCount[y]--;
            return block;
        }
        else
        {
            return null;
        }
    }

    void CalculateGrid()
    {
        if (!calculateGrid) return;
        
        var removeColumns = new List<int>();
        var removeRows = new List<int>();

        var blocksRemoved = new List<BtBlock>();

        for (int i = 0; i < width; i++)
        {
            if (columnBlockCount[i] >= height)
            {
                removeColumns.Add(i);
            }
        }

        for (int i = 0; i < height; i++)
        {
            if (rowBlockCount[i] >= width)
            {
                removeRows.Add(i);
            }
        }

        foreach (var x in removeColumns)
        {
            for (int y = 0; y < height; y++)
            {
                var block = CollectBlock(x, y);
                if (block) blocksRemoved.Add(block);
            }
        }

        foreach (var y in removeRows)
        {
            for (int x = 0; x < width; x++)
            {
                var block = CollectBlock(x, y);
                if (block) blocksRemoved.Add(block);
            }
        }

        calculateGrid = false;

        OnLinesCleared?.Invoke(blocksRemoved, removeColumns.Count + removeRows.Count);
    }

    private void Update()
    {
        CalculateGrid();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (columnBlockCount == null) return;

        var style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 16;
        style.alignment = TextAnchor.MiddleCenter;

        for (int x = 0; x < width; x++)
        {
            Handles.Label(transform.position + Vector3.down * 0.2f + Vector3.right * (0.5f + x), $"{columnBlockCount[x]}", style);
        }

        for (int y = 0; y < width; y++)
        {
            Handles.Label(transform.position + Vector3.left * 0.2f + Vector3.up * (0.5f + 0.5f + y), $"{rowBlockCount[y]}", style);
        }
    }
#endif

}