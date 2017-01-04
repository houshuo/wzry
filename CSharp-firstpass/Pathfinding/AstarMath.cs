namespace Pathfinding
{
    using System;
    using UnityEngine;

    public class AstarMath
    {
        public static int Abs(int a)
        {
            if (a < 0)
            {
                return -a;
            }
            return a;
        }

        public static float Abs(float a)
        {
            if (a < 0f)
            {
                return -a;
            }
            return a;
        }

        public static int Bit(int a, int b)
        {
            return ((a >> b) & 1);
        }

        public static int Clamp(int a, int b, int c)
        {
            return ((a <= c) ? ((a >= b) ? a : b) : c);
        }

        public static float Clamp(float a, float b, float c)
        {
            return ((a <= c) ? ((a >= b) ? a : b) : c);
        }

        public static int Clamp01(int a)
        {
            return ((a <= 1) ? ((a >= 0) ? a : 0) : 1);
        }

        public static float Clamp01(float a)
        {
            return ((a <= 1f) ? ((a >= 0f) ? a : 0f) : 1f);
        }

        public static int ComputeVertexHash(int x, int y, int z)
        {
            uint num = 0x8da6b343;
            uint num2 = 0xd8163841;
            uint num3 = 0xcb1ab31f;
            uint num4 = (uint) (((num * x) + (num2 * y)) + (num3 * z));
            return (((int) num4) & 0x3fffffff);
        }

        public static Vector3 CubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float num = 1f - t;
            return (Vector3) ((((((num * num) * num) * p0) + ((((3f * num) * num) * t) * p1)) + ((((3f * num) * t) * t) * p2)) + (((t * t) * t) * p3));
        }

        public static float DistancePointSegment(VInt3 a, VInt3 b, VInt3 p)
        {
            float num = b.x - a.x;
            float num2 = b.z - a.z;
            float num3 = p.x - a.x;
            float num4 = p.z - a.z;
            float num5 = (num * num) + (num2 * num2);
            float num6 = (num * num3) + (num2 * num4);
            if (num5 > 0f)
            {
                num6 /= num5;
            }
            if (num6 < 0f)
            {
                num6 = 0f;
            }
            else if (num6 > 1f)
            {
                num6 = 1f;
            }
            num3 = (a.x + (num6 * num)) - p.x;
            num4 = (a.z + (num6 * num2)) - p.z;
            return ((num3 * num3) + (num4 * num4));
        }

        public static float DistancePointSegment(int x, int z, int px, int pz, int qx, int qz)
        {
            float num = qx - px;
            float num2 = qz - pz;
            float num3 = x - px;
            float num4 = z - pz;
            float num5 = (num * num) + (num2 * num2);
            float num6 = (num * num3) + (num2 * num4);
            if (num5 > 0f)
            {
                num6 /= num5;
            }
            if (num6 < 0f)
            {
                num6 = 0f;
            }
            else if (num6 > 1f)
            {
                num6 = 1f;
            }
            num3 = (px + (num6 * num)) - x;
            num4 = (pz + (num6 * num2)) - z;
            return ((num3 * num3) + (num4 * num4));
        }

        public static float DistancePointSegment2(Vector3 a, Vector3 b, Vector3 p)
        {
            float num = b.x - a.x;
            float num2 = b.z - a.z;
            float num3 = Mathf.Abs((float) ((num * (p.z - a.z)) - ((p.x - a.x) * num2)));
            float f = (num * num) + (num2 * num2);
            if (f > 0f)
            {
                return (num3 / Mathf.Sqrt(f));
            }
            Vector3 vector = a - p;
            return vector.magnitude;
        }

        public static float DistancePointSegment2(int x, int z, int px, int pz, int qx, int qz)
        {
            Vector3 p = new Vector3((float) x, 0f, (float) z);
            Vector3 a = new Vector3((float) px, 0f, (float) pz);
            Vector3 b = new Vector3((float) qx, 0f, (float) qz);
            return DistancePointSegment2(a, b, p);
        }

        public static long DistancePointSegmentStrict(VInt3 a, VInt3 b, VInt3 p)
        {
            VInt3 num2 = NearestPointStrict(ref a, ref b, ref p) - p;
            return num2.sqrMagnitudeLong;
        }

        public static string FormatBytes(int bytes)
        {
            double num = (bytes < 0) ? -1.0 : 1.0;
            bytes = (bytes < 0) ? -bytes : bytes;
            if (bytes < 0x3e8)
            {
                double num2 = bytes * num;
                return (num2.ToString() + " bytes");
            }
            if (bytes < 0xf4240)
            {
                double num3 = (((double) bytes) / 1000.0) * num;
                return (num3.ToString("0.0") + " kb");
            }
            if (bytes < 0x3b9aca00)
            {
                double num4 = (((double) bytes) / 1000000.0) * num;
                return (num4.ToString("0.0") + " mb");
            }
            double num5 = (((double) bytes) / 1000000000.0) * num;
            return (num5.ToString("0.0") + " gb");
        }

        public static string FormatBytesBinary(int bytes)
        {
            double num = (bytes < 0) ? -1.0 : 1.0;
            bytes = (bytes < 0) ? -bytes : bytes;
            if (bytes < 0x400)
            {
                double num2 = bytes * num;
                return (num2.ToString() + " bytes");
            }
            if (bytes < 0x100000)
            {
                double num3 = (((double) bytes) / 1024.0) * num;
                return (num3.ToString("0.0") + " kb");
            }
            if (bytes < 0x40000000)
            {
                double num4 = (((double) bytes) / 1048576.0) * num;
                return (num4.ToString("0.0") + " mb");
            }
            double num5 = (((double) bytes) / 1073741824.0) * num;
            return (num5.ToString("0.0") + " gb");
        }

        public static float Hermite(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, (value * value) * (3f - (2f * value)));
        }

        public static Color IntToColor(int i, float a)
        {
            int num = (Bit(i, 1) + (Bit(i, 3) * 2)) + 1;
            int num2 = (Bit(i, 2) + (Bit(i, 4) * 2)) + 1;
            int num3 = (Bit(i, 0) + (Bit(i, 5) * 2)) + 1;
            return new Color(num * 0.25f, num2 * 0.25f, num3 * 0.25f, a);
        }

        public static float Lerp(float a, float b, float t)
        {
            return (a + ((b - a) * ((t <= 1f) ? ((t >= 0f) ? t : 0f) : 1f)));
        }

        public static float MagnitudeXZ(Vector3 a, Vector3 b)
        {
            Vector3 vector = a - b;
            return (float) Math.Sqrt((double) ((vector.x * vector.x) + (vector.z * vector.z)));
        }

        public static float MapTo(float startMin, float startMax, float value)
        {
            value -= startMin;
            value /= startMax - startMin;
            value = Mathf.Clamp01(value);
            return value;
        }

        public static float MapTo(float startMin, float startMax, float targetMin, float targetMax, float value)
        {
            value -= startMin;
            value /= startMax - startMin;
            value = Mathf.Clamp01(value);
            value *= targetMax - targetMin;
            value += targetMin;
            return value;
        }

        public static float MapToRange(float targetMin, float targetMax, float value)
        {
            value *= targetMax - targetMin;
            value += targetMin;
            return value;
        }

        public static int Max(int a, int b)
        {
            return ((a <= b) ? b : a);
        }

        public static float Max(float a, float b)
        {
            return ((a <= b) ? b : a);
        }

        public static ushort Max(ushort a, ushort b)
        {
            return ((a <= b) ? b : a);
        }

        public static uint Max(uint a, uint b)
        {
            return ((a <= b) ? b : a);
        }

        public static int Min(int a, int b)
        {
            return ((a >= b) ? b : a);
        }

        public static float Min(float a, float b)
        {
            return ((a >= b) ? b : a);
        }

        public static uint Min(uint a, uint b)
        {
            return ((a >= b) ? b : a);
        }

        public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            Vector3 rhs = Vector3.Normalize(lineEnd - lineStart);
            float num = Vector3.Dot(point - lineStart, rhs);
            return (lineStart + ((Vector3) (num * rhs)));
        }

        public static float NearestPointFactor(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            Vector3 rhs = lineEnd - lineStart;
            float magnitude = rhs.magnitude;
            rhs = (magnitude <= float.Epsilon) ? Vector3.zero : ((Vector3) (rhs / magnitude));
            return (Vector3.Dot(point - lineStart, rhs) / magnitude);
        }

        public static float NearestPointFactor(VInt2 lineStart, VInt2 lineEnd, VInt2 point)
        {
            VInt2 b = lineEnd - lineStart;
            double sqrMagnitudeLong = b.sqrMagnitudeLong;
            double num3 = VInt2.DotLong(point - lineStart, b);
            if (sqrMagnitudeLong != 0.0)
            {
                num3 /= sqrMagnitudeLong;
            }
            return (float) num3;
        }

        public static float NearestPointFactor(VInt3 lineStart, VInt3 lineEnd, VInt3 point)
        {
            VInt3 rhs = lineEnd - lineStart;
            double sqrMagnitude = rhs.sqrMagnitude;
            double num3 = VInt3.Dot(point - lineStart, rhs);
            if (sqrMagnitude != 0.0)
            {
                num3 /= sqrMagnitude;
            }
            return (float) num3;
        }

        public static VFactor NearestPointFactor(ref VInt3 lineStart, ref VInt3 lineEnd, ref VInt3 point)
        {
            VInt3 rhs = lineEnd - lineStart;
            long sqrMagnitudeLong = rhs.sqrMagnitudeLong;
            VFactor zero = VFactor.zero;
            zero.nom = VInt3.DotLong(point - lineStart, rhs);
            if (sqrMagnitudeLong != 0)
            {
                zero.den = sqrMagnitudeLong;
            }
            return zero;
        }

        public static float NearestPointFactorXZ(VInt3 lineStart, VInt3 lineEnd, VInt3 point)
        {
            VInt2 b = new VInt2(lineEnd.x - lineStart.x, lineEnd.z - lineStart.z);
            double sqrMagnitude = b.sqrMagnitude;
            VInt2 a = new VInt2(point.x - lineStart.x, point.z - lineStart.z);
            double num4 = VInt2.Dot(a, b);
            if (sqrMagnitude != 0.0)
            {
                num4 /= sqrMagnitude;
            }
            return (float) num4;
        }

        public static VFactor NearestPointFactorXZ(ref VInt3 lineStart, ref VInt3 lineEnd, ref VInt3 point)
        {
            VInt2 b = new VInt2(lineEnd.x - lineStart.x, lineEnd.z - lineStart.z);
            VInt2 a = new VInt2(point.x - lineStart.x, point.z - lineStart.z);
            long sqrMagnitudeLong = b.sqrMagnitudeLong;
            VFactor zero = VFactor.zero;
            zero.nom = VInt2.DotLong(a, b);
            if (sqrMagnitudeLong != 0)
            {
                zero.den = sqrMagnitudeLong;
            }
            return zero;
        }

        public static VInt3 NearestPointStrict(VInt3 lineStart, VInt3 lineEnd, VInt3 point)
        {
            return NearestPointStrict(ref lineStart, ref lineEnd, ref point);
        }

        public static VInt3 NearestPointStrict(ref VInt3 lineStart, ref VInt3 lineEnd, ref VInt3 point)
        {
            VInt3 rhs = lineEnd - lineStart;
            long sqrMagnitudeLong = rhs.sqrMagnitudeLong;
            if (sqrMagnitudeLong == 0)
            {
                return lineStart;
            }
            long m = IntMath.Clamp(VInt3.DotLong(point - lineStart, rhs), 0L, sqrMagnitudeLong);
            return (IntMath.Divide(rhs, m, sqrMagnitudeLong) + lineStart);
        }

        public static Vector3 NearestPointStrictFloat(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            Vector3 vector = lineEnd - lineStart;
            float magnitude = vector.magnitude;
            Vector3 rhs = (magnitude <= float.Epsilon) ? Vector3.zero : ((Vector3) (vector / magnitude));
            float num2 = Vector3.Dot(point - lineStart, rhs);
            return (lineStart + ((Vector3) (Mathf.Clamp(num2, 0f, magnitude) * rhs)));
        }

        public static Vector3 NearestPointStrictXZ(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            lineStart.y = point.y;
            lineEnd.y = point.y;
            Vector3 vector2 = lineEnd - lineStart;
            vector2.y = 0f;
            float magnitude = vector2.magnitude;
            Vector3 rhs = (magnitude <= float.Epsilon) ? Vector3.zero : ((Vector3) (vector2 / magnitude));
            float num2 = Vector3.Dot(point - lineStart, rhs);
            return (lineStart + ((Vector3) (Mathf.Clamp(num2, 0f, vector2.magnitude) * rhs)));
        }

        public static int Repeat(int i, int n)
        {
            while (i >= n)
            {
                i -= n;
            }
            return i;
        }

        public static int RoundToInt(double v)
        {
            return (int) (v + 0.5);
        }

        public static int RoundToInt(float v)
        {
            return (int) (v + 0.5f);
        }

        public static int Sign(int a)
        {
            return ((a >= 0) ? 1 : -1);
        }

        public static float Sign(float a)
        {
            return ((a >= 0f) ? 1f : -1f);
        }

        public static float SqrMagnitudeXZ(Vector3 a, Vector3 b)
        {
            Vector3 vector = a - b;
            return ((vector.x * vector.x) + (vector.z * vector.z));
        }
    }
}

