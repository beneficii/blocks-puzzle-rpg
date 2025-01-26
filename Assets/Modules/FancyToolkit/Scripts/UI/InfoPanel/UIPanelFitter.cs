using System.Collections;
using UnityEngine;

namespace FancyToolkit
{
    public class UIPanelFitter : MonoBehaviour
    {
        [SerializeField] float offsetRadius = 1;
        [SerializeField] RectTransform area;
        [SerializeField] RectTransform panel;

        Vector2 cachedDirection = Vector2.up;

        void Update()
        {
            PositionPanel();
        }

        static Vector2 DetermineDirection(Vector2 vector)
        {
            Vector2 normalized = vector.normalized;

            return normalized.y > 0 ? Vector2.up : Vector2.down;

            //if (Mathf.Abs(normalized.x) > Mathf.Abs(normalized.y))
            //{
            //    return normalized.x > 0 ? Vector2.right : Vector2.left;
            //}
            //else
            //{
            //    return normalized.y > 0 ? Vector2.up : Vector2.down;
            //}
        }


        public void PositionPanel()
        {
            Vector2 localMousePosition = area.InverseTransformPoint(Helpers.MouseToWorldPosition());

            var dCenter = area.rect.center - localMousePosition;
            // prevent jaggy side change when close to center
            if (Mathf.Abs(dCenter.x) > 5 && Mathf.Abs(dCenter.y) > 5)
            {
                cachedDirection = DetermineDirection(dCenter);

                var anchor = (Vector2.one - cachedDirection) / 2;
                panel.anchorMin = anchor;
                panel.anchorMax = anchor;
                panel.pivot = anchor;
            }
            

            Vector2 panelPosition = localMousePosition + (cachedDirection * offsetRadius);
            panel.position = area.TransformPoint(panelPosition); // new Vector3(worldPanelPosition.x, worldPanelPosition.y, panel.position.z);
            panel.ClampInsideParent(area);

        }


        //private void OnDrawGizmos()
        //{
        //    // Draw area in green
        //    Gizmos.color = Color.green;
        //    if (area != null)
        //    {
        //        DrawRectTransform(area);
        //    }

        //    // Draw panel in red
        //    Gizmos.color = Color.red;
        //    if (panel != null)
        //    {
        //        DrawRectTransform(panel);
        //    }
        //}

        //private void DrawRectTransform(RectTransform rectTransform)
        //{
        //    Vector3[] corners = new Vector3[4];
        //    rectTransform.GetWorldCorners(corners);

        //    // Draw lines between the four corners
        //    for (int i = 0; i < 4; i++)
        //    {
        //        Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
        //    }
        //}
    }
}