using System;

public class TssSdtLongSlot
{
    private int m_index;
    private long[] m_value = new long[TssSdtDataTypeFactory.GetValueArraySize()];
    private long m_xor_key;

    public TssSdtLongSlot()
    {
        this.m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % this.m_value.Length;
    }

    private static void CollectSlot(TssSdtLongSlot slot)
    {
    }

    public long GetValue()
    {
        long num = this.m_value[this.m_index];
        return (num ^ this.m_xor_key);
    }

    public static TssSdtLongSlot NewSlot(TssSdtLongSlot slot)
    {
        CollectSlot(slot);
        return new TssSdtLongSlot();
    }

    public void SetValue(long v)
    {
        this.m_xor_key = TssSdtDataTypeFactory.GetLongXORKey();
        int index = this.m_index + 1;
        if (index == this.m_value.Length)
        {
            index = 0;
        }
        this.m_value[index] = v ^ this.m_xor_key;
        this.m_index = index;
    }
}

