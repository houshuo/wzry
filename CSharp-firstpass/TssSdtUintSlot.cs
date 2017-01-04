using System;

public class TssSdtUintSlot
{
    private int m_index;
    private uint[] m_value = new uint[TssSdtDataTypeFactory.GetValueArraySize()];
    private uint m_xor_key;

    public TssSdtUintSlot()
    {
        this.m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % this.m_value.Length;
    }

    private static void CollectSlot(TssSdtUintSlot slot)
    {
    }

    public uint GetValue()
    {
        uint num = this.m_value[this.m_index];
        return (num ^ this.m_xor_key);
    }

    public static TssSdtUintSlot NewSlot(TssSdtUintSlot slot)
    {
        CollectSlot(slot);
        return new TssSdtUintSlot();
    }

    public void SetValue(uint v)
    {
        this.m_xor_key = TssSdtDataTypeFactory.GetUintXORKey();
        int index = this.m_index + 1;
        if (index == this.m_value.Length)
        {
            index = 0;
        }
        this.m_value[index] = v ^ this.m_xor_key;
        this.m_index = index;
    }
}

