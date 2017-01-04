using System;

public class TssSdtByteSlot
{
    private int m_index;
    private byte[] m_value = new byte[TssSdtDataTypeFactory.GetValueArraySize()];
    private byte m_xor_key;

    public TssSdtByteSlot()
    {
        this.m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % this.m_value.Length;
    }

    private static void CollectSlot(TssSdtByteSlot slot)
    {
    }

    public byte GetValue()
    {
        byte num = this.m_value[this.m_index];
        return (byte) (num ^ this.m_xor_key);
    }

    public static TssSdtByteSlot NewSlot(TssSdtByteSlot slot)
    {
        CollectSlot(slot);
        return new TssSdtByteSlot();
    }

    public void SetValue(byte v)
    {
        this.m_xor_key = TssSdtDataTypeFactory.GetByteXORKey();
        int index = this.m_index + 1;
        if (index == this.m_value.Length)
        {
            index = 0;
        }
        byte num2 = v;
        this.m_value[index] = (byte) (num2 ^ this.m_xor_key);
        this.m_index = index;
    }
}

