using System;

public class TssSdtShort
{
    private TssSdtShortSlot m_slot;

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return this.GetValue().GetHashCode();
    }

    private short GetValue()
    {
        if (this.m_slot == null)
        {
            this.m_slot = TssSdtShortSlot.NewSlot(null);
        }
        return this.m_slot.GetValue();
    }

    public static TssSdtShort NewTssSdtShort()
    {
        return new TssSdtShort { m_slot = TssSdtShortSlot.NewSlot(null) };
    }

    public static TssSdtShort operator --(TssSdtShort v)
    {
        TssSdtShort @short = new TssSdtShort();
        if (v == null)
        {
            @short.SetValue(-1);
            return @short;
        }
        short num = (short) (v.GetValue() - 1);
        @short.SetValue(num);
        return @short;
    }

    public static bool operator ==(TssSdtShort a, TssSdtShort b)
    {
        return ((object.Equals(a, null) && object.Equals(b, null)) || ((!object.Equals(a, null) && !object.Equals(b, null)) && (a.GetValue() == b.GetValue())));
    }

    public static implicit operator TssSdtShort(short v)
    {
        TssSdtShort @short = new TssSdtShort();
        @short.SetValue(v);
        return @short;
    }

    public static implicit operator short(TssSdtShort v)
    {
        if (v == null)
        {
            return 0;
        }
        return v.GetValue();
    }

    public static TssSdtShort operator ++(TssSdtShort v)
    {
        TssSdtShort @short = new TssSdtShort();
        if (v == null)
        {
            @short.SetValue(1);
            return @short;
        }
        short num = (short) (v.GetValue() + 1);
        @short.SetValue(num);
        return @short;
    }

    public static bool operator !=(TssSdtShort a, TssSdtShort b)
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

    private void SetValue(short v)
    {
        if (this.m_slot == null)
        {
            this.m_slot = TssSdtShortSlot.NewSlot(null);
        }
        this.m_slot.SetValue(v);
    }

    public override string ToString()
    {
        return string.Format("{0}", this.GetValue());
    }
}

