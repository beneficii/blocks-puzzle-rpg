using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] UnitData dataPlayer;
    public Unit prefabUnit;

    public Unit player { get; private set; }
    public Unit enemy { get; private set; }

    public List<Unit> summons;

    public Unit SpawnEnemy(UnitData data)
    {
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

    public Unit SpawnPlayer()
    {
        var unit = Instantiate(prefabUnit, spotPlayer);
        unit.Init(dataPlayer, Team.Ally);

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

    private void Start()
    {
        SpawnPlayer();
    }
}