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
    public Unit prefabUnit;

    public Unit player { get; private set; }
    public Unit enemy { get; private set; }

    public List<Unit> summons;

    int level = 1;
    public Unit SpawnEnemy()
    {
        var data = new UnitData
        {
            sprite = DataManager.current.gameData.spriteTempEnemy,
            damage = level * 2,
            hp = level * 10,
        };
        level++;

        var unit = Instantiate(prefabUnit, spotEnemy);
        unit.Init(data);
        enemy = unit;

        return unit;
    }

    public Unit SpawnPlayer()
    {
        var data = new UnitData
        {
            sprite = DataManager.current.gameData.spriteTempHero,
            damage = 1,
            hp = 100,
        };

        var unit = Instantiate(prefabUnit, spotPlayer);
        unit.Init(data);

        player = unit;

        return unit;
    }

    private void Start()
    {
        SpawnPlayer();
        SpawnEnemy();
    }
}