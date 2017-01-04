using System;
using System.Runtime.InteropServices;

[Serializable, StructLayout(LayoutKind.Sequential)]
public struct VFactor
{
    public long nom;
    public long den;
    [NonSerialized]
    public static VFactor zero;
    [NonSerialized]
    public static VFactor one;
    [NonSerialized]
    public static VFactor pi;
    [NonSerialized]
    public static VFactor twoPi;
    private static long mask_;
    private static long upper_;
    public VFactor(long n, long d)
    {
        this.nom = n;
        this.den = d;
    }

    static VFactor()
    {
        zero = new VFactor(0L, 1L);
        one = new VFactor(1L, 1L);
        pi = new VFactor(0x7ab8L, 0x2710L);
        twoPi = new VFactor(0xf570L, 0x2710L);
        mask_ = 0x7fffffffffffffffL;
        upper_ = 0xffffffL;
    }

    public int roundInt
    {
        get
        {
            return (int) IntMath.Divide(this.nom, this.den);
        }
    }
    public int integer
    {
        get
        {
            return (int) (this.nom / this.den);
        }
    }
    public float single
    {
        get
        {
            double num = ((double) this.nom) / ((double) this.den);
            return (float) num;
        }
    }
    public bool IsPositive
    {
        get
        {
            DebugHelper.Assert(this.den != 0L, "VFactor: denominator is zero !");
            if (this.nom == 0)
            {
                return false;
            }
            bool flag = this.nom > 0L;
            bool flag2 = this.den > 0L;
            return !(flag ^ flag2);
        }
    }
    public bool IsNegative
    {
        get
        {
            DebugHelper.Assert(this.den != 0L, "VFactor: denominator is zero !");
            if (this.nom == 0)
            {
                return false;
            }
            bool flag = this.nom > 0L;
            bool flag2 = this.den > 0L;
            return (flag ^ flag2);
        }
    }
    public bool IsZero
    {
        get
        {
            return (this.nom == 0L);
        }
    }
    public override bool Equals(object obj)
    {
        return (((obj != null) && (base.GetType() == obj.GetType())) && (this == ((VFactor) obj)));
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public VFactor Inverse
    {
        get
        {
            return new VFactor(this.den, this.nom);
        }
    }
    public override string ToString()
    {
        return this.single.ToString();
    }

    public void strip()
    {
        while (((this.nom & mask_) > upper_) && ((this.den & mask_) > upper_))
        {
            this.nom = this.nom >> 1;
            this.den = this.den >> 1;
        }
    }

    public static bool operator <(VFactor a, VFactor b)
    {
        long num = a.nom * b.den;
        long num2 = b.nom * a.den;
        return (!((b.den > 0L) ^ (a.den > 0L)) ? (num < num2) : (num > num2));
    }

    public static bool operator >(VFactor a, VFactor b)
    {
        long num = a.nom * b.den;
        long num2 = b.nom * a.den;
        return (!((b.den > 0L) ^ (a.den > 0L)) ? (num > num2) : (num < num2));
    }

    public static bool operator <=(VFactor a, VFactor b)
    {
        long num = a.nom * b.den;
        long num2 = b.nom * a.den;
        return (!((b.den > 0L) ^ (a.den > 0L)) ? (num <= num2) : (num >= num2));
    }

    public static bool operator >=(VFactor a, VFactor b)
    {
        long num = a.nom * b.den;
        long num2 = b.nom * a.den;
        return (!((b.den > 0L) ^ (a.den > 0L)) ? (num >= num2) : (num <= num2));
    }

    public static bool operator ==(VFactor a, VFactor b)
    {
        return ((a.nom * b.den) == (b.nom * a.den));
    }

    public static bool operator !=(VFactor a, VFactor b)
    {
        return ((a.nom * b.den) != (b.nom * a.den));
    }

    public static bool operator <(VFactor a, long b)
    {
        long nom = a.nom;
        long num2 = b * a.den;
        return ((a.den <= 0L) ? (nom > num2) : (nom < num2));
    }

    public static bool operator >(VFactor a, long b)
    {
        long nom = a.nom;
        long num2 = b * a.den;
        return ((a.den <= 0L) ? (nom < num2) : (nom > num2));
    }

    public static bool operator <=(VFactor a, long b)
    {
        long nom = a.nom;
        long num2 = b * a.den;
        return ((a.den <= 0L) ? (nom >= num2) : (nom <= num2));
    }

    public static bool operator >=(VFactor a, long b)
    {
        long nom = a.nom;
        long num2 = b * a.den;
        return ((a.den <= 0L) ? (nom <= num2) : (nom >= num2));
    }

    public static bool operator ==(VFactor a, long b)
    {
        return (a.nom == (b * a.den));
    }

    public static bool operator !=(VFactor a, long b)
    {
        return (a.nom != (b * a.den));
    }

    public static VFactor operator +(VFactor a, VFactor b)
    {
        return new VFactor { nom = (a.nom * b.den) + (b.nom * a.den), den = a.den * b.den };
    }

    public static VFactor operator +(VFactor a, long b)
    {
        a.nom += b * a.den;
        return a;
    }

    public static VFactor operator -(VFactor a, VFactor b)
    {
        return new VFactor { nom = (a.nom * b.den) - (b.nom * a.den), den = a.den * b.den };
    }

    public static VFactor operator -(VFactor a, long b)
    {
        a.nom -= b * a.den;
        return a;
    }

    public static VFactor operator *(VFactor a, long b)
    {
        a.nom *= b;
        return a;
    }

    public static VFactor operator /(VFactor a, long b)
    {
        a.den *= b;
        return a;
    }

    public static VInt3 operator *(VInt3 v, VFactor f)
    {
        return IntMath.Divide(v, f.nom, f.den);
    }

    public static VInt2 operator *(VInt2 v, VFactor f)
    {
        return IntMath.Divide(v, f.nom, f.den);
    }

    public static VInt3 operator /(VInt3 v, VFactor f)
    {
        return IntMath.Divide(v, f.den, f.nom);
    }

    public static VInt2 operator /(VInt2 v, VFactor f)
    {
        return IntMath.Divide(v, f.den, f.nom);
    }

    public static int operator *(int i, VFactor f)
    {
        return (int) IntMath.Divide((long) (i * f.nom), f.den);
    }

    public static VFactor operator -(VFactor a)
    {
        a.nom = -a.nom;
        return a;
    }
}

