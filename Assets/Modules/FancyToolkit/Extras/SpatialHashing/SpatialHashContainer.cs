using UnityEngine;

namespace FancyToolkit
{
    public partial class SpatialHashGrid<TObj>
    where TObj : MonoBehaviour
    {
        public class Container
        {
            Vector3 cachedPosition;
            public int index;
            public Cell cell;
            public SpatialHashGrid<TObj> grid;
            public TObj obj;

            public bool OutOfBounds => cell == null;

            public Container(SpatialHashGrid<TObj> grid, TObj obj)
            {
                this.grid = grid;
                this.index = -1;
                this.cell = null;
                this.obj = obj;
            }

            public void Update()
            {
                if (obj.transform.localPosition == cachedPosition) return;

                RefreshPosition();
            }

            public void RefreshPosition()
            {
                var newCell = grid.GetCell(obj.transform.localPosition);
                if (newCell == cell) return;
                cell?.Remove(this);

                cell = newCell;

                if (newCell == null)
                {
                    //UnityEngine.Debug.LogError("ToDo: Handle unit out of bounds");
                    return;
                }
                newCell.Add(this);
                cachedPosition = obj.transform.localPosition;
            }

            public void Remove()
            {
                cell?.Remove(this);
            }
        }
    }
}