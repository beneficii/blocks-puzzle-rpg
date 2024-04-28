using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapCtrl : MonoBehaviour
{
    static MapCtrl _current;
    public static MapCtrl current
    {
        get
        {
            if (!_current)
            {
                _current = FindFirstObjectByType<MapCtrl>();
            }

            return _current;
        }
    }

    [SerializeField] List<UnitData> units;

    [SerializeField] Color colorPointPassed = Color.gray;
    [SerializeField] Color colorPointCurrent = Color.green;
    [SerializeField] Color colorPointFuture = Color.gray;

    List<MapPoint> points;

    int mapIdx = 0;

    private void Awake()
    {
        points = GetComponentsInChildren<MapPoint>()
            .ToList();

        for (int i = 0; i < points.Count; i++)
        {
            if (i < units.Count)
            {
                points[i].unitData = units[i];
            }
        }
    }

    void RefreshPoints()
    {
        for (int i = 0; i < points.Count; i++)
        {
            MapPoint point = points[i];
            var img = point.GetComponent<Image>();
            if (i < mapIdx)
            {
                img.color = colorPointPassed;
            }
            else if (i == mapIdx)
            {
                img.color = colorPointCurrent;
            }
            else
            {
                img.color = colorPointFuture;
            }
        }
    }

    public MapPoint Next()
    {
        if (mapIdx < points.Count)
        {
            var point = points[mapIdx];
            RefreshPoints();
            mapIdx++;

            return point;
        }

        return null;
    }
}