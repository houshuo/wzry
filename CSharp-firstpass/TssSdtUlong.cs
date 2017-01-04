using System;

public class TssSdtUlong
{
    private TssSdtUlongSlot m_slot;

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return this.GetValue().GetHashCode();
    }

    private ulong GetValue()
    {
        if (this.m_slot == null)
        {
            this.m_slot = TssSdtUlongSlot.NewSlot(null);
        }
        return this.m_slot.GetValue();
    }

    public static TssSdtUlong NewTssSdtUlong()
    {
        return new TssSdtUlong { m_slot = TssSdtUlongSlot.NewSlot(null) };
    }

    public static TssSdtUlong operator --(TssSdtUlong v)
    {
        TssSdtUlong @ulong = new TssSdtUlong();
        if (v == null)
        {
            ulong num = 0L;
            num -= (ulong) 1L;
            @ulong.SetValue(num);
            return @ulong;
        }
        ulong num2 = v.GetValue() - ((ulong) 1L);
        @ulong.SetValue(num2);
        return @ulong;
    }

    public static bool operator ==(TssSdtUlong a, TssSdtUlong b)
    {
        return ((object.Equals(a, null) && object.Equals(b, null)) || ((!object.Equals(a, null) && !object.Equals(b, null)) && (a.GetValue() == b.GetValue())));
    }

    public static implicit operator TssSdtUlong(ulong v)
    {
        TssSdtUlong @ulong = new TssSdtUlong();
        @ulong.SetValue(v);
        return @ulong;
    }

    public static implicit operator ulong(TssSdtUlong v)
    {
        if (v == null)
        {
            return 0L;
        }
        return v.GetValue();
    }

    public static TssSdtUlong operator ++(TssSdtUlong v)
    {
        TssSdtUlong @ulong = new TssSdtUlong();
        if (v == null)
        {
            @ulong.SetValue(1L);
            return @ulong;
        }
        ulong num = v.GetValue() + ((ulong) 1L);
        @ulong.SetValue(num);
        return @ulong;
    }

    public static bool operator !=(TssSdtUlong a, TssSdtUlong b)
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

    private void SetValue(ulong v)
    {
        if (this.m_slot == null)
        {
            this.m_slot = TssSdtUlongSlot.NewSlot(null);
        }
        this.m_slot.SetValue(v);
    }

    public override string ToString()
    {
        return string.Format("{0}", this.GetValue());
    }
}

