using System;

public class TssSdtByte
{
    private TssSdtByteSlot m_slot;

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return this.GetValue().GetHashCode();
    }

    private byte GetValue()
    {
        if (this.m_slot == null)
        {
            this.m_slot = TssSdtByteSlot.NewSlot(null);
        }
        return this.m_slot.GetValue();
    }

    public static TssSdtByte NewTssSdtByte()
    {
        return new TssSdtByte { m_slot = TssSdtByteSlot.NewSlot(null) };
    }

    public static TssSdtByte operator --(TssSdtByte v)
    {
        TssSdtByte num = new TssSdtByte();
        if (v == null)
        {
            byte num2 = 0;
            num2 = (byte) (num2 - 1);
            num.SetValue(num2);
            return num;
        }
        byte num3 = (byte) (v.GetValue() - 1);
        num.SetValue(num3);
        return num;
    }

    public static bool operator ==(TssSdtByte a, TssSdtByte b)
    {
        return ((object.Equals(a, null) && object.Equals(b, null)) || ((!object.Equals(a, null) && !object.Equals(b, null)) && (a.GetValue() == b.GetValue())));
    }

    public static implicit operator TssSdtByte(byte v)
    {
        TssSdtByte num = new TssSdtByte();
        num.SetValue(v);
        return num;
    }

    public static implicit operator byte(TssSdtByte v)
    {
        if (v == null)
        {
            return 0;
        }
        return v.GetValue();
    }

    public static TssSdtByte operator ++(TssSdtByte v)
    {
        TssSdtByte num = new TssSdtByte();
        if (v == null)
        {
            num.SetValue(1);
            return num;
        }
        byte num2 = (byte) (v.GetValue() + 1);
        num.SetValue(num2);
        return num;
    }

    public static bool operator !=(TssSdtByte a, TssSdtByte b)
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

    private void SetValue(byte v)
    {
        if (this.m_slot == null)
        {
            this.m_slot = TssSdtByteSlot.NewSlot(null);
        }
        this.m_slot.SetValue(v);
    }

    public override string ToString()
    {
        return string.Format("{0}", this.GetValue());
    }
}

