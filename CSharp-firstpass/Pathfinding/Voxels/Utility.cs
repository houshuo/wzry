namespace Pathfinding.Voxels
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class Utility
    {
        public static float additiveTimer;
        private static float[] clipPolygonCache = new float[0x15];
        private static int[] clipPolygonIntCache = new int[0x15];
        public static Color[] colors = new Color[] { Color.green, Color.blue, Color.red, Color.yellow, Color.cyan, Color.white, Color.black };
        public static float lastAdditiveTimerStart;
        public static float lastStartTime;

        public static int Bit(int a, int b)
        {
            return ((a & (1 << (b & 0x1f))) >> b);
        }

        public static int ClipPoly(float[] vIn, int n, float[] vOut, float pnx, float pnz, float pd)
        {
            float[] clipPolygonCache = Utility.clipPolygonCache;
            for (int i = 0; i < n; i++)
            {
                clipPolygonCache[i] = ((pnx * vIn[i * 3]) + (pnz * vIn[(i * 3) + 2])) + pd;
            }
            int num2 = 0;
            int index = 0;
            int num4 = n - 1;
            while (index < n)
            {
                bool flag = clipPolygonCache[num4] >= 0f;
                bool flag2 = clipPolygonCache[index] >= 0f;
                if (flag != flag2)
                {
                    float num5 = clipPolygonCache[num4] / (clipPolygonCache[num4] - clipPolygonCache[index]);
                    vOut[num2 * 3] = vIn[num4 * 3] + ((vIn[index * 3] - vIn[num4 * 3]) * num5);
                    vOut[(num2 * 3) + 1] = vIn[(num4 * 3) + 1] + ((vIn[(index * 3) + 1] - vIn[(num4 * 3) + 1]) * num5);
                    vOut[(num2 * 3) + 2] = vIn[(num4 * 3) + 2] + ((vIn[(index * 3) + 2] - vIn[(num4 * 3) + 2]) * num5);
                    num2++;
                }
                if (flag2)
                {
                    vOut[num2 * 3] = vIn[index * 3];
                    vOut[(num2 * 3) + 1] = vIn[(index * 3) + 1];
                    vOut[(num2 * 3) + 2] = vIn[(index * 3) + 2];
                    num2++;
                }
                num4 = index;
                index++;
            }
            return num2;
        }

        public static int ClipPolygon(float[] vIn, int n, float[] vOut, float multi, float offset, int axis)
        {
            float[] clipPolygonCache = Utility.clipPolygonCache;
            for (int i = 0; i < n; i++)
            {
                clipPolygonCache[i] = (multi * vIn[(i * 3) + axis]) + offset;
            }
            int num2 = 0;
            int index = 0;
            int num4 = n - 1;
            while (index < n)
            {
                bool flag = clipPolygonCache[num4] >= 0f;
                bool flag2 = clipPolygonCache[index] >= 0f;
                if (flag != flag2)
                {
                    int num5 = num2 * 3;
                    int num6 = index * 3;
                    int num7 = num4 * 3;
                    float num8 = clipPolygonCache[num4] / (clipPolygonCache[num4] - clipPolygonCache[index]);
                    vOut[num5] = vIn[num7] + ((vIn[num6] - vIn[num7]) * num8);
                    vOut[num5 + 1] = vIn[num7 + 1] + ((vIn[num6 + 1] - vIn[num7 + 1]) * num8);
                    vOut[num5 + 2] = vIn[num7 + 2] + ((vIn[num6 + 2] - vIn[num7 + 2]) * num8);
                    num2++;
                }
                if (flag2)
                {
                    int num9 = num2 * 3;
                    int num10 = index * 3;
                    vOut[num9] = vIn[num10];
                    vOut[num9 + 1] = vIn[num10 + 1];
                    vOut[num9 + 2] = vIn[num10 + 2];
                    num2++;
                }
                num4 = index;
                index++;
            }
            return num2;
        }

        public static int ClipPolygon(Vector3[] vIn, int n, Vector3[] vOut, float multi, float offset, int axis)
        {
            float[] clipPolygonCache = Utility.clipPolygonCache;
            for (int i = 0; i < n; i++)
            {
                clipPolygonCache[i] = (multi * vIn[i][axis]) + offset;
            }
            int index = 0;
            int num3 = 0;
            int num4 = n - 1;
            while (num3 < n)
            {
                bool flag = clipPolygonCache[num4] >= 0f;
                bool flag2 = clipPolygonCache[num3] >= 0f;
                if (flag != flag2)
                {
                    float num5 = clipPolygonCache[num4] / (clipPolygonCache[num4] - clipPolygonCache[num3]);
                    vOut[index] = vIn[num4] + ((Vector3) ((vIn[num3] - vIn[num4]) * num5));
                    index++;
                }
                if (flag2)
                {
                    vOut[index] = vIn[num3];
                    index++;
                }
                num4 = num3;
                num3++;
            }
            return index;
        }

        public static int ClipPolygon(VInt3[] vIn, int n, VInt3[] vOut, int multi, int offset, int axis)
        {
            int[] clipPolygonIntCache = Utility.clipPolygonIntCache;
            for (int i = 0; i < n; i++)
            {
                clipPolygonIntCache[i] = (multi * vIn[i][axis]) + offset;
            }
            int index = 0;
            int num3 = 0;
            int num4 = n - 1;
            while (num3 < n)
            {
                bool flag = clipPolygonIntCache[num4] >= 0;
                bool flag2 = clipPolygonIntCache[num3] >= 0;
                if (flag != flag2)
                {
                    double num5 = ((double) clipPolygonIntCache[num4]) / ((double) (clipPolygonIntCache[num4] - clipPolygonIntCache[num3]));
                    vOut[index] = vIn[num4] + ((VInt3) ((vIn[num3] - vIn[num4]) * num5));
                    index++;
                }
                if (flag2)
                {
                    vOut[index] = vIn[num3];
                    index++;
                }
                num4 = num3;
                num3++;
            }
            return index;
        }

        public static int ClipPolygonY(float[] vIn, int n, float[] vOut, float multi, float offset, int axis)
        {
            float[] clipPolygonCache = Utility.clipPolygonCache;
            for (int i = 0; i < n; i++)
            {
                clipPolygonCache[i] = (multi * vIn[(i * 3) + axis]) + offset;
            }
            int num2 = 0;
            int index = 0;
            int num4 = n - 1;
            while (index < n)
            {
                bool flag = clipPolygonCache[num4] >= 0f;
                bool flag2 = clipPolygonCache[index] >= 0f;
                if (flag != flag2)
                {
                    vOut[(num2 * 3) + 1] = vIn[(num4 * 3) + 1] + ((vIn[(index * 3) + 1] - vIn[(num4 * 3) + 1]) * (clipPolygonCache[num4] / (clipPolygonCache[num4] - clipPolygonCache[index])));
                    num2++;
                }
                if (flag2)
                {
                    vOut[(num2 * 3) + 1] = vIn[(index * 3) + 1];
                    num2++;
                }
                num4 = index;
                index++;
            }
            return num2;
        }

        public static void CopyVector(float[] a, int i, Vector3 v)
        {
            a[i] = v.x;
            a[i + 1] = v.y;
            a[i + 2] = v.z;
        }

        public static void EndTimer(string label)
        {
            Debug.Log(label + ", process took " + ToMillis(Time.realtimeSinceStartup - lastStartTime) + "ms to complete");
        }

        public static void EndTimerAdditive(string label, bool log)
        {
            additiveTimer += Time.realtimeSinceStartup - lastAdditiveTimerStart;
            if (log)
            {
                Debug.Log(label + ", process took " + ToMillis(additiveTimer) + "ms to complete");
            }
            lastAdditiveTimerStart = Time.realtimeSinceStartup;
        }

        public static Color GetColor(int i)
        {
            while (i >= colors.Length)
            {
                i -= colors.Length;
            }
            while (i < 0)
            {
                i += colors.Length;
            }
            return colors[i];
        }

        public static bool IntersectXAxis(out Vector3 intersection, Vector3 start1, Vector3 dir1, float x)
        {
            float num = dir1.x;
            if (num == 0f)
            {
                intersection = Vector3.zero;
                return false;
            }
            float num2 = x - start1.x;
            float num3 = num2 / num;
            num3 = Mathf.Clamp01(num3);
            intersection = start1 + ((Vector3) (dir1 * num3));
            return true;
        }

        public static bool IntersectZAxis(out Vector3 intersection, Vector3 start1, Vector3 dir1, float z)
        {
            float num = -dir1.z;
            if (num == 0f)
            {
                intersection = Vector3.zero;
                return false;
            }
            float num2 = start1.z - z;
            float num3 = num2 / num;
            num3 = Mathf.Clamp01(num3);
            intersection = start1 + ((Vector3) (dir1 * num3));
            return true;
        }

        public static Color IntToColor(int i, float a)
        {
            int num = (Bit(i, 1) + (Bit(i, 3) * 2)) + 1;
            int num2 = (Bit(i, 2) + (Bit(i, 4) * 2)) + 1;
            int num3 = (Bit(i, 0) + (Bit(i, 5) * 2)) + 1;
            return new Color(num * 0.25f, num2 * 0.25f, num3 * 0.25f, a);
        }

        public static float Max(float a, float b, float c)
        {
            a = (a <= b) ? b : a;
            return ((a <= c) ? c : a);
        }

        public static int Max(int a, int b, int c, int d)
        {
            a = (a <= b) ? b : a;
            a = (a <= c) ? c : a;
            return ((a <= d) ? d : a);
        }

        public static float Max(float a, float b, float c, float d)
        {
            a = (a <= b) ? b : a;
            a = (a <= c) ? c : a;
            return ((a <= d) ? d : a);
        }

        public static float Min(float a, float b, float c)
        {
            a = (a >= b) ? b : a;
            return ((a >= c) ? c : a);
        }

        public static int Min(int a, int b, int c, int d)
        {
            a = (a >= b) ? b : a;
            a = (a >= c) ? c : a;
            return ((a >= d) ? d : a);
        }

        public static float Min(float a, float b, float c, float d)
        {
            a = (a >= b) ? b : a;
            a = (a >= c) ? c : a;
            return ((a >= d) ? d : a);
        }

        public static void StartTimer()
        {
            lastStartTime = Time.realtimeSinceStartup;
        }

        public static void StartTimerAdditive(bool reset)
        {
            if (reset)
            {
                additiveTimer = 0f;
            }
            lastAdditiveTimerStart = Time.realtimeSinceStartup;
        }

        public static string ToMillis(float v)
        {
            float num = v * 1000f;
            return num.ToString("0");
        }

        public static float TriangleArea(Vector3 a, Vector3 b, Vector3 c)
        {
            return (((b.x - a.x) * (c.z - a.z)) - ((c.x - a.x) * (b.z - a.z)));
        }

        public static float TriangleArea2(Vector3 a, Vector3 b, Vector3 c)
        {
            return Mathf.Abs((float) ((((((a.x * b.z) + (b.x * c.z)) + (c.x * a.z)) - (a.x * c.z)) - (c.x * b.z)) - (b.x * a.z)));
        }
    }
}

