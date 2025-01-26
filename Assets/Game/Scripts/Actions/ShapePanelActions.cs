using FancyToolkit;
using GridBoard;
using System.Collections;
using UnityEngine;
using TileShapes;

namespace GameActions
{
    public class AddTileToShape : ActionBase
    {
        int count;
        string tileId;

        public override IHasInfo GetExtraInfo() => GetData();
        TileData GetData() => TileCtrl.current.Get(tileId);

        public AddTileToShape(int count, string tileId)
        {
            this.count = count;
            this.tileId = tileId;
        }

        public override string GetDescription()
        {
            if (count == 1)
            {
                return $"Put '{GetData().title}' on next shape";
            }
            else
            {
                return $"Put {count} '{GetData().title}' on next shapes";
            }
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var shapePanel = Object.FindAnyObjectByType<ShapePanel>();
            if (!shapePanel)
            {
                Debug.LogError("Could not find shape panel!");
                yield break;
            }
            shapePanel.AddBonusTile(GetData(), count * multiplier);

            yield return null;
        }

        public class Builder : FactoryBuilder<ActionBase, int, string>
        {
            public override ActionBase Build() => new AddTileToShape(value, value2);
        }
    }
}