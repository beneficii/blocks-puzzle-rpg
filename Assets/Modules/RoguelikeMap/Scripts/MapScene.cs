using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueLikeMap;
using FancyToolkit;
using UnityEngine.SceneManagement;

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
            Clear();
            mapInstance = Instantiate(prefabMapInstance, mapParent);

            mapInstance.Init(layout, rng);
        }

        public void Clear()
        {
            if (mapInstance) Destroy(mapInstance.gameObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                FindAnyObjectByType<MapInstance>().UnlockAllNodes();
            }
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.R))
            {
                Game.current.GameOver();
                Destroy(Game.current.gameObject);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
#endif
        }
    }
}