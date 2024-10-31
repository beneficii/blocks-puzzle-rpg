using FancyToolkit;
using GridBoard;
using System.Collections;
using UnityEngine;
using TileActions;

namespace SkillActions
{
    public class SpawnTile : SkillActionBase
    {
        int count;
        string tileId;

        TileData GetData() => TileCtrl.current.GetTile(tileId);

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

        public override IEnumerator Run()
        {
            var data = GetData();
            if (data == null)
            {
                Debug.LogError($"Data with id `{tileId}` not found!");
                yield break;
            }

            for (int i = 0; i < count; i++)
            {
                var target = parent.board.TakeEmptyTile();
                if (!target) yield break;

                MakeBullet()
                    .SetTarget(target)
                    .SetSprite(data.visuals.sprite)
                    .SetTileAction(Spawn);

                yield return new WaitForSeconds(.05f);
            }
        }

        public class Builder : FactoryBuilder<SkillActionBase, int, string>
        {
            public override SkillActionBase Build() => new SpawnTile(value, value2);
        }

    }

}