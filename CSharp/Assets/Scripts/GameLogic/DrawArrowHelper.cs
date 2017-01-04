namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class DrawArrowHelper
    {
        public static void Draw(Vector3 StartPos, Vector3 EndPos, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
        {
            Gizmos.DrawLine(StartPos, EndPos);
            Vector3 vector4 = EndPos - StartPos;
            Vector3 normalized = vector4.normalized;
            Vector3 vector2 = (Vector3) ((Quaternion.LookRotation(normalized) * Quaternion.Euler(0f, 180f + arrowHeadAngle, 0f)) * new Vector3(0f, 0f, 1f));
            Vector3 vector3 = (Vector3) ((Quaternion.LookRotation(normalized) * Quaternion.Euler(0f, 180f - arrowHeadAngle, 0f)) * new Vector3(0f, 0f, 1f));
            Gizmos.DrawRay(EndPos, (Vector3) (vector2 * arrowHeadLength));
            Gizmos.DrawRay(EndPos, (Vector3) (vector3 * arrowHeadLength));
        }

        public static void Draw(Vector3 StartPos, Vector3 EndPos, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
        {
            Gizmos.color = color;
            Draw(StartPos, EndPos, arrowHeadLength, arrowHeadAngle);
        }

        public static void DrawDebug(Vector3 StartPos, Vector3 EndPos, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
        {
            Debug.DrawLine(StartPos, EndPos);
            Vector3 vector4 = EndPos - StartPos;
            Vector3 normalized = vector4.normalized;
            Vector3 vector2 = (Vector3) ((Quaternion.LookRotation(normalized) * Quaternion.Euler(0f, 180f + arrowHeadAngle, 0f)) * new Vector3(0f, 0f, 1f));
            Vector3 vector3 = (Vector3) ((Quaternion.LookRotation(normalized) * Quaternion.Euler(0f, 180f - arrowHeadAngle, 0f)) * new Vector3(0f, 0f, 1f));
            Debug.DrawRay(EndPos, (Vector3) (vector2 * arrowHeadLength));
            Debug.DrawRay(EndPos, (Vector3) (vector3 * arrowHeadLength));
        }

        public static void DrawDebug(Vector3 StartPos, Vector3 EndPos, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
        {
            Debug.DrawLine(StartPos, EndPos, color);
            Vector3 vector4 = EndPos - StartPos;
            Vector3 normalized = vector4.normalized;
            Vector3 vector2 = (Vector3) ((Quaternion.LookRotation(normalized) * Quaternion.Euler(0f, 180f + arrowHeadAngle, 0f)) * new Vector3(0f, 0f, 1f));
            Vector3 vector3 = (Vector3) ((Quaternion.LookRotation(normalized) * Quaternion.Euler(0f, 180f - arrowHeadAngle, 0f)) * new Vector3(0f, 0f, 1f));
            Debug.DrawRay(EndPos, (Vector3) (vector2 * arrowHeadLength), color);
            Debug.DrawRay(EndPos, (Vector3) (vector3 * arrowHeadLength), color);
        }
    }
}

