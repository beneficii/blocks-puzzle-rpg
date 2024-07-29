using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;
using GridBoard;

namespace TileShapes
{
    public class HintCtrl : MonoBehaviour
    {
        static HintCtrl _current;
        public static HintCtrl current
        {
            get
            {
                if (!_current)
                {
                    _current = FindFirstObjectByType<HintCtrl>();
                }

                return _current;
            }
        }

        [SerializeField] GameObject prefabTile;
        [SerializeField] float displayDelay = 0.5f;
        [SerializeField] float fadeDelay = 0.15f;
        [SerializeField] Board board;


        public void Show(Queue<Hint> hints)
        {
            if (hints == null) return;

            StartCoroutine(HintRoutine(hints));
        }

        IEnumerator HintRoutine(Queue<Hint> hints)
        {
            int idx = 1;
            var arr = hints.ToArray();
            foreach (var item in arr)
            {
                if (!Show(item.info, item.pos, idx++)) yield break;
                yield return new WaitForSeconds(displayDelay);
            }
        }

        public bool Show(Shape.Info info, Vector2Int gridPos, int idx)
        {
            var list = board.GetTilePositions(gridPos, info.GetTiles(), false);
            if (list != null)
            {
                foreach (var pos in list)
                {
                    var instance = Instantiate(prefabTile, pos, Quaternion.identity);
                    Destroy(instance, displayDelay + fadeDelay);
                    var caption = instance.GetComponentInChildren<TMPro.TextMeshPro>();
                    if (caption) caption.SetText($"{idx}");
                }
                return true;
            }
            else
            {
                Debug.LogError("Could not get hint position");
                return false;
            }
        }
    }

    public class Hint
    {
        public Shape.Info info;
        public Vector2Int pos;

        public Hint(Shape.Info info, Vector2Int pos)
        {
            this.info = info;
            this.pos = pos;
        }

        public bool Matches(Shape.Info info, Vector2Int pos)
        {
            return info.data == this.info.data
                && info.rotation == this.info.rotation
                && pos == this.pos;
        }
    }
}
