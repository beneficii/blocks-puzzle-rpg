﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CombatArena : MonoBehaviour
{
    static CombatArena _current;
    public static CombatArena current
    {
        get
        {
            if (!_current) 
            {
                _current = FindFirstObjectByType<CombatArena>();
            }

            return _current;
        }
    }

    [SerializeField] Transform spotPlayer;
    [SerializeField] Transform spotEnemy;
    public Unit prefabUnit;

    public int startingPlayerHealth { get; set; }

    public Unit player { get; private set; }
    public Unit enemy { get; private set; }

    public List<Unit> summons;

    public Unit SpawnEnemy(UnitData data)
    {
        Assert.IsNotNull(data);
        var unit = Instantiate(prefabUnit, spotEnemy);
        unit.Init(data, Team.Enemy);
        enemy = unit;

        if (player)
        {
            player.SetTarget(enemy);
            enemy.SetTarget(player);
        }

        return unit;
    }

    public Unit SpawnEnemy(string id)
        => SpawnEnemy(UnitCtrl.current.Get(id));

    public Unit SpawnPlayer()
    {
        var unit = Instantiate(prefabUnit, spotPlayer);
        unit.Init(UnitCtrl.current.Get("mage"), Team.Ally);
        if (startingPlayerHealth > 0)
        {
            unit.SetHp(startingPlayerHealth);
        }

        player = unit;

        if (enemy)
        {
            player.SetTarget(enemy);
            enemy.SetTarget(player);
        }

        return unit;
    }

    public IEnumerable<Unit> GetUnits()
    {
        if (player) yield return player;
        if (enemy) yield return enemy;
    }

    public IEnumerable<Unit> GetEnemies()
    {
        if (enemy) yield return enemy;
    }
}