using System;

public class TssSdtUlongSlot
{
    private int m_index;
    private ulong[] m_value = new ulong[TssSdtDataTypeFactory.GetValueArraySize()];
    private ulong m_xor_key;

    public TssSdtUlongSlot()
    {
        this.m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % this.m_value.Length;
    }

    private static void CollectSlot(TssSdtUlongSlot slot)
    {
    }

    public ulong GetValue()
    {
        ulong num = this.m_value[this.m_index];
        return (num ^ this.m_xor_key);
    }

    public static TssSdtUlongSlot NewSlot(TssSdtUlongSlot slot)
    {
        CollectSlot(slot);
        return new TssSdtUlongSlot();
    }

    public void SetValue(ulong v)
    {
        this.m_xor_key = TssSdtDataTypeFactory.GetUlongXORKey();
        int index = this.m_index + 1;
        if (index == this.m_value.Length)
        {
            index = 0;
        }
        this.m_value[index] = v ^ this.m_xor_key;
        this.m_index = index;
    }
}

