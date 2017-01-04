using System;

public class TssSdtInt
{
    private TssSdtIntSlot m_slot;

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return this.GetValue().GetHashCode();
    }

    private int GetValue()
    {
        if (this.m_slot == null)
        {
            this.m_slot = TssSdtIntSlot.NewSlot(null);
        }
        return this.m_slot.GetValue();
    }

    public static TssSdtInt NewTssSdtInt()
    {
        return new TssSdtInt { m_slot = TssSdtIntSlot.NewSlot(null) };
    }

    public static TssSdtInt operator --(TssSdtInt v)
    {
        TssSdtInt num = new TssSdtInt();
        if (v == null)
        {
            num.SetValue(-1);
            return num;
        }
        int num2 = v.GetValue() - 1;
        num.SetValue(num2);
        return num;
    }

    public static bool operator ==(TssSdtInt a, TssSdtInt b)
    {
        return ((object.Equals(a, null) && object.Equals(b, null)) || ((!object.Equals(a, null) && !object.Equals(b, null)) && (a.GetValue() == b.GetValue())));
    }

    public static implicit operator TssSdtInt(int v)
    {
        TssSdtInt num = new TssSdtInt();
        num.SetValue(v);
        return num;
    }

    public static implicit operator int(TssSdtInt v)
    {
        if (v == null)
        {
            return 0;
        }
        return v.GetValue();
    }

    public static TssSdtInt operator ++(TssSdtInt v)
    {
        TssSdtInt num = new TssSdtInt();
        if (v == null)
        {
            num.SetValue(1);
            return num;
        }
        int num2 = v.GetValue() + 1;
        num.SetValue(num2);
        return num;
    }

    public static bool operator !=(TssSdtInt a, TssSdtInt b)
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

    private void SetValue(int v)
    {
        if (this.m_slot == null)
        {
            this.m_slot = TssSdtIntSlot.NewSlot(null);
        }
        this.m_slot.SetValue(v);
    }

    public override string ToString()
    {
        return string.Format("{0}", this.GetValue());
    }
}

