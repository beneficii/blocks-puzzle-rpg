using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace FancyToolkit
{
    public static class Helpers
    {
        static Camera _camera;
        public static Camera Camera
        {
            get
            {
                if (!_camera) _camera = Camera.main;
                return _camera;
            }
        }

        public static bool CanCaptureMouse()
        {
            return HasMouse() || Input.GetMouseButton(0);
        }

        // for mobile testing
        public static bool HasMouse()
        {
            return Input.mousePresent;
        }

        public static Vector2 MouseToWorldPosition()
        {
            return Camera.ScreenToWorldPoint(Input.mousePosition);
        }

        public static Vector2 ScreenToWorldPos(Vector2 pos)
        {
            return (Vector2)Camera.ScreenToWorldPoint(pos);
        }

        public static void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // requires "scren space - camera" Canvas
        public static bool IsMouseOverUI(RectTransform ui) =>
            RectTransformUtility.RectangleContainsScreenPoint(ui, Input.mousePosition, Camera);

        // requires "scren space - camera" Canvas
        public static bool IsPosOverUI(RectTransform ui, Vector2 position) =>
            RectTransformUtility.RectangleContainsScreenPoint(ui, position, Camera);


        public static TObj ClosestItem<TObj>(Vector2 worldPos, IEnumerable<TObj> items)
            where TObj : MonoBehaviour
        {
            if (items.Count() == 0) return null;

            float minDistanceSqr = float.PositiveInfinity;
            TObj result = null;

            foreach (var item in items)
            {
                float distanceSqr = (worldPos - (Vector2)item.transform.position).sqrMagnitude;
                if (distanceSqr < minDistanceSqr)
                {
                    minDistanceSqr = distanceSqr;
                    result = item;
                }
            }

            return result;
        }
    }
}