using System;

public class TssSdtUshort
{
    private TssSdtUshortSlot m_slot;

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return this.GetValue().GetHashCode();
    }

    private ushort GetValue()
    {
        if (this.m_slot == null)
        {
            this.m_slot = TssSdtUshortSlot.NewSlot(null);
        }
        return this.m_slot.GetValue();
    }

    public static TssSdtUshort NewTssSdtUshort()
    {
        return new TssSdtUshort { m_slot = TssSdtUshortSlot.NewSlot(null) };
    }

    public static TssSdtUshort operator --(TssSdtUshort v)
    {
        TssSdtUshort @ushort = new TssSdtUshort();
        if (v == null)
        {
            ushort num = 0;
            num = (ushort) (num - 1);
            @ushort.SetValue(num);
            return @ushort;
        }
        ushort num2 = (ushort) (v.GetValue() - 1);
        @ushort.SetValue(num2);
        return @ushort;
    }

    public static bool operator ==(TssSdtUshort a, TssSdtUshort b)
    {
        return ((object.Equals(a, null) && object.Equals(b, null)) || ((!object.Equals(a, null) && !object.Equals(b, null)) && (a.GetValue() == b.GetValue())));
    }

    public static implicit operator TssSdtUshort(ushort v)
    {
        TssSdtUshort @ushort = new TssSdtUshort();
        @ushort.SetValue(v);
        return @ushort;
    }

    public static implicit operator ushort(TssSdtUshort v)
    {
        if (v == null)
        {
            return 0;
        }
        return v.GetValue();
    }

    public static TssSdtUshort operator ++(TssSdtUshort v)
    {
        TssSdtUshort @ushort = new TssSdtUshort();
        if (v == null)
        {
            @ushort.SetValue(1);
            return @ushort;
        }
        ushort num = (ushort) (v.GetValue() + 1);
        @ushort.SetValue(num);
        return @ushort;
    }

    public static bool operator !=(TssSdtUshort a, TssSdtUshort b)
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

    private void SetValue(ushort v)
    {
        if (this.m_slot == null)
        {
            this.m_slot = TssSdtUshortSlot.NewSlot(null);
        }
        this.m_slot.SetValue(v);
    }

    public override string ToString()
    {
        return string.Format("{0}", this.GetValue());
    }
}

