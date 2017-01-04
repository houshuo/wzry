using System;

public class TssSdtShortSlot
{
    private int m_index;
    private short[] m_value = new short[TssSdtDataTypeFactory.GetValueArraySize()];
    private short m_xor_key;

    public TssSdtShortSlot()
    {
        this.m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % this.m_value.Length;
    }

    private static void CollectSlot(TssSdtShortSlot slot)
    {
    }

    public short GetValue()
    {
        short num = this.m_value[this.m_index];
        return (short) (num ^ this.m_xor_key);
    }

    public static TssSdtShortSlot NewSlot(TssSdtShortSlot slot)
    {
        CollectSlot(slot);
        return new TssSdtShortSlot();
    }

    public void SetValue(short v)
    {
        this.m_xor_key = TssSdtDataTypeFactory.GetShortXORKey();
        int index = this.m_index + 1;
        if (index == this.m_value.Length)
        {
            index = 0;
        }
        short num2 = v;
        this.m_value[index] = (short) (num2 ^ this.m_xor_key);
        this.m_index = index;
    }
}

