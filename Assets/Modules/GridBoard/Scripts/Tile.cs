﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;
using TMPro;
using System.Linq;
using UnityEditor;

namespace GridBoard
{
    public class Tile : MonoBehaviour, IHasInfo, IIconProvider, IInfoTextProvider, IHintContainer, IHoverInfoTarget
    {
        public const float scale = 40 / 35f;
        public const float rScale = 35 / 40f;
        public static Vector2 IndexToPos(int x, int y) => new Vector2(x + 0.5f, y + 0.5f) * rScale;
        public static Vector2 IndexToPos(Vector2Int idx) => IndexToPos(idx.x, idx.y);
        public static Vector2Int PosToIndex(Vector2 pos) => (pos * scale).ToIntVector2();

        public static event System.Action<Tile> OnClickDone;
        public static event System.Action<Tile> OnPlaced;
        public static event System.Action<Tile, bool> OnChangedBoardState;
        public static event System.Action<Tile> OnInited;
        bool boardState = false;

        [SerializeField] int baseRenderLayerOrder = 100;
        [SerializeField] protected SpriteRenderer bgRender;
        [SerializeField] protected SpriteRenderer shadowRender;
        [SerializeField] protected SpriteRenderer hlRender;
        [SerializeField] protected SpriteRenderer iconRender;
        [SerializeField] protected SpriteRenderer attentionIndicator;

        [SerializeField] protected ProgressBar progressBar;

        [SerializeField] List<Sprite> bgSprites;
        [SerializeField] Sprite bgSprite1;
        [SerializeField] Sprite bgSprite2;
        [SerializeField] ParticleSystem particleBreak;

        [SerializeField] protected bool debug;

        [SerializeField] List<GameObject> objLabels;
        [SerializeField] TextMeshPro txtLevelCaption;
        int level;
        public int Level
        {
            get => level;
            set
            {
                level = value;

                foreach (var item in objLabels)
                {
                    item.SetActive(value > 0);
                }

                if (txtLevelCaption) txtLevelCaption.text = level > 0 ? $"{level}" : "";
            }
        }

        public bool isBeingPlaced;
        public bool isTaken;
        public bool isFadedOut;
        public bool isActionLocked;

        public bool IsInProgress => progressBar && progressBar.IsActive;


        public TileData data { get; private set; }
        public Vector2Int position { get; set; }
        public Board board { get; set; }

        System.Action<Tile> progressCallback;
        protected TileAction onClick;
        protected List<TileActionAccept> onDragAccept;
        protected TileState stateDefault;
        protected TileState stateCurrent;

        public virtual bool CanDrag => true; //data.canDrag;
        public virtual bool CanClick => onClick != null;

        public bool CanAccept(Tile other)
        {
            if (other == this) return false;
            return onDragAccept.Any(x=>x.CanAccept(other, this));
        }

        public virtual string GetDescription()
            => data.GetDescription(this);

        public Sprite GetIcon() => data.sprite;

        public virtual void InitVirtual(TileData data)
        {

        }

        public void Break()
        {
            if (isFadedOut) return;
            var instance = Instantiate(particleBreak, transform.position, Quaternion.identity);
            Destroy(instance, .4f);
        }

        public void Init(TileData data, int level = -1)
        {
            if (this.data != null) Clean();


            this.data = data;
            iconRender.sprite = data.sprite;
            if (level >= 0)
            {
                Level = level;
            }
            else
            {
                Level = data.startingLevel;
            }

            onClick = Factory<TileAction>.Create(data.onClick);
            if (data.onDragAccept != null)
            {
                onDragAccept = data.onDragAccept
                    .Select(x => Factory<TileActionAccept>.Create(x))
                    .Where(x => x != null)
                    .ToList();
            }
            else
            {
                onDragAccept = new List<TileActionAccept>();
            }
            
            stateDefault = Factory<TileState>.Create(data.defaultState);
            SetDefaultState();
            if (progressBar) progressBar.OnFinished += HandleProgressFinished;
            isTaken = false;
            InitVirtual(data);
            if (boardState != (bool)board)
            {
                boardState = (bool)board;
                OnChangedBoardState?.Invoke(this, boardState);
            }

            OnInited?.Invoke(this);
        }

        public virtual void SetBoard(Board board)
        {
            this.board = board;
            SetRenderLayer(RenderLayer.Board);
        }

        IEnumerator DebugBlink(Color color)
        {
            var old = bgRender.color;
            bgRender.color = color;
            yield return new WaitForSeconds(0.2f);
            bgRender.color = old;
        }

        public void Blink(Color color)
        {
            StartCoroutine(DebugBlink(color));
        }

        public virtual void Click()
        {
            if (onClick != null)
            {
                AnimateClick();
                onClick.Run(this);
                OnClickDone?.Invoke(this);
            }
        }

        public void SetState(TileState state)
        {
            stateCurrent?.Exit();
            this.stateCurrent = state;
            state?.Enter(this);
        }

        public void SetDefaultState()
        {
            SetState(stateDefault);
        }

        public virtual bool DragTo(Board board, Vector2Int? position, Tile targetTile)
        {
            if (!position.HasValue)
            {
                return false;
            }

            if (!targetTile)
            {
                return board.PlaceTile(position.Value, this);
            }

            if (debug) StartCoroutine(targetTile.DebugBlink(Color.red));

            foreach (var accept in targetTile.onDragAccept)
            {
                if (accept.CanAccept(this, targetTile))
                {
                    accept.Accept(this, targetTile);
                    break;
                }
            }
            
            return true;
        }

        public void SetRenderLayer(RenderLayer layer)
        {
            int order = baseRenderLayerOrder + (int)layer * 10;
            GetComponentInChildren<UnityEngine.Rendering.SortingGroup>().sortingOrder = order;

            shadowRender.enabled = layer == RenderLayer.Inventory;
            hlRender.enabled = layer == RenderLayer.Hover;
            /*
            bgRender.sortingOrder = order;
            iconRender.sortingOrder = order + 1;
            if (txtLevelCaption) txtLevelCaption.sortingOrder = order + 2;
            if (attentionIndicator) attentionIndicator.sortingOrder = order + 3;*/
        }

        IEnumerator CollectRoutine()
        {
            yield return FadeOut(4f);

            Destroy(gameObject);
        }

        public virtual bool Collect()
        {
            StartCoroutine(CollectRoutine());
            return true;
        }

        public void PopOut()
        {
            //ToDo: some cool animation or smth
            //transform.localScale = Vector3.one * 1.1f;
            bgRender.sprite = bgSprite2;
            //bgRender.SetAlpha(0.8f);
            isTaken = true;
        }

        public void SetPreviewColor(bool value)
        {
            bgRender.sprite = value ? bgSprite2 : bgSprite1;
        }

        
        public virtual IEnumerator FadeOut(float fadeSpeed)
        {
            if (isFadedOut) yield break;

            isFadedOut = true;
            float alpha = 1f;
            while (alpha > 0)
            {
                alpha -= Time.deltaTime * fadeSpeed;
                iconRender.SetAlpha(alpha);
                bgRender.SetAlpha(alpha);
                transform.localScale = Vector3.one * (0.7f + 0.6f * (1 - alpha));
                yield return null;
            }

            transform.localScale = Vector3.zero;
        }

        void Awake()
        {
            bgSprite1 = bgRender.sprite;
        }

        IEnumerator SpawnRoutine()
        {
            float speed = 3f;
            float progress = 0.3f;

            while (progress < 1f)
            {
                progress += Time.deltaTime * speed;
                transform.localScale = Vector3.one * Easings.OutBack(progress);
                yield return null;
            }

            transform.localScale = Vector3.one;
        }

        public void AnimateSpawn()
        {
            StartCoroutine(SpawnRoutine());
        }

        public IEnumerator ClickAnimationRoutine()
        {
            float speed = 1f;
            float progress = 0.8f;

            while (progress < 1f)
            {
                progress += Time.deltaTime * speed;
                var easing = Easings.OutBack(progress);
                transform.localScale = Vector3.one * easing*easing;
                yield return null;
            }

            transform.localScale = Vector3.one;
        }

        public void AnimateClick()
        {
            StartCoroutine(ClickAnimationRoutine());
        }

        private void OnDestroy()
        {
            Clean();
        }

        protected virtual void Clean()
        {
            if (boardState != false)
            {
                boardState = false;
                OnChangedBoardState?.Invoke(this, false);
            }
            
            data = null;
        }

        IEnumerator UpgradeRoutine()
        {
            float speed = 1f;
            float progress = 0.8f;

            while (progress < 1f)
            {
                progress += Time.deltaTime * speed;
                var easing = Easings.OutBack(progress);
                transform.localScale = Vector3.one * easing * easing;
                yield return null;
            }

            transform.localScale = Vector3.one;
        }

        public virtual IEnumerator Place()
        {
            OnPlaced?.Invoke(this);
            yield break;
        }

        public void AnimateUpgrade()
        {
            StartCoroutine(UpgradeRoutine());
        }

        public void SetHighlight(bool value)
        {
            hlRender.enabled = value;
        }

        public void SetAttention(bool value)
        {
            if (!attentionIndicator) return;

            attentionIndicator.enabled = value;
        }

        void HandleProgressFinished()
        {
            if (progressCallback == null)
            {
                Debug.LogError("no progress callback");
                return;
            }

            progressCallback.Invoke(this);
            progressCallback = null;
        }

        public virtual void CopyParams(Tile other)
        {

        }

        public void StartProgress(float duration, System.Action<Tile> callback)
        {
            progressCallback = callback;
            if (progressBar) progressBar.StartProgress(duration);
        }

        public bool HasTag(string tag)
        {
            return data != null && data.HasTag(tag);
        }

        private void FixedUpdate()
        {
            stateCurrent?.Update();
        }

        public string GetTitle() => data?.title ?? string.Empty;
        public List<string> GetTooltips() => data.GetTooltips();

        public bool ShouldShowInfo() => !data.isEmpty;

        public List<string> GetTags() => data.GetTags();

        public virtual IHasInfo GetExtraInfo()
        {
            return ((IHasInfo)data).GetExtraInfo();
        }

        public virtual List<IHintProvider> GetHintProviders() => null;

        public virtual bool ShouldShowHoverInfo() => false;

        public virtual string GetInfoText(int size) => "";

        public enum RenderLayer
        {
            Bg,
            Board,
            Inventory,
            Hover,
        }

        public enum Type
        {
            None,
            Weapon,
            Armor,
            Spell,
            Blessing,
            Trap,
            Curse,
            Empty,
        }

        public class Info
        {
            public TileData data;
            public Vector2Int pos;

            public Info(TileData data, Vector2Int pos)
            {
                this.data = data;
                this.pos = pos;
            }

            public Info(StringScanner scanner)
            {
                if (scanner.TryGet(out string id))
                {
                    data = TileCtrl.current.Get(id);
                }

                if (data == null)
                {
                    Debug.LogError("Info(str): Data not found!");
                }

                pos.x = scanner.NextInt();
                pos.y = scanner.NextInt();
            }

            public Info Rotate(int rotation, Vector2Int size)
            {
                return rotation switch
                {
                    1 => new(data, new(pos.y, size.x - pos.x - 1)),
                    2 => new(data, new(size.x - pos.x - 1, size.y - pos.y - 1)),
                    3 => new(data, new(size.y - pos.y, pos.x)),
                    _ => new(data, pos),
                };
            }

            public override string ToString()
                => $"{data.id} {pos.x} {pos.y}";

        }
    }
}
