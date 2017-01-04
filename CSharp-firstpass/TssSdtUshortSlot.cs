using System;

public class TssSdtUshortSlot
{
    private int m_index;
    private ushort[] m_value = new ushort[TssSdtDataTypeFactory.GetValueArraySize()];
    private ushort m_xor_key;

    public TssSdtUshortSlot()
    {
        this.m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % this.m_value.Length;
    }

    private static void CollectSlot(TssSdtUshortSlot slot)
    {
    }

    public ushort GetValue()
    {
        ushort num = this.m_value[this.m_index];
        return (ushort) (num ^ this.m_xor_key);
    }

    public static TssSdtUshortSlot NewSlot(TssSdtUshortSlot slot)
    {
        CollectSlot(slot);
        return new TssSdtUshortSlot();
    }

    public void SetValue(ushort v)
    {
        this.m_xor_key = TssSdtDataTypeFactory.GetUshortXORKey();
        int index = this.m_index + 1;
        if (index == this.m_value.Length)
        {
            index = 0;
        }
        ushort num2 = v;
        this.m_value[index] = (ushort) (num2 ^ this.m_xor_key);
        this.m_index = index;
    }
}

