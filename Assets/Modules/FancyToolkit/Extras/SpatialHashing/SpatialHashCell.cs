using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FancyToolkit
{
    public partial class SpatialHashGrid<TObj>
    where TObj : MonoBehaviour
    {
        public class Cell
        {
            public readonly SpatialHashGrid<TObj> grid;
            public readonly int x, y;
            public readonly Vector2Int pos;
            public readonly Vector2 worldPos;
            public Vector2Int size;
            List<Container> items = new();

            public IReadOnlyList<Container> Items => items;

            public Cell(SpatialHashGrid<TObj> grid, int x, int y, Vector2Int size, Vector2 worldPos)
            {
                this.grid = grid;
                this.x = x;
                this.y = y;
                this.pos = new Vector2Int(x, y);
                this.size = size;
                this.items = new();
                this.worldPos = worldPos;
            }

            public TObj GetFirst(Predicate<TObj> func = null)
            {
                if (items.Count == 0) return null;

                if (func == null) return items[0].obj;

                foreach (var item in items)
                {
                    var obj = item.obj;
                    if (func(obj)) return obj;
                }

                return null;
            }

            public void Add(Container item)
            {
                item.cell = this;
                item.index = items.Count;

                items.Add(item);
            }

            public void Remove(Container item)
            {
                if (item.cell != this) return;

                int idxToRemove = item.index;
                int lastIdx = items.Count - 1;

                if (idxToRemove != lastIdx)
                {
                    // Swap the item with the last element in the list.
                    items[idxToRemove] = items[lastIdx];
                    items[idxToRemove].index = idxToRemove;
                }

                // Remove the item from the list.
                items.RemoveAt(lastIdx);

                // Clear the reference to the parent cell and cell index of the removed item.
                item.cell = null;
                item.index = -1;
            }
#if UNITY_EDITOR
            public void DrawGizmo(Transform parent)
            {
                var style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.alignment = TextAnchor.MiddleCenter;

                Vector3 localPos = parent.TransformPoint(worldPos);

                Handles.Label(localPos + new Vector3(0, 2), $"{items.Count}", style);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(localPos, new Vector3(size.x, size.y, 1) * parent.lossyScale.x);
            }
#endif
        }
    }
}