using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileShapes
{
    public class ShapeManager : MonoBehaviour
    {
        static ShapeManager _current;
        public static ShapeManager current
        {
            get
            {
                if (!_current)
                {
                    _current = FindFirstObjectByType<ShapeManager>();
                    _current.Init();
                }

                return _current;
            }
        }

        [SerializeField] DatabaseShapes database;

        public List<ShapeData> shapes { get; private set; }

        void Init()
        {
            shapes = database.GenerateShapes();
        }
    }
}
