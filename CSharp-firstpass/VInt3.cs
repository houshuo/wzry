using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable, StructLayout(LayoutKind.Sequential)]
public struct VInt3
{
    public const int Precision = 0x3e8;
    public const float FloatPrecision = 1000f;
    public const float PrecisionFactor = 0.001f;
    public int x;
    public int y;
    public int z;
    public static readonly VInt3 zero;
    public static readonly VInt3 one;
    public static readonly VInt3 half;
    public static readonly VInt3 forward;
    public static readonly VInt3 up;
    public static readonly VInt3 right;
    public VInt3(Vector3 position)
    {
        this.x = (int) Math.Round((double) (position.x * 1000f));
        this.y = (int) Math.Round((double) (position.y * 1000f));
        this.z = (int) Math.Round((double) (position.z * 1000f));
    }

    public VInt3(int _x, int _y, int _z)
    {
        this.x = _x;
        this.y = _y;
        this.z = _z;
    }

    static VInt3()
    {
        zero = new VInt3(0, 0, 0);
        one = new VInt3(0x3e8, 0x3e8, 0x3e8);
        half = new VInt3(500, 500, 500);
        forward = new VInt3(0, 0, 0x3e8);
        up = new VInt3(0, 0x3e8, 0);
        right = new VInt3(0x3e8, 0, 0);
    }

    public VInt3 DivBy2()
    {
        this.x = this.x >> 1;
        this.y = this.y >> 1;
        this.z = this.z >> 1;
        return this;
    }

    public int this[int i]
    {
        get
        {
            return ((i != 0) ? ((i != 1) ? this.z : this.y) : this.x);
        }
        set
        {
            if (i == 0)
            {
                this.x = value;
            }
            else if (i == 1)
            {
                this.y = value;
            }
            else
            {
                this.z = value;
            }
        }
    }
    public static float Angle(VInt3 lhs, VInt3 rhs)
    {
        double d = ((double) Dot(lhs, rhs)) / (lhs.magnitude * rhs.magnitude);
        d = (d >= -1.0) ? ((d <= 1.0) ? d : 1.0) : -1.0;
        return (float) Math.Acos(d);
    }

    public static VFactor AngleInt(VInt3 lhs, VInt3 rhs)
    {
        long den = lhs.magnitude * rhs.magnitude;
        return IntMath.acos((long) Dot(ref lhs, ref rhs), den);
    }

    public static int Dot(ref VInt3 lhs, ref VInt3 rhs)
    {
        return (((lhs.x * rhs.x) + (lhs.y * rhs.y)) + (lhs.z * rhs.z));
    }

    public static int Dot(VInt3 lhs, VInt3 rhs)
    {
        return (((lhs.x * rhs.x) + (lhs.y * rhs.y)) + (lhs.z * rhs.z));
    }

    public static long DotLong(VInt3 lhs, VInt3 rhs)
    {
        return (((lhs.x * rhs.x) + (lhs.y * rhs.y)) + (lhs.z * rhs.z));
    }

    public static long DotLong(ref VInt3 lhs, ref VInt3 rhs)
    {
        return (((lhs.x * rhs.x) + (lhs.y * rhs.y)) + (lhs.z * rhs.z));
    }

    public static long DotXZLong(ref VInt3 lhs, ref VInt3 rhs)
    {
        return ((lhs.x * rhs.x) + (lhs.z * rhs.z));
    }

    public static long DotXZLong(VInt3 lhs, VInt3 rhs)
    {
        return ((lhs.x * rhs.x) + (lhs.z * rhs.z));
    }

    public static VInt3 Cross(ref VInt3 lhs, ref VInt3 rhs)
    {
        return new VInt3(IntMath.Divide((int) ((lhs.y * rhs.z) - (lhs.z * rhs.y)), 0x3e8), IntMath.Divide((int) ((lhs.z * rhs.x) - (lhs.x * rhs.z)), 0x3e8), IntMath.Divide((int) ((lhs.x * rhs.y) - (lhs.y * rhs.x)), 0x3e8));
    }

    public static VInt3 Cross(VInt3 lhs, VInt3 rhs)
    {
        return new VInt3(IntMath.Divide((int) ((lhs.y * rhs.z) - (lhs.z * rhs.y)), 0x3e8), IntMath.Divide((int) ((lhs.z * rhs.x) - (lhs.x * rhs.z)), 0x3e8), IntMath.Divide((int) ((lhs.x * rhs.y) - (lhs.y * rhs.x)), 0x3e8));
    }

    public static VInt3 MoveTowards(VInt3 from, VInt3 to, int dt)
    {
        VInt3 num2 = to - from;
        if (num2.sqrMagnitudeLong <= (dt * dt))
        {
            return to;
        }
        VInt3 num = to - from;
        return (from + num.NormalizeTo(dt));
    }

    public VInt3 Normal2D()
    {
        return new VInt3(this.z, this.y, -this.x);
    }

    public VInt3 NormalizeTo(int newMagn)
    {
        long num = this.x * 100;
        long num2 = this.y * 100;
        long num3 = this.z * 100;
        long a = ((num * num) + (num2 * num2)) + (num3 * num3);
        if (a != 0)
        {
            long b = IntMath.Sqrt(a);
            long num6 = newMagn;
            this.x = (int) IntMath.Divide((long) (num * num6), b);
            this.y = (int) IntMath.Divide((long) (num2 * num6), b);
            this.z = (int) IntMath.Divide((long) (num3 * num6), b);
        }
        return this;
    }

    public long Normalize()
    {
        long num = this.x << 7;
        long num2 = this.y << 7;
        long num3 = this.z << 7;
        long a = ((num * num) + (num2 * num2)) + (num3 * num3);
        if (a == 0)
        {
            return 0L;
        }
        long b = IntMath.Sqrt(a);
        long num6 = 0x3e8L;
        this.x = (int) IntMath.Divide((long) (num * num6), b);
        this.y = (int) IntMath.Divide((long) (num2 * num6), b);
        this.z = (int) IntMath.Divide((long) (num3 * num6), b);
        return (b >> 7);
    }

    public Vector3 vec3
    {
        get
        {
            return new Vector3(this.x * 0.001f, this.y * 0.001f, this.z * 0.001f);
        }
    }
    public VInt2 xz
    {
        get
        {
            return new VInt2(this.x, this.z);
        }
    }
    public int magnitude
    {
        get
        {
            long x = this.x;
            long y = this.y;
            long z = this.z;
            return IntMath.Sqrt(((x * x) + (y * y)) + (z * z));
        }
    }
    public int magnitude2D
    {
        get
        {
            long x = this.x;
            long z = this.z;
            return IntMath.Sqrt((x * x) + (z * z));
        }
    }
    public VInt3 RotateY(ref VFactor radians)
    {
        VInt3 num;
        VFactor factor;
        VFactor factor2;
        IntMath.sincos(out factor, out factor2, radians.nom, radians.den);
        long num2 = factor2.nom * factor.den;
        long num3 = factor2.den * factor.nom;
        long b = factor2.den * factor.den;
        num.x = (int) IntMath.Divide((long) ((this.x * num2) + (this.z * num3)), b);
        num.z = (int) IntMath.Divide((long) ((-this.x * num3) + (this.z * num2)), b);
        num.y = 0;
        return num.NormalizeTo(0x3e8);
    }

    public VInt3 RotateY(int degree)
    {
        VInt3 num;
        VFactor factor;
        VFactor factor2;
        IntMath.sincos(out factor, out factor2, (long) (0x7ab8 * degree), 0x1b7740L);
        long num2 = factor2.nom * factor.den;
        long num3 = factor2.den * factor.nom;
        long b = factor2.den * factor.den;
        num.x = (int) IntMath.Divide((long) ((this.x * num2) + (this.z * num3)), b);
        num.z = (int) IntMath.Divide((long) ((-this.x * num3) + (this.z * num2)), b);
        num.y = 0;
        return num.NormalizeTo(0x3e8);
    }

    public int costMagnitude
    {
        get
        {
            return this.magnitude;
        }
    }
    public float worldMagnitude
    {
        get
        {
            double x = this.x;
            double y = this.y;
            double z = this.z;
            return (((float) Math.Sqrt(((x * x) + (y * y)) + (z * z))) * 0.001f);
        }
    }
    public double sqrMagnitude
    {
        get
        {
            double x = this.x;
            double y = this.y;
            double z = this.z;
            return (((x * x) + (y * y)) + (z * z));
        }
    }
    public long sqrMagnitudeLong
    {
        get
        {
            long x = this.x;
            long y = this.y;
            long z = this.z;
            return (((x * x) + (y * y)) + (z * z));
        }
    }
    public long sqrMagnitudeLong2D
    {
        get
        {
            long x = this.x;
            long z = this.z;
            return ((x * x) + (z * z));
        }
    }
    public int unsafeSqrMagnitude
    {
        get
        {
            return (((this.x * this.x) + (this.y * this.y)) + (this.z * this.z));
        }
    }
    public VInt3 abs
    {
        get
        {
            return new VInt3(Math.Abs(this.x), Math.Abs(this.y), Math.Abs(this.z));
        }
    }
    [Obsolete("Same implementation as .magnitude")]
    public float safeMagnitude
    {
        get
        {
            double x = this.x;
            double y = this.y;
            double z = this.z;
            return (float) Math.Sqrt(((x * x) + (y * y)) + (z * z));
        }
    }
    [Obsolete(".sqrMagnitude is now per default safe (.unsafeSqrMagnitude can be used for unsafe operations)")]
    public float safeSqrMagnitude
    {
        get
        {
            float num = this.x * 0.001f;
            float num2 = this.y * 0.001f;
            float num3 = this.z * 0.001f;
            return (((num * num) + (num2 * num2)) + (num3 * num3));
        }
    }
    public override string ToString()
    {
        object[] objArray1 = new object[] { "( ", this.x, ", ", this.y, ", ", this.z, ")" };
        return string.Concat(objArray1);
    }

    public override bool Equals(object o)
    {
        if (o == null)
        {
            return false;
        }
        VInt3 num = (VInt3) o;
        return (((this.x == num.x) && (this.y == num.y)) && (this.z == num.z));
    }

    public override int GetHashCode()
    {
        return (((this.x * 0x466f45d) ^ (this.y * 0x127409f)) ^ (this.z * 0x4f9ffb7));
    }

    public static VInt3 Lerp(VInt3 a, VInt3 b, float f)
    {
        return new VInt3(Mathf.RoundToInt(a.x * (1f - f)) + Mathf.RoundToInt(b.x * f), Mathf.RoundToInt(a.y * (1f - f)) + Mathf.RoundToInt(b.y * f), Mathf.RoundToInt(a.z * (1f - f)) + Mathf.RoundToInt(b.z * f));
    }

    public static VInt3 Lerp(VInt3 a, VInt3 b, VFactor f)
    {
        return new VInt3(((int) IntMath.Divide((long) ((b.x - a.x) * f.nom), f.den)) + a.x, ((int) IntMath.Divide((long) ((b.y - a.y) * f.nom), f.den)) + a.y, ((int) IntMath.Divide((long) ((b.z - a.z) * f.nom), f.den)) + a.z);
    }

    public static VInt3 Lerp(VInt3 a, VInt3 b, int factorNom, int factorDen)
    {
        return new VInt3(IntMath.Divide((int) ((b.x - a.x) * factorNom), factorDen) + a.x, IntMath.Divide((int) ((b.y - a.y) * factorNom), factorDen) + a.y, IntMath.Divide((int) ((b.z - a.z) * factorNom), factorDen) + a.z);
    }

    public long XZSqrMagnitude(VInt3 rhs)
    {
        long num = this.x - rhs.x;
        long num2 = this.z - rhs.z;
        return ((num * num) + (num2 * num2));
    }

    public long XZSqrMagnitude(ref VInt3 rhs)
    {
        long num = this.x - rhs.x;
        long num2 = this.z - rhs.z;
        return ((num * num) + (num2 * num2));
    }

    public bool IsEqualXZ(VInt3 rhs)
    {
        return ((this.x == rhs.x) && (this.z == rhs.z));
    }

    public bool IsEqualXZ(ref VInt3 rhs)
    {
        return ((this.x == rhs.x) && (this.z == rhs.z));
    }

    public static bool operator ==(VInt3 lhs, VInt3 rhs)
    {
        return (((lhs.x == rhs.x) && (lhs.y == rhs.y)) && (lhs.z == rhs.z));
    }

    public static bool operator !=(VInt3 lhs, VInt3 rhs)
    {
        return (((lhs.x != rhs.x) || (lhs.y != rhs.y)) || (lhs.z != rhs.z));
    }

    public static explicit operator VInt3(Vector3 ob)
    {
        return new VInt3((int) Math.Round((double) (ob.x * 1000f)), (int) Math.Round((double) (ob.y * 1000f)), (int) Math.Round((double) (ob.z * 1000f)));
    }

    public static explicit operator Vector3(VInt3 ob)
    {
        return new Vector3(ob.x * 0.001f, ob.y * 0.001f, ob.z * 0.001f);
    }

    public static VInt3 operator -(VInt3 lhs, VInt3 rhs)
    {
        lhs.x -= rhs.x;
        lhs.y -= rhs.y;
        lhs.z -= rhs.z;
        return lhs;
    }

    public static VInt3 operator -(VInt3 lhs)
    {
        lhs.x = -lhs.x;
        lhs.y = -lhs.y;
        lhs.z = -lhs.z;
        return lhs;
    }

    public static VInt3 operator +(VInt3 lhs, VInt3 rhs)
    {
        lhs.x += rhs.x;
        lhs.y += rhs.y;
        lhs.z += rhs.z;
        return lhs;
    }

    public static VInt3 operator *(VInt3 lhs, int rhs)
    {
        lhs.x *= rhs;
        lhs.y *= rhs;
        lhs.z *= rhs;
        return lhs;
    }

    public static VInt3 operator *(VInt3 lhs, float rhs)
    {
        lhs.x = (int) Math.Round((double) (lhs.x * rhs));
        lhs.y = (int) Math.Round((double) (lhs.y * rhs));
        lhs.z = (int) Math.Round((double) (lhs.z * rhs));
        return lhs;
    }

    public static VInt3 operator *(VInt3 lhs, double rhs)
    {
        lhs.x = (int) Math.Round((double) (lhs.x * rhs));
        lhs.y = (int) Math.Round((double) (lhs.y * rhs));
        lhs.z = (int) Math.Round((double) (lhs.z * rhs));
        return lhs;
    }

    public static VInt3 operator *(VInt3 lhs, Vector3 rhs)
    {
        lhs.x = (int) Math.Round((double) (lhs.x * rhs.x));
        lhs.y = (int) Math.Round((double) (lhs.y * rhs.y));
        lhs.z = (int) Math.Round((double) (lhs.z * rhs.z));
        return lhs;
    }

    public static VInt3 operator *(VInt3 lhs, VInt3 rhs)
    {
        lhs.x *= rhs.x;
        lhs.y *= rhs.y;
        lhs.z *= rhs.z;
        return lhs;
    }

    public static VInt3 operator /(VInt3 lhs, float rhs)
    {
        lhs.x = (int) Math.Round((double) (((float) lhs.x) / rhs));
        lhs.y = (int) Math.Round((double) (((float) lhs.y) / rhs));
        lhs.z = (int) Math.Round((double) (((float) lhs.z) / rhs));
        return lhs;
    }

    public static implicit operator string(VInt3 ob)
    {
        return ob.ToString();
    }
}

