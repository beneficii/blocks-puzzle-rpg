using UnityEngine;

namespace RogueLikeMap
{
    public struct Edge
    {
        public Vector2Int a;
        public Vector2Int b;

        public Edge(Vector2Int a, Vector2Int b)
        {
            this.a = a;
            this.b = b;
        }

        public Edge(int ax, int ay, int bx, int by)
        {
            this.a = new Vector2Int(ax, ay);
            this.b = new Vector2Int(bx, by);
        }
    }
}
