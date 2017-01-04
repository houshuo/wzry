using System;

public class TssSdtUint
{
    private TssSdtUintSlot m_slot;

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return this.GetValue().GetHashCode();
    }

    private uint GetValue()
    {
        if (this.m_slot == null)
        {
            this.m_slot = TssSdtUintSlot.NewSlot(null);
        }
        return this.m_slot.GetValue();
    }

    public static TssSdtUint NewTssSdtUint()
    {
        return new TssSdtUint { m_slot = TssSdtUintSlot.NewSlot(null) };
    }

    public static TssSdtUint operator --(TssSdtUint v)
    {
        TssSdtUint num = new TssSdtUint();
        if (v == null)
        {
            uint num2 = 0;
            num2--;
            num.SetValue(num2);
            return num;
        }
        uint num3 = v.GetValue() - 1;
        num.SetValue(num3);
        return num;
    }

    public static bool operator ==(TssSdtUint a, TssSdtUint b)
    {
        return ((object.Equals(a, null) && object.Equals(b, null)) || ((!object.Equals(a, null) && !object.Equals(b, null)) && (a.GetValue() == b.GetValue())));
    }

    public static implicit operator TssSdtUint(uint v)
    {
        TssSdtUint num = new TssSdtUint();
        num.SetValue(v);
        return num;
    }

    public static implicit operator uint(TssSdtUint v)
    {
        if (v == null)
        {
            return 0;
        }
        return v.GetValue();
    }

    public static TssSdtUint operator ++(TssSdtUint v)
    {
        TssSdtUint num = new TssSdtUint();
        if (v == null)
        {
            num.SetValue(1);
            return num;
        }
        uint num2 = v.GetValue() + 1;
        num.SetValue(num2);
        return num;
    }

    public static bool operator !=(TssSdtUint a, TssSdtUint b)
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

    private void SetValue(uint v)
    {
        if (this.m_slot == null)
        {
            this.m_slot = TssSdtUintSlot.NewSlot(null);
        }
        this.m_slot.SetValue(v);
    }

    public override string ToString()
    {
        return string.Format("{0}", this.GetValue());
    }
}

