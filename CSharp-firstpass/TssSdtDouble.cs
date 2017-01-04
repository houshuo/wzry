using System;

public class TssSdtDouble
{
    private TssSdtDoubleSlot m_slot;

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return this.GetValue().GetHashCode();
    }

    private double GetValue()
    {
        if (this.m_slot == null)
        {
            this.m_slot = TssSdtDoubleSlot.NewSlot(null);
        }
        return this.m_slot.GetValue();
    }

    public static TssSdtDouble NewTssSdtDouble()
    {
        return new TssSdtDouble { m_slot = TssSdtDoubleSlot.NewSlot(null) };
    }

    public static TssSdtDouble operator --(TssSdtDouble v)
    {
        TssSdtDouble num = new TssSdtDouble();
        if (v == null)
        {
            num.SetValue(-1.0);
            return num;
        }
        double num2 = v.GetValue() - 1.0;
        num.SetValue(num2);
        return num;
    }

    public static bool operator ==(TssSdtDouble a, TssSdtDouble b)
    {
        return ((object.Equals(a, null) && object.Equals(b, null)) || ((!object.Equals(a, null) && !object.Equals(b, null)) && (a.GetValue() == b.GetValue())));
    }

    public static implicit operator TssSdtDouble(double v)
    {
        TssSdtDouble num = new TssSdtDouble();
        num.SetValue(v);
        return num;
    }

    public static implicit operator double(TssSdtDouble v)
    {
        if (v == null)
        {
            return 0.0;
        }
        return v.GetValue();
    }

    public static TssSdtDouble operator ++(TssSdtDouble v)
    {
        TssSdtDouble num = new TssSdtDouble();
        if (v == null)
        {
            num.SetValue(1.0);
            return num;
        }
        double num2 = v.GetValue() + 1.0;
        num.SetValue(num2);
        return num;
    }

    public static bool operator !=(TssSdtDouble a, TssSdtDouble b)
    {
        if (object.Equals(a, null) && object.Equals(b, null))
        {
            return false;
        }
        if (!object.Equals(a, null) && !object.Equals(b, null))
        {
            return (a.GetValue() != b.GetValue());
        }
        return true;
    }

    private void SetValue(double v)
    {
        if (this.m_slot == null)
        {
            this.m_slot = TssSdtDoubleSlot.NewSlot(null);
        }
        this.m_slot.SetValue(v);
    }

    public override string ToString()
    {
        return string.Format("{0}", this.GetValue());
    }
}

