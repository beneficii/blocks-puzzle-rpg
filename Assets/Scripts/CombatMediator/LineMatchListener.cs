using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

public class LineMatchListener : MonoBehaviour
{
    //CooldownComponent animationCooldown;

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

    //Queue<MyAction> actionQueue = new Queue<MyAction>();

    private void Awake()
    {
        //animationCooldown = new CooldownComponent(0.15f);
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
        var bullet = DataManager.current.gameData.prefabBullet.MakeInstance(origin);
        bullet.Init(target, value);
        bullet.SetSprite(data.sprite);
        bullet.SetLaunchDelay(delay);
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
                break;
            default:
                break;
        }
    }

    void HandleLinesCleared(List<BtBlock> blocks, int totalLines)
    {
        //var arena = CombatArena.current;

        float delay = 0;
        foreach (var block in blocks)
        {
            ExecuteAction(block.data, block.transform.position, delay);
            delay += 0.1f;
            //if (block.data.type != BtBlockType.None) actionQueue.Enqueue(new MyAction(block.data, block.transform.position));
            /*
            switch (block.data.type)
            {
                case BtBlockType.Sword:
                    CreateDamage(3, block, arena.enemy);
                    break;
                case BtBlockType.Shield:
                    CreateDamage(-2, block, arena.hero);
                    break;
                case BtBlockType.Fire:
                    CreateDamage(10, block, arena.enemy);
                    break;
                default:
                    break;
            }
            */
        }
    }

    void HandleShapesGenerated(bool initial)
    {
        if (initial) return;

        CombatArena.current.hero.RemoveHp(5);
    }
    /*
    private void Update()
    {
        if (actionQueue.Count > 0 && animationCooldown.Use())
        {
            var action = actionQueue.Dequeue();

            ExecuteAction(action.data, action.origin);
        }
    }*/
}