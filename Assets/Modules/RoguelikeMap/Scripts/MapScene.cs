using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueLikeMap;
using FancyToolkit;

namespace RogueLikeMap
{
    public class MapScene : MonoBehaviour
    {
        public static System.Action<MapScene> OnReady;

        [SerializeField] MapInstance prefabMapInstance;
        [SerializeField] Transform mapParent;

        MapInstance mapInstance;

        private void Start()
        {
            OnReady?.Invoke(this);
        }

        public void CreateMap(MapLayout layout, System.Random rng = null)
        {
            if (mapInstance) Destroy(mapInstance.gameObject);
            mapInstance = Instantiate(prefabMapInstance, mapParent);

            mapInstance.Init(layout, rng);
        }
    }
}