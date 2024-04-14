using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class LineMatchListener : MonoBehaviour
{
    [SerializeField] float bulletDelaySpacing = 0.1f;

    struct MyAction
    {
        public BtBlockData data;
        public Vector2 origin;

        public MyAction(BtBlockData data, Vector2 origin)
        {
            this.data = data;
            this.origin = origin;
        }
    }

    private void OnEnable()
    {
        BtGrid.OnLinesCleared += HandleLinesCleared;
        ShapePanel.OnShapesGenerated += HandleShapesGenerated;
    }

    private void OnDisable()
    {
        BtGrid.OnLinesCleared -= HandleLinesCleared;
        ShapePanel.OnShapesGenerated -= HandleShapesGenerated;
    }

    void CreateDamage(int value, BtBlockData data, Vector2 origin, Unit target, float delay)
    {
        DataManager.current.gameData.prefabBullet.MakeInstance(origin)
            .SetTarget(target)
            .SetDamage(value)
            .SetSprite(data.sprite)
            .SetLaunchDelay(delay);
    }

    void ExecuteAction(BtBlockData data, Vector2 origin, float delay)
    {
        var arena = CombatArena.current;

        switch (data.type)
        {
            case BtBlockType.Sword:
                CreateDamage(3, data, origin, arena.enemy, delay);
                break;
            case BtBlockType.Shield:
                CreateDamage(-2, data, origin, arena.hero, delay);
                break;
            case BtBlockType.Fire:
                CreateDamage(10, data, origin, arena.enemy, delay);
                /*DataManager.current.gameData.prefabBullet.MakeInstance(origin)
                    .SetTarget(arena.enemy)
                    .SetAction((obj)=>
                    {
                        if (!obj.TryGetComponent<Unit>(out var unit)) return;
                        unit.SpecialTestAction();

                    })
                    .SetSprite(data.sprite)
                    .SetLaunchDelay(delay);
                */
                break;
            default:
                break;
        }
    }

    void HandleLinesCleared(List<BtBlock> blocks, int totalLines)
    {
        float delay = 0;
        foreach (var block in blocks)
        {
            ExecuteAction(block.data, block.transform.position, delay);
            delay += bulletDelaySpacing;
        }
    }

    void HandleShapesGenerated(bool initial)
    {
        if (initial) return;

        CombatArena.current.hero.RemoveHp(5);
    }
}