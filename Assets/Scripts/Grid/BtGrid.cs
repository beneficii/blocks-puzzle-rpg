using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using FancyToolkit;
#if UNITY_EDITOR
using UnityEditor;
#endif

[DefaultExecutionOrder(-1)]
public class BtGrid : MonoBehaviour
{
    public static BtGrid current { get; private set; }

    public static System.Action<List<BtBlock>, int> OnLinesCleared;

    public int width { get; private set; }
    public int height { get; private set; }

    [SerializeField] Sprite spriteTile;
    [SerializeField] HintTile prefabHintTile;

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

    public TempGridState MakeTempGrid()
    {
        return new TempGridState(this);
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

    public bool CanFit(int x, int y, List<BtBlockInfo> blocks)
    {
        var origin = new Vector2Int(x, y);
        foreach (var item in blocks)
        {
            var pos = item.pos + origin;
            if (!InBounds(pos.x, pos.y)) return false;

            var other = GetItem(pos.x, pos.y);
            if (other) return false;
        }

        return true;
    }

    public bool CanFit(int x, int y, BtShapeData data, int rotation) => CanFit(x, y, data.GetBlocks(rotation));
      
    public bool CanFit(BtShapeData data, int rotation)
    {
        var blocks = data.GetBlocks(rotation);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (CanFit(x, y, blocks))
                {
                    print($"{blocks.Count} fits: {x},{y}");
                    return true;
                }
            }
        }

        return false;
    }

    public List<BtBlock> PlaceShape(int x, int y, BtShapeData data, int rotation)
    {
        if (!CanFit(x, y, data, rotation)) return null;

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
        int totalCleared = removeColumns.Count + removeRows.Count;
        if (totalCleared > 0)
        {
            OnLinesCleared?.Invoke(blocksRemoved, totalCleared);
        }
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

    public void ShowHint(List<Vector2Int> hints)
    {
        StartCoroutine(HintRoutine(hints));
    }

    IEnumerator HintRoutine(List<Vector2Int> hints)
    {
        int idx = 1;
        foreach (var item in hints)
        {
            var tile = tiles[item.x, item.y];

            prefabHintTile.MakeInstance(tile.transform.position)
                .InitTimed(idx.ToString(), 2f);
            idx++;

            yield return new WaitForSeconds(0.3f);
        }
    }

    public class TempGridState
    {
        public bool[,] ocupied;
        int width;
        int height;

        int[] rows;
        int[] columns;

        List<Vector2Int> searchPoints;

        public Vector2Int hint { get; private set; }

        void MakeRandomSearchPoints()
        {
            searchPoints = new List<Vector2Int>();
            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    searchPoints.Add(new Vector2Int(x, y));
                }
            }

            searchPoints.Shuffle();
        }

        bool InBounds(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return false;

            return true;
        }

        public TempGridState(BtGrid grid)
        {
            width = grid.width;
            height = grid.height;
            ocupied = new bool[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    ocupied[x, y] = (bool)grid.blocks[x, y];
                }
            }
            rows = grid.rowBlockCount.ToArray();
            columns = grid.columnBlockCount.ToArray();
            MakeRandomSearchPoints();
        }

        void CheckLines()
        {
            var removeColumns = new List<int>();
            var removeRows = new List<int>();

            for (int i = 0; i < width; i++)
            {
                if (columns[i] >= height)
                {
                    removeColumns.Add(i);
                }
            }

            for (int i = 0; i < height; i++)
            {
                if (rows[i] >= width)
                {
                    removeRows.Add(i);
                }
            }

            foreach (var x in removeColumns)
            {
                for (int y = 0; y < height; y++)
                {
                    if (ocupied[x,y])
                    {
                        ocupied[x, y] = false;
                        columns[x]--;
                        rows[y]--;
                    }
                }
            }

            foreach (var y in removeRows)
            {
                for (int x = 0; x < width; x++)
                {
                    if (ocupied[x, y])
                    {
                        ocupied[x, y] = false;
                        columns[x]--;
                        rows[y]--;
                    }
                }
            }
        }

        bool CanFit(Vector2Int origin, List<Vector2Int> blocks)
        {
            foreach (var item in blocks)
            {
                var pos = origin + item;

                if (!InBounds(pos.x, pos.y)) return false;
                if (ocupied[pos.x, pos.y]) return false;
            }

            return true;
        }

        public bool AddPiece(List<Vector2Int> blocks)
        {
            foreach (var pos in searchPoints)
            {
                if (CanFit(pos, blocks))
                {
                    foreach (var block in blocks)
                    {
                        var bpos = pos + block;
                        
                        ocupied[bpos.x, bpos.y] = true;
                        columns[bpos.x]++;
                        rows[bpos.y]++;
                    }
                    CheckLines();
                    hint = pos;
                    return true;
                }
            }

            return false;
        }
    }
}