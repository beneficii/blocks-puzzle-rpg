using FancyToolkit;
using GridBoard;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace GameActions
{
    public class SpawnTile : ActionBase
    {
        int count;
        string tileId;

        TileData GetData() => TileCtrl.current.Get(tileId);

        public SpawnTile(int count, string tileId)
        {
            this.count = count;
            this.tileId = tileId;
        }

        public override string GetDescription()
        {
            if (count == 1)
            {
                return $"Spawn '{GetData().title}'";
            }
            else
            {
                return $"Spawn {count} '{GetData().title}'";
            }
        }

        void Spawn(MyTile tile)
        {
            tile.SetBoard(parent.board);
            tile.Init(GetData());
            tile.isActionLocked = true;
        }

        public override IEnumerator Run(int multiplier = 1)
        {
            var data = GetData();
            if (data == null)
            {
                Debug.LogError($"Data with id `{tileId}` not found!");
                yield break;
            }

            for (int i = 0; i < count * multiplier; i++)
            {
                var target = parent.board.TakeEmptyTile();
                //if (!target) Debug.Log("no empty tiles");
                if (!target) yield break;

                MakeBullet(parent)
                    .SetTarget(target)
                    .SetSprite(data.sprite)
                    .SetTileAction(Spawn);

                yield return new WaitForSeconds(.05f);
            }
            yield return new WaitForSeconds(.1f);
        }

        public class Builder : FactoryBuilder<ActionBase, int, string>
        {
            public override ActionBase Build() => new SpawnTile(value, value2);
        }
    }
}