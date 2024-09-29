using System.Collections;
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
    [SerializeField] UnitData dataPlayer;
    public Unit prefabUnit;

    public int startingPlayerHealth { get; set; }

    public Unit player { get; private set; }
    public Unit enemy { get; private set; }

    public List<Unit> summons;

    public Unit SpawnEnemy(UnitData data)
    {
        Assert.IsNotNull(data);
        //var unit = Instantiate(data.visuals.obj, spotEnemy);
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
        => SpawnEnemy(UnitCtrl.current.GetUnit(id));

    public Unit SpawnPlayer()
    {
        var data = dataPlayer;//UnitCtrl.current.GetUnit("player");
        var unit = Instantiate(prefabUnit, spotPlayer);
        //var unit = Instantiate(data.visuals.obj, spotPlayer);
        unit.Init(data, Team.Ally);
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

    private void Start()
    {
        SpawnPlayer();
    }
}