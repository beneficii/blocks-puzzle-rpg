using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class LineMatchListener : MonoBehaviour
{
    [SerializeField] float bulletDelaySpacing = 0.1f;

    CombatArena arena;

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
        Unit.OnKilled += HandleUnitKilled;
    }

    private void OnDisable()
    {
        BtGrid.OnLinesCleared -= HandleLinesCleared;
        ShapePanel.OnShapesGenerated -= HandleShapesGenerated;
        Unit.OnKilled += HandleUnitKilled;
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
        switch (data.type)
        {
            case BtBlockType.Sword:
                CreateDamage(5, data, origin, arena.enemy, delay);
                break;
            case BtBlockType.Shield:
                CreateDamage(-2, data, origin, arena.player, delay);
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

    int upgradeOfferCtr = 0;
    void HandleUnitKilled(Unit unit)
    {
        if (unit == arena.enemy)
        {
            arena.SpawnEnemy();
            upgradeOfferCtr++;
            var rarity = upgradeOfferCtr % 5 == 0 ? BtUpgradeRarity.Rare : BtUpgradeRarity.Common;
            BtUpgradeCtrl.Show(rarity, 3);
        }
    }

    void HandleLinesCleared(List<BtBlock> blocks, int totalLines)
    {
        float delay = 0;
        arena.enemy.RemoveHp(totalLines);
        foreach (var block in blocks)
        {
            ExecuteAction(block.data, block.transform.position, delay);
            delay += bulletDelaySpacing;
        }
    }

    void HandleShapesGenerated(bool initial)
    {
        if (initial) return;

        
        arena.player.RemoveHp(arena.enemy.data.damage);
    }

    private void Awake()
    {
        arena = CombatArena.current;
    }
}