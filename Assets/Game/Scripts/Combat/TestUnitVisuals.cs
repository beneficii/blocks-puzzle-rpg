using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Combat
{
    public class TestUnitVisuals : MonoBehaviour
    {
        [SerializeField] Unit prefab;

        [SerializeField] List<Unit> units;

        [EasyButtons.Button]
        public void SpawnUnits()
        {
            var visuals = Resources.LoadAll<UnitVisualData>(UnitCtrl.visualsFolder);

            Vector2 position = Vector2.zero;
            units = new List<Unit>();
            int idx = 0;
            foreach (var item in visuals)
            {
                idx++;
                //Debug.Log($"{idx}: {position}");
                
                var instance = Instantiate(prefab, position, Quaternion.identity, transform);
                instance.visuals = item;
                instance.LoadVisualData();
                units.Add(instance);

                instance.transform.position = position;
                if (idx % 7 == 0)
                {
                    position.y += 7;
                    position.x = 0;
                }
                else
                {
                    position.x += 7;
                    
                }
                
            }
        }

        void PlayAnimation(Unit.AnimType type)
        {
            foreach (var item in units)
            {
                if (item.visuals.frIddle == 0) continue;
                var animator = item.GetComponent<UnitAnimator>();
                animator.Play(type);
            }
        }

        private void Start()
        {
            foreach (var item in units)
            {
                if (item.visuals.frIddle == 0) continue;
                var animator = item.GetComponent<UnitAnimator>();
                animator.Init(item.visuals);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                PlayAnimation(Unit.AnimType.Attack1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                PlayAnimation(Unit.AnimType.Attack2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                PlayAnimation(Unit.AnimType.Hit);
            }
        }
#if UNITY_EDITOR
        [EasyButtons.Button]
        public void SaveAll()
        {
            foreach (var unit in units)
            {
                unit.SaveVisualData();
            }
        }
#endif
    }
}