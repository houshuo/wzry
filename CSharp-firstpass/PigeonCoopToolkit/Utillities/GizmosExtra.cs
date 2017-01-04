namespace PigeonCoopToolkit.Utillities
{
    using System;
    using UnityEngine;

    public static class GizmosExtra
    {
        public static void GizmosDrawArrow(Vector3 from, Vector3 to, float arrowSize)
        {
            Gizmos.DrawLine(from, to);
            Vector3 vector = to - from;
            vector = (Vector3) (vector.normalized * arrowSize);
            Gizmos.DrawLine(to, to - (Quaternion.Euler(0f, 0f, 45f) * vector));
            Gizmos.DrawLine(to, to - (Quaternion.Euler(0f, 0f, -45f) * vector));
        }

        public static void GizmosDrawCircle(Vector3 position, Vector3 up, float size, int divisions)
        {
            Vector3 vector = (Vector3) (Quaternion.Euler(90f, 0f, 0f) * (up * size));
            for (int i = 0; i < divisions; i++)
            {
                Vector3 vector2 = (Vector3) (Quaternion.AngleAxis(360f / ((float) divisions), up) * vector);
                Gizmos.DrawLine(position + vector, position + vector2);
                vector = vector2;
            }
        }
    }
}

