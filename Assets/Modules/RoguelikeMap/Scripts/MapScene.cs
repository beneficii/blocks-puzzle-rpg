using System.Linq;
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

        [SerializeField] List<PositionData> positionData;

        MapInstance mapInstance;

        private void Start()
        {
            OnReady?.Invoke(this);
        }

        public void CreateMap(MapLayout layout, System.Random rng = null)
        {
            Clear();
            int maxHeight = layout.nodes.Max(n => n.pos.y);
            for (int i = 0; i < positionData.Count; i++)
            {
                var item = positionData[i];
                if (maxHeight < item.maxHeight || i == positionData.Count - 1)
                {
                    Vector3 pos = item.position;
                    pos.z = transform.localPosition.z;
                    transform.localPosition = pos;
                    break;
                }
            }

            mapInstance = Instantiate(prefabMapInstance, mapParent);
            mapInstance.Init(layout, rng);
        }

        public void Clear()
        {
            if (mapInstance) Destroy(mapInstance.gameObject);
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.U))
            {
                FindAnyObjectByType<MapInstance>().UnlockAllNodes();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                Game.current.GameOver();
                Destroy(Game.current.gameObject);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
#endif
        }

        [System.Serializable]
        public class PositionData
        {
            public int maxHeight;
            public Vector2 position;
        }
    }
}