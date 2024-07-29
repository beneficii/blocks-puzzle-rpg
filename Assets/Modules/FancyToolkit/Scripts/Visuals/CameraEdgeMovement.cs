using UnityEngine;

namespace FancyToolkit
{
    public class CameraEdgeMovement : MonoBehaviour
    {
        public float speed = 0.1f; // Speed of camera movement
        public Vector2 minPos; // Minimum position of the camera
        public Vector2 maxPos; // Maximum position of the camera

        void Update()
        {
            // Calculate the direction to move the camera based on the WASD keys
            Vector3 moveDir = new Vector3(0, 0, 0);
            if (Input.GetKey(KeyCode.W)) moveDir.y = 1;
            if (Input.GetKey(KeyCode.A)) moveDir.x = -1;
            if (Input.GetKey(KeyCode.S)) moveDir.y = -1;
            if (Input.GetKey(KeyCode.D)) moveDir.x = 1;

            // Move the camera in the calculated direction, within the specified bounds
            Vector3 newPos = transform.position + moveDir * speed;
            newPos.x = Mathf.Clamp(newPos.x, minPos.x, maxPos.x);
            newPos.y = Mathf.Clamp(newPos.y, minPos.y, maxPos.y);
            transform.position = newPos;
        }

        // Draw gizmos to visualize the bounds of camera movement
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(minPos.x, minPos.y, 0), new Vector3(maxPos.x, minPos.y, 0));
            Gizmos.DrawLine(new Vector3(maxPos.x, minPos.y, 0), new Vector3(maxPos.x, maxPos.y, 0));
            Gizmos.DrawLine(new Vector3(maxPos.x, maxPos.y, 0), new Vector3(minPos.x, maxPos.y, 0));
            Gizmos.DrawLine(new Vector3(minPos.x, maxPos.y, 0), new Vector3(minPos.x, minPos.y, 0));
        }
    }
}