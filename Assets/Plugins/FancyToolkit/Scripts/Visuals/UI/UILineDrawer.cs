using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FancyToolkit
{
    public class UILineDrawer : MonoBehaviour
    {
        [SerializeField] LineRenderer line;
        [SerializeField] float deltaMargin = 0.2f;
        [SerializeField] float lineWidth = 0.05f;

        Transform objectA;
        Transform objectB;

        public void Init(Transform a, Transform b)
        {
            objectA = a;
            objectB = b;
        }

        void Update()
        {
            var posA = objectA.transform.position;
            var posB = objectB.transform.position;
            var margin = (posA - posB).normalized * deltaMargin;
            Vector3[] pathPoints = { posA - margin, posB + margin };
            line.positionCount = pathPoints.Length;
            line.startWidth = line.endWidth = lineWidth;
            line.SetPositions(pathPoints);
        }
    }
}
