using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;
using FancyToolkit;
using System.Linq;
using TMPro;
using System;

namespace GridBoard
{
    public class Board : MonoBehaviour
    {
        const string layoutResourceFolder = "BoardLayouts";

        public int width = 8;
        public int height = 8;

        public event System.Action OnCleared;

        public event System.Action<Tile> OnTilePlaced;
        public event System.Action<Tile> OnTileRemoved;

        public Tile prefabTile;
        [SerializeField] List<Sprite> tileBgSprites;
        [SerializeField] bool addSpriteTiles;

        [SerializeField] Color colorTile1 = Color.white;
        [SerializeField] Color colorTile2 = Color.white;
        [SerializeField] Color colorTileHighlight = Color.white;

        [SerializeField] AudioClip soundPlace;

        [SerializeField] bool shouldHighlightTiles;
        [SerializeField] bool drawGizmos;

        [SerializeField] GameObject prefabGhostTile;

        [SerializeField] DatabaseBoardPresets dbBoardPresets;
        List<PredefinedLayout> predefinedBoards;

        Tile[,] tiles;
        SpriteRenderer[,] bgTiles;

        HashSet<Vector2Int> allowedTiles = null;

        [SerializeField] public Tile mouseTile { get; private set; }
        [SerializeField] public Vector2Int? mousePos { get; private set; }
        public UnityEvent<Tile> OnMouseTileChanged;
        public UnityEvent<Vector2Int?> OnMousePosChanged;

        Dictionary<string, int> dictTileCounter = new();
        Dictionary<string, int> dictTagCounter = new();

        public int StateVersion { get; private set; }
        public bool InitDone { get; private set; }

        Queue<Tile> emptyTileQueue = new();
        List<GameObject> toClearAllowedTiles = new();

        List<Vector2Int> adjDeltas = new()
        {
            new (-1, 0),
            new (0, +1),
            new (+1, 0),
            new (0, -1),
        };

        List<Vector2Int> aroundDeltas = new()
        {
            new (-1, 0),  // Left
            new (-1, +1), // Top-left
            new (0, +1),  // Up
            new (+1, +1), // Top-right
            new (+1, 0),  // Right
            new (+1, -1), // Bottom-right
            new (0, -1),  // Down
            new (-1, -1), // Bottom-left
        };


        Color GetBgColor(int x, int y)
            => (x % 2 != y % 2) ? colorTile1 : colorTile2;

        public bool IsOcupied(int x, int y) => (bool)tiles[x, y];

        SpriteRenderer CreateBgTile(int x, int y)
        {
            var obj = new GameObject($"Tile[{x},{y}]");
            var render = obj.AddComponent<SpriteRenderer>();
            render.sprite = addSpriteTiles ? tileBgSprites.Rand() : null;

            obj.transform.parent = transform;
            obj.transform.localPosition = Tile.IndexToPos(x, y);
            render.color = GetBgColor(x, y);

            return render;
        }

        public int GetIdTileCount(string id) => dictTileCounter.Get(id);
        public int GetTagTileCount(string tag) => dictTagCounter.Get(tag);

        public void Init()
        {
            if (bgTiles != null) return;

            predefinedBoards = dbBoardPresets.GenerateBoards();

            bgTiles = new SpriteRenderer[width, height];
            tiles = new Tile[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bgTiles[x, y] = CreateBgTile(x, y);
                }
            }

            foreach (var item in GetComponents<IModule>())
            {
                item.InitBoard(this);
            }
            StateVersion++;
            Tile.OnChangedBoardState += HandleTileChangedBoardState;
            InitDone = true;
        }

        void Awake()
        {
            Init();
        }

        public Vector2 GetAllowedMidPos()
        {
            Assert.IsTrue(allowedTiles != null);
            Assert.IsTrue(allowedTiles.Count > 0);

            Vector2 totalPos = Vector2.zero;
            foreach (var item in allowedTiles)
            {
                var pos = bgTiles[item.x, item.y].transform.position;
                totalPos.x += pos.x;
                totalPos.y += pos.y;
            }

            return new Vector2(
                totalPos.x / allowedTiles.Count,
                totalPos.y / allowedTiles.Count
            );
        }

        void HandleTileChangedBoardState(Tile tile, bool state)
        {
            var id = tile.data.id;
            if (!dictTileCounter.ContainsKey(id)) dictTileCounter.Add(id, 0);
            dictTileCounter[id] += state ? +1 : -1;

            foreach (var tag in tile.data.tags)
            {
                if (!dictTagCounter.ContainsKey(tag)) dictTagCounter.Add(tag, 0);
                dictTagCounter[tag] += state ? +1 : -1;
            }
        }

        public bool CanPlace(int x, int y)
        {
            if (!InBounds(x, y)) return false;
            if (tiles[x, y]) return false;

            if (allowedTiles != null && !allowedTiles.Contains(new(x,y))) return false;

            return true;
        }

        public bool CanPlace(Vector2 worldPos)
        {
            var (x, y) = GetXY(worldPos);
            return CanPlace(x, y);
        }

        bool PlaceTileInstant(int x, int y, Tile instance)
        {
            if (!CanPlace(x, y)) return false;

            RemoveTile(instance, false);

            instance.transform.parent = bgTiles[x, y].transform;
            instance.transform.localPosition = Vector3.zero;
            tiles[x, y] = instance;
            instance.position = new(x, y);
            instance.SetBoard(this);

            StateVersion++;
            OnTilePlaced?.Invoke(instance);

            return true;
        }

        Tile PlaceTileInstant(int x, int y, TileData data)
        {
            if (!CanPlace(x, y)) return null;

            var instance = Instantiate(prefabTile, bgTiles[x, y].transform);

            if (!PlaceTileInstant(x, y, instance))
            {
                Destroy(instance);
                return null;
            }
            instance.Init(data);

            return instance;
        }

        Tile PlaceTileInstant(Tile.Info info)
            => PlaceTileInstant(info.pos.x, info.pos.y, info.data);

        public Tile PlaceTile(int x, int y, TileData data)
        {
            var instance = PlaceTileInstant(x, y, data);
            if (!instance) return null;
            
            soundPlace?.PlayNow();
            instance.AnimateSpawn();

            return instance;
        }

        public Tile PlaceTile(Tile.Info info)
            => PlaceTile(info.pos.x, info.pos.y, info.data);

        public bool PlaceTile(Vector2Int pos, Tile instance)
            => PlaceTileInstant(pos.x, pos.y, instance);

        public Tile PlaceTile(Vector2 worldPos, TileData data)
        {
            var (x, y) = GetXY(worldPos);

            return PlaceTile(x, y, data);
        }

        public Tile PlaceTile(Vector2Int pos, TileData data)
            => PlaceTile(pos.x, pos.y, data);

        public Tile PlaceTileAtMouse(TileData data)
        {
            if (mouseTile != null) return null;
            if (!mousePos.HasValue) return null;
            var pos = mousePos.Value;

            return PlaceTile(pos.x, pos.y, data);
        }

        public void UnlockAllTileActions()
        {
            foreach (var item in GetAllTiles())
            {
                item.isActionLocked = false;
            }
        }

        void Clear()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var oldTile = tiles[x, y];
                    if (!oldTile) continue;
                    OnTileRemoved?.Invoke(oldTile);
                    Destroy(oldTile.gameObject);
                }
            }

            tiles = new Tile[width, height];
            ClearAllowed();
            OnCleared?.Invoke();
        }

        void ClearAllowed()
        {
            allowedTiles = null;
            foreach (var obj in toClearAllowedTiles)
            {
                Destroy(obj);
            }
            toClearAllowedTiles.Clear();
        }

        public void LoadLayout(PredefinedLayout layout)
        {
            Clear();
            foreach (var info in layout.tiles)
            {
                var pos = info.pos;
                tiles[pos.x, pos.y] = PlaceTileInstant(info);
            }
            var allowed = layout.allowedTiles;
            if (allowed != null && allowed.Count > 0)
            {
                allowedTiles = new (allowed);
                foreach (var tile in allowed)
                {
                    var obj = Instantiate(prefabGhostTile, bgTiles[tile.x, tile.y].transform);
                    toClearAllowedTiles.Add(obj);
                }
            }
            StateVersion++;
        }

        public void LoadLayoutByName(string name, TileData specialTile = null)
        {
            var png = Resources.Load<Texture2D>($"{layoutResourceFolder}/{name}");
            if (png == null)
            {
                Debug.LogError($"Could not find layout by name ({name})");
                return;
            }

            var layout = dbBoardPresets.FromTexture(png);
            if (layout == null)
            {
                Debug.LogError($"Could not get layout from texture ({name})");
                return;
            }

            if (specialTile != null)
            {
                layout = layout.Replace(TileCtrl.current.placeholderTile, specialTile);
            }

            LoadLayout(layout);
        }

        public void LoadRandomLayout(TileData specialTile = null)
        {
            var randomLayout = predefinedBoards.Rand();
            if (specialTile != null)
            {
                randomLayout = randomLayout.Replace(TileCtrl.current.placeholderTile, specialTile);
            }

            LoadLayout(randomLayout);
        }

        public void LoadRandomLayout(string specialTileId)
        {
            var tile = TileCtrl.current.Get(specialTileId);
            LoadRandomLayout(tile);
        }

        public void LoadRandomLayout(int level, TileData specialTile = null)
        {
            var randomLayout = predefinedBoards
                        .Where(x => x.level == level)
                        .Rand();

            if (specialTile != null)
            {
                randomLayout = randomLayout.Replace(TileCtrl.current.placeholderTile, specialTile);
            }

            LoadLayout(randomLayout);
        }

        TextMeshPro txtDebug;
        (int, int) GetXY(Vector2 pos)
        {
            int x = Mathf.RoundToInt((pos.x - transform.position.x) * Tile.scale);
            int y = Mathf.RoundToInt((pos.y - transform.position.y) * Tile.scale);

            return (x, y);
        }

        public Vector2Int GetGridPos(Vector2 worldPos)
        {
            var (x, y) = GetXY(worldPos);
            return new Vector2Int(x, y);
        }

        public Vector2Int? CheckGridPos(Vector2 worldPos)
        {
            var (x, y) = GetXY(worldPos);
            if (!InBounds(x, y)) return null;
            return new Vector2Int(x, y);
        }

        bool InBounds(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return false;

            return true;
        }

        public Tile GetItem(int x, int y)
        {
            if (!InBounds(x, y)) return null;

            var cell = tiles[x, y];

            return cell;
        }

        public Tile GetItem(Vector2Int pos)
            => GetItem(pos.x, pos.y);

        public Tile GetItem(Vector2 pos)
        {
            var (x, y) = GetXY(pos);

            return GetItem(x, y);
        }


        public List<Vector2> GetTilePositions(Vector2Int origin, List<Tile.Info> deltaInfos, bool strict)
        {
            var result = new List<Vector2>();

            foreach (var info in deltaInfos)
            {
                var pos = info.pos + origin;
                if (!InBounds(pos.x, pos.y)) return null;

                if (strict)
                {
                    var other = GetItem(pos.x, pos.y);
                    if (other) return null;
                }

                result.Add(bgTiles[pos.x, pos.y].transform.position);
            }

            return result;
        }

        public List<Vector2Int> GetTileGridPositions(Vector2Int origin, List<Tile.Info> deltaInfos, bool strict)
        {
            var result = new List<Vector2Int>();

            foreach (var info in deltaInfos)
            {
                var pos = info.pos + origin;
                if (!InBounds(pos.x, pos.y)) return null;

                if (strict)
                {
                    if (IsOcupied(pos.x, pos.y)) return null;
                }

                result.Add(pos);
            }

            return result;
        }

        public bool CanFit(int x, int y, List<Tile.Info> deltaInfos)
        {
            var origin = new Vector2Int(x, y);
            foreach (var item in deltaInfos)
            {
                var pos = item.pos + origin;
                if(!CanPlace(pos.x, pos.y)) return false;
            }

            return true;
        }

        public List<Tile> PlaceTiles(int x, int y, List<Tile.Info> infoDeltas)
        {
            if (!CanFit(x, y, infoDeltas)) return null;

            var result = new List<Tile>();
            foreach (var item in infoDeltas)
            {
                var pos = item.pos + new Vector2Int(x, y);

                var block = PlaceTile(pos.x, pos.y, item.data);
                result.Add(block);
            }

            ClearAllowed();
            return result;
        }

        public List<Tile> PlaceTiles(Vector2 worldPos, List<Tile.Info> infoDeltas)
        {
            var (x, y) = GetXY(worldPos);

            return PlaceTiles(x, y, infoDeltas);
        }

        public IEnumerable<Tile> GetAdjacentTiles(int x, int y)
        {
            foreach (var delta in adjDeltas)
            {
                var dx = x + delta.x;
                var dy = y + delta.y;

                var item = GetItem(dx, dy);

                if (item) yield return item;
            }
        }

        public IEnumerable<Tile> GetTilesAround(int x, int y)
        {
            foreach (var delta in aroundDeltas)
            {
                var dx = x + delta.x;
                var dy = y + delta.y;

                var item = GetItem(dx, dy);

                if (item) yield return item;
            }
        }

        public static void RemoveTile(Tile tile, bool destroy = true)
        {
            if (destroy) Destroy(tile.gameObject);

            var oldBoard = tile.board;
            if (!oldBoard) return;
            var pos = tile.position;
            if (oldBoard.GetItem(pos) == tile)
            {
                tile.board.OnTileRemoved?.Invoke(tile);
                oldBoard.tiles[pos.x, pos.y] = null;
            }
        }

        public Tile CollectAt(int x, int y)
        {
            var tile = tiles[x, y];
            if (!tile) return null;

            if (tile.Collect())
            {
                OnTileRemoved?.Invoke(tile);
                tiles[x, y] = null;
                return tile;
            }
            else
            {
                return null;
            }
        }

        public Tile PopOutAt(int x, int y)
        {
            var tile = tiles[x, y];
            if (!tile) return null;

            OnTileRemoved?.Invoke(tile);
            tiles[x, y] = null;
            tile.PopOut();

            return tile;
        }

        public Tile CollectAt(Vector2Int pos)
            => CollectAt(pos.x, pos.y);

        public Tile CollectAtMouse()
        {
            if (mousePos == null) return null;
            var pos = mousePos.Value;
            return CollectAt(pos.x, pos.y);
        }

        void CheckTileUnderMouse()
        {
            var gridPos = CheckGridPos(Helpers.MouseToWorldPosition());
            Tile tile = null;
            if (gridPos.HasValue)
            {
                tile = GetItem(gridPos.Value);
            }

            bool hadChanges = false;

            if (gridPos != mousePos)
            {
                if (shouldHighlightTiles)
                {
                    if (mousePos.HasValue)
                    {
                        var pos = mousePos.Value;
                        bgTiles[pos.x, pos.y].color = GetBgColor(pos.x, pos.y);
                    }

                    if (gridPos.HasValue)
                    {
                        var pos = gridPos.Value;
                        bgTiles[pos.x, pos.y].color = colorTileHighlight;
                    }
                }

                mousePos = gridPos;
                hadChanges = true;
                OnMousePosChanged?.Invoke(mousePos);

            }

            if (tile != mouseTile)
            {
                mouseTile = tile;
                hadChanges = true;
                OnMouseTileChanged?.Invoke(tile);
            }

            if (hadChanges)
            {
                // nothing so far
            }
        }

        public bool IsFreeSpot(Vector2Int pos)
        {
            if (!InBounds(pos.x, pos.y)) return false;

            return GetItem(pos) == null;
        }

        public IEnumerable<Vector2Int> GetFreeSpots()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!tiles[x, y]) yield return new(x, y);
                }
            }
        }

        public List<Vector2Int> GetEmptyTilesAround(Vector2Int origin)
        {
            var tilesAround = new List<Vector2Int>();

            Vector2Int[] directions = new Vector2Int[]
            {
                new Vector2Int(0, 1),    // North
                new Vector2Int(0, -1),   // South
                new Vector2Int(1, 0),    // East
                new Vector2Int(-1, 0),   // West
                new Vector2Int(1, 1),    // Northeast
                new Vector2Int(-1, 1),   // Northwest
                new Vector2Int(1, -1),   // Southeast
                new Vector2Int(-1, -1)   // Southwest
            };

            foreach (var direction in directions)
            {
                var pos = origin + direction;
                if (CanPlace(pos)) tilesAround.Add(pos);
            }

            return tilesAround;
        }

        public IEnumerable<Tile> GetAllTiles()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var tile = tiles[x, y];
                    if (tile)
                    {
                        yield return tile;
                    }
                }
            }
        }

        public IEnumerable<Tile> GetAllTiles(Predicate<Tile> func)
        {
            foreach (var item in GetAllTiles())
            {
                if (func(item)) yield return item;
            }
        }

        public IEnumerable<Tile> GetEmptyTiles()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var tile = tiles[x, y];
                    if (tile && tile.data.isEmpty)
                    {
                        yield return tile;
                    }
                }
            }
        }

        public void GenerateEmptyTileQueue()
        {
            var list = new List<Tile>();
            foreach (var item in GetEmptyTiles())
            {
                if (!item.isTaken) list.Add(item);
            }
            list.Shuffle();

            emptyTileQueue = new Queue<Tile>(list);
        }

        public Tile TakeEmptyTile()
        {
            Tile tile = null;
            while (emptyTileQueue.Count > 0 && (!tile || tile.isTaken))
            {
                tile = emptyTileQueue.Dequeue();
            }

            if (!tile)
            {
                GenerateEmptyTileQueue();
                if (emptyTileQueue.Count == 0) return null;
                tile = emptyTileQueue.Dequeue();
            }

            tile.isTaken = true;
            return tile;
        }

        public IEnumerable<Tile> GetNonEmptyTiles()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var tile = tiles[x, y];
                    if (tile && !tile.data.isEmpty)
                    {
                        yield return tile;
                    }
                }
            }
        }

        public IEnumerable<TTile> GetNonEmptyTiles<TTile>() where TTile : Tile
            => GetNonEmptyTiles().OfType<TTile>();

        public IEnumerable<Tile.Info> GetTileInfos()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var tile = tiles[x, y];
                    if (tile && !tile.data.isEmpty)
                    {
                        yield return new(tile.data, new(x,y));
                    }
                }
            }
        }

        private void Update()
        {
            CheckTileUnderMouse();
        }

        private void OnDestroy()
        {
            Tile.OnChangedBoardState -= HandleTileChangedBoardState;
        }

        private void OnDrawGizmosSelected()
        {
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;

            Gizmos.color = Color.blue;
            var size = new Vector2(width, height) * Tile.rScale;
            Gizmos.DrawWireCube((Vector2)transform.position + size / 2f, size);

            if (bgTiles != null)
            {
                foreach (var tile in bgTiles)
                {
                    Gizmos.DrawWireCube((Vector2)tile.transform.position, new Vector2(Tile.rScale, Tile.rScale));
                }
            }
        }

        public IEnumerable<Tile> IterateTiles()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (tiles[x, y]) yield return tiles[x, y];
                }
            }
        }

        public Tile PlaceAtRandom(string id, int level)
        {
            var data = TileCtrl.current.Get(id);
            if (data == null)
            {
                Debug.Log($"No Data by id {id}");
                return null;
            }

            var freeSpots = GetFreeSpots().ToList();
            if (freeSpots.Count == 0) return null;
            var spot = freeSpots.Rand();
            var tile = PlaceTile(spot, data);
            if (tile) tile.Level = level;
            return tile;
        }


        public Tile PlaceAround(string id, Vector2Int position, int level = 0)
        {
            var data = TileCtrl.current.Get(id);
            if (data == null)
            {
                Debug.Log($"No Data by id {id}");
                return null;
            }

            Vector2Int spot;
            if (IsFreeSpot(position))
            {
                // tile was removed
                spot = position;
            }
            else
            {
                var spots = GetEmptyTilesAround(position);
                if (spots.Count == 0) return null;
                spot = spots.Rand();
            }

            var instance = PlaceTile(spot, data);

            if (!instance) return null;

            instance.AnimateSpawn();
            instance.Level = level;
            return instance;
        }

        public interface IModule
        {
            void InitBoard(Board board);
        }
    }

    public class PredefinedLayout : DataWithId
    {
        public int level;
        public List<Tile.Info> tiles;
        public List<Vector2Int> allowedTiles;

        public PredefinedLayout Replace(TileData from, TileData to)
        {
            return new()
            {
                level = level,
                tiles = tiles.Select(
                        x => x.data == from
                        ? new Tile.Info(to, x.pos)
                        : x
                    ).ToList(),
            };
        }

        public PredefinedLayout()
        {

        }

        public PredefinedLayout(Board board)
        {
            level = 0;
            tiles = board.GetTileInfos()
                .ToList();
        }

        public PredefinedLayout(StringScanner scanner)
        {
            tiles = new();
            while (!scanner.Empty)
            {
                tiles.Add(new(scanner));
            }
        }

        public override string ToString()
            => string.Join(" ", tiles.Select(x => x.ToString()));
    }

    public interface IBoardUpdateSubscriber
    {
        bool NeedsUpdate { get; }
        void BoardUpdated(Board board);
    }

}