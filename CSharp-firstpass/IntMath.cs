using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class IntMath
{
    public static VFactor acos(long nom, long den)
    {
        int num = ((int) Divide((long) (nom * AcosLookupTable.HALF_COUNT), den)) + AcosLookupTable.HALF_COUNT;
        num = Mathf.Clamp(num, 0, AcosLookupTable.COUNT);
        return new VFactor { nom = AcosLookupTable.table[num], den = 0x2710L };
    }

    public static VFactor atan2(int y, int x)
    {
        int num;
        int num2;
        if (x < 0)
        {
            if (y < 0)
            {
                x = -x;
                y = -y;
                num2 = 1;
            }
            else
            {
                x = -x;
                num2 = -1;
            }
            num = -31416;
        }
        else
        {
            if (y < 0)
            {
                y = -y;
                num2 = -1;
            }
            else
            {
                num2 = 1;
            }
            num = 0;
        }
        int dIM = Atan2LookupTable.DIM;
        long num4 = dIM - 1;
        long b = (x >= y) ? ((long) x) : ((long) y);
        int num6 = (int) Divide((long) (x * num4), b);
        int num7 = (int) Divide((long) (y * num4), b);
        int num8 = Atan2LookupTable.table[(num7 * dIM) + num6];
        return new VFactor { nom = (num8 + num) * num2, den = 0x2710L };
    }

    public static int CeilPowerOfTwo(int x)
    {
        x--;
        x |= x >> 1;
        x |= x >> 2;
        x |= x >> 4;
        x |= x >> 8;
        x |= x >> 0x10;
        x++;
        return x;
    }

    public static long Clamp(long a, long min, long max)
    {
        if (a < min)
        {
            return min;
        }
        if (a > max)
        {
            return max;
        }
        return a;
    }

    public static VFactor cos(long nom, long den)
    {
        int index = SinCosLookupTable.getIndex(nom, den);
        return new VFactor((long) SinCosLookupTable.cos_table[index], (long) SinCosLookupTable.FACTOR);
    }

    public static int Divide(int a, int b)
    {
        int num = ((a ^ b) & -2147483648) >> 0x1f;
        int num2 = (num * -2) + 1;
        return ((a + ((b / 2) * num2)) / b);
    }

    public static long Divide(long a, long b)
    {
        long num = ((a ^ b) & -9223372036854775808L) >> 0x3f;
        long num2 = (num * -2L) + 1L;
        return ((a + ((b / 2L) * num2)) / b);
    }

    public static VInt2 Divide(VInt2 a, long b)
    {
        a.x = (int) Divide((long) a.x, b);
        a.y = (int) Divide((long) a.y, b);
        return a;
    }

    public static VInt3 Divide(VInt3 a, int b)
    {
        a.x = Divide(a.x, b);
        a.y = Divide(a.y, b);
        a.z = Divide(a.z, b);
        return a;
    }

    public static VInt3 Divide(VInt3 a, long b)
    {
        a.x = (int) Divide((long) a.x, b);
        a.y = (int) Divide((long) a.y, b);
        a.z = (int) Divide((long) a.z, b);
        return a;
    }

    public static VInt2 Divide(VInt2 a, long m, long b)
    {
        a.x = (int) Divide((long) (a.x * m), b);
        a.y = (int) Divide((long) (a.y * m), b);
        return a;
    }

    public static VInt3 Divide(VInt3 a, long m, long b)
    {
        a.x = (int) Divide((long) (a.x * m), b);
        a.y = (int) Divide((long) (a.y * m), b);
        a.z = (int) Divide((long) (a.z * m), b);
        return a;
    }

    public static bool IntersectSegment(ref VInt2 seg1Src, ref VInt2 seg1Vec, ref VInt2 seg2Src, ref VInt2 seg2Vec, out VInt2 interPoint)
    {
        long num;
        long num2;
        long num3;
        long num4;
        long num5;
        long num6;
        SegvecToLinegen(ref seg1Src, ref seg1Vec, out num, out num2, out num3);
        SegvecToLinegen(ref seg2Src, ref seg2Vec, out num4, out num5, out num6);
        long b = (num * num5) - (num4 * num2);
        if (b != 0)
        {
            long x = Divide((long) ((num2 * num6) - (num5 * num3)), b);
            long y = Divide((long) ((num4 * num3) - (num * num6)), b);
            bool flag = IsPointOnSegment(ref seg1Src, ref seg1Vec, x, y) && IsPointOnSegment(ref seg2Src, ref seg2Vec, x, y);
            interPoint.x = (int) x;
            interPoint.y = (int) y;
            return flag;
        }
        interPoint = VInt2.zero;
        return false;
    }

    private static bool IsPointOnSegment(ref VInt2 segSrc, ref VInt2 segVec, long x, long y)
    {
        long num = x - segSrc.x;
        long num2 = y - segSrc.y;
        return ((((segVec.x * num) + (segVec.y * num2)) >= 0L) && (((num * num) + (num2 * num2)) <= segVec.sqrMagnitudeLong));
    }

    public static bool IsPowerOfTwo(int x)
    {
        return ((x & (x - 1)) == 0);
    }

    public static int Lerp(int src, int dest, int nom, int den)
    {
        return Divide((int) ((src * den) + ((dest - src) * nom)), den);
    }

    public static long Lerp(long src, long dest, long nom, long den)
    {
        return Divide((long) ((src * den) + ((dest - src) * nom)), den);
    }

    public static long Max(long a, long b)
    {
        return ((a <= b) ? b : a);
    }

    public static bool PointInPolygon(ref VInt2 pnt, VInt2[] plg)
    {
        if ((plg == null) || (plg.Length < 3))
        {
            return false;
        }
        bool flag = false;
        int index = 0;
        for (int i = plg.Length - 1; index < plg.Length; i = index++)
        {
            VInt2 num3 = plg[index];
            VInt2 num4 = plg[i];
            if (((num3.y <= pnt.y) && (pnt.y < num4.y)) || ((num4.y <= pnt.y) && (pnt.y < num3.y)))
            {
                int num5 = num4.y - num3.y;
                long num6 = ((pnt.y - num3.y) * (num4.x - num3.x)) - ((pnt.x - num3.x) * num5);
                if (num5 > 0)
                {
                    if (num6 > 0L)
                    {
                        flag = !flag;
                    }
                }
                else if (num6 < 0L)
                {
                    flag = !flag;
                }
            }
        }
        return flag;
    }

    public static bool SegIntersectPlg(ref VInt2 segSrc, ref VInt2 segVec, VInt2[] plg, out VInt2 nearPoint, out VInt2 projectVec)
    {
        nearPoint = VInt2.zero;
        projectVec = VInt2.zero;
        if ((plg == null) || (plg.Length < 2))
        {
            return false;
        }
        bool flag = false;
        long num2 = -1L;
        int index = -1;
        for (int i = 0; i < plg.Length; i++)
        {
            VInt2 num;
            VInt2 num5 = plg[(i + 1) % plg.Length] - plg[i];
            if (IntersectSegment(ref segSrc, ref segVec, ref plg[i], ref num5, out num))
            {
                VInt2 num11 = num - segSrc;
                long sqrMagnitudeLong = num11.sqrMagnitudeLong;
                if ((num2 < 0L) || (sqrMagnitudeLong < num2))
                {
                    nearPoint = num;
                    num2 = sqrMagnitudeLong;
                    index = i;
                    flag = true;
                }
            }
        }
        if (index >= 0)
        {
            VInt2 num7 = plg[(index + 1) % plg.Length] - plg[index];
            VInt2 num8 = (segSrc + segVec) - nearPoint;
            long num9 = (num8.x * num7.x) + (num8.y * num7.y);
            if (num9 < 0L)
            {
                num9 = -num9;
                num7 = -num7;
            }
            long b = num7.sqrMagnitudeLong;
            projectVec.x = (int) Divide((long) (num7.x * num9), b);
            projectVec.y = (int) Divide((long) (num7.y * num9), b);
        }
        return flag;
    }

    public static void SegvecToLinegen(ref VInt2 segSrc, ref VInt2 segVec, out long a, out long b, out long c)
    {
        a = segVec.y;
        b = -segVec.x;
        c = (segVec.x * segSrc.y) - (segSrc.x * segVec.y);
    }

    public static VFactor sin(long nom, long den)
    {
        int index = SinCosLookupTable.getIndex(nom, den);
        return new VFactor((long) SinCosLookupTable.sin_table[index], (long) SinCosLookupTable.FACTOR);
    }

    public static void sincos(out VFactor s, out VFactor c, VFactor angle)
    {
        int index = SinCosLookupTable.getIndex(angle.nom, angle.den);
        s = new VFactor((long) SinCosLookupTable.sin_table[index], (long) SinCosLookupTable.FACTOR);
        c = new VFactor((long) SinCosLookupTable.cos_table[index], (long) SinCosLookupTable.FACTOR);
    }

    public static void sincos(out VFactor s, out VFactor c, long nom, long den)
    {
        int index = SinCosLookupTable.getIndex(nom, den);
        s = new VFactor((long) SinCosLookupTable.sin_table[index], (long) SinCosLookupTable.FACTOR);
        c = new VFactor((long) SinCosLookupTable.cos_table[index], (long) SinCosLookupTable.FACTOR);
    }

    public static int Sqrt(long a)
    {
        if (a <= 0L)
        {
            return 0;
        }
        if (a <= 0xffffffffL)
        {
            return (int) Sqrt32((uint) a);
        }
        return (int) Sqrt64((ulong) a);
    }

    public static uint Sqrt32(uint a)
    {
        uint num = 0;
        uint num2 = 0;
        for (int i = 0; i < 0x10; i++)
        {
            num2 = num2 << 1;
            num = num << 2;
            num += a >> 30;
            a = a << 2;
            if (num2 < num)
            {
                num2++;
                num -= num2;
                num2++;
            }
        }
        return ((num2 >> 1) & 0xffff);
    }

    public static ulong Sqrt64(ulong a)
    {
        ulong num = 0L;
        ulong num2 = 0L;
        for (int i = 0; i < 0x20; i++)
        {
            num2 = num2 << 1;
            num = num << 2;
            num += a >> 0x3e;
            a = a << 2;
            if (num2 < num)
            {
                num2 += (ulong) 1L;
                num -= num2;
                num2 += (ulong) 1L;
            }
        }
        return ((num2 >> 1) & 0xffffffffL);
    }

    public static long SqrtLong(long a)
    {
        if (a <= 0L)
        {
            return 0L;
        }
        if (a <= 0xffffffffL)
        {
            return (long) Sqrt32((uint) a);
        }
        return (long) Sqrt64((ulong) a);
    }

    public static VInt3 Transform(ref VInt3 point, ref VInt3 forward, ref VInt3 trans)
    {
        VInt3 up = VInt3.up;
        VInt3 num2 = VInt3.Cross(VInt3.up, forward);
        return Transform(ref point, ref num2, ref up, ref forward, ref trans);
    }

    public static VInt3 Transform(VInt3 point, VInt3 forward, VInt3 trans)
    {
        VInt3 up = VInt3.up;
        VInt3 num2 = VInt3.Cross(VInt3.up, forward);
        return Transform(ref point, ref num2, ref up, ref forward, ref trans);
    }

    public static VInt3 Transform(VInt3 point, VInt3 forward, VInt3 trans, VInt3 scale)
    {
        VInt3 up = VInt3.up;
        VInt3 num2 = VInt3.Cross(VInt3.up, forward);
        return Transform(ref point, ref num2, ref up, ref forward, ref trans, ref scale);
    }

    public static VInt3 Transform(ref VInt3 point, ref VInt3 axis_x, ref VInt3 axis_y, ref VInt3 axis_z, ref VInt3 trans)
    {
        return new VInt3(Divide((int) (((axis_x.x * point.x) + (axis_y.x * point.y)) + (axis_z.x * point.z)), 0x3e8) + trans.x, Divide((int) (((axis_x.y * point.x) + (axis_y.y * point.y)) + (axis_z.y * point.z)), 0x3e8) + trans.y, Divide((int) (((axis_x.z * point.x) + (axis_y.z * point.y)) + (axis_z.z * point.z)), 0x3e8) + trans.z);
    }

    public static VInt3 Transform(VInt3 point, ref VInt3 axis_x, ref VInt3 axis_y, ref VInt3 axis_z, ref VInt3 trans)
    {
        return new VInt3(Divide((int) (((axis_x.x * point.x) + (axis_y.x * point.y)) + (axis_z.x * point.z)), 0x3e8) + trans.x, Divide((int) (((axis_x.y * point.x) + (axis_y.y * point.y)) + (axis_z.y * point.z)), 0x3e8) + trans.y, Divide((int) (((axis_x.z * point.x) + (axis_y.z * point.y)) + (axis_z.z * point.z)), 0x3e8) + trans.z);
    }

    public static VInt3 Transform(ref VInt3 point, ref VInt3 axis_x, ref VInt3 axis_y, ref VInt3 axis_z, ref VInt3 trans, ref VInt3 scale)
    {
        long num = point.x * scale.x;
        long num2 = point.y * scale.x;
        long num3 = point.z * scale.x;
        return new VInt3(((int) Divide((long) (((axis_x.x * num) + (axis_y.x * num2)) + (axis_z.x * num3)), (long) 0xf4240L)) + trans.x, ((int) Divide((long) (((axis_x.y * num) + (axis_y.y * num2)) + (axis_z.y * num3)), (long) 0xf4240L)) + trans.y, ((int) Divide((long) (((axis_x.z * num) + (axis_y.z * num2)) + (axis_z.z * num3)), (long) 0xf4240L)) + trans.z);
    }
}

