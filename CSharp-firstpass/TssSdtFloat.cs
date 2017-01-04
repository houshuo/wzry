using System;

public class TssSdtFloat
{
    private TssSdtFloatSlot m_slot;

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return this.GetValue().GetHashCode();
    }

    private float GetValue()
    {
        if (this.m_slot == null)
        {
            this.m_slot = TssSdtFloatSlot.NewSlot(null);
        }
        return this.m_slot.GetValue();
    }

    public static TssSdtFloat NewTssSdtFloat()
    {
        return new TssSdtFloat { m_slot = TssSdtFloatSlot.NewSlot(null) };
    }

    public static TssSdtFloat operator --(TssSdtFloat v)
    {
        TssSdtFloat num = new TssSdtFloat();
        if (v == null)
        {
            num.SetValue(-1f);
            return num;
        }
        float num2 = v.GetValue() - 1f;
        num.SetValue(num2);
        return num;
    }

    public static bool operator ==(TssSdtFloat a, TssSdtFloat b)
    {
        return ((object.Equals(a, null) && object.Equals(b, null)) || ((!object.Equals(a, null) && !object.Equals(b, null)) && (a.GetValue() == b.GetValue())));
    }

    public static implicit operator TssSdtFloat(float v)
    {
        TssSdtFloat num = new TssSdtFloat();
        num.SetValue(v);
        return num;
    }

    public static implicit operator float(TssSdtFloat v)
    {
        if (v == null)
        {
            return 0f;
        }
        return v.GetValue();
    }

    public static TssSdtFloat operator ++(TssSdtFloat v)
    {
        TssSdtFloat num = new TssSdtFloat();
        if (v == null)
        {
            num.SetValue(1f);
            return num;
        }
        float num2 = v.GetValue() + 1f;
        num.SetValue(num2);
        return num;
    }

    public static bool operator !=(TssSdtFloat a, TssSdtFloat b)
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

    private void SetValue(float v)
    {
        if (this.m_slot == null)
        {
            this.m_slot = TssSdtFloatSlot.NewSlot(null);
        }
        this.m_slot.SetValue(v);
    }

    public override string ToString()
    {
        return string.Format("{0}", this.GetValue());
    }
}

