using System;

public class TssSdtDoubleSlot
{
    private int m_index;
    private ulong[] m_value = new ulong[TssSdtDataTypeFactory.GetValueArraySize()];
    private byte m_xor_key;

    public TssSdtDoubleSlot()
    {
        this.m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % this.m_value.Length;
    }

    private static void CollectSlot(TssSdtDoubleSlot slot)
    {
    }

    public double GetValue()
    {
        ulong v = this.m_value[this.m_index];
        return TssSdtDataTypeFactory.GetDoubleDecValue(v, this.m_xor_key);
    }

    public static TssSdtDoubleSlot NewSlot(TssSdtDoubleSlot slot)
    {
        CollectSlot(slot);
        return new TssSdtDoubleSlot();
    }

    public void SetValue(double v)
    {
        this.m_xor_key = TssSdtDataTypeFactory.GetByteXORKey();
        int index = this.m_index + 1;
        if (index == this.m_value.Length)
        {
            index = 0;
        }
        this.m_value[index] = TssSdtDataTypeFactory.GetDoubleEncValue(v, this.m_xor_key);
        this.m_index = index;
    }
}

