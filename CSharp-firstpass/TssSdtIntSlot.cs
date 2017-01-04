using System;

public class TssSdtIntSlot
{
    private int m_index;
    private int[] m_value = new int[TssSdtDataTypeFactory.GetValueArraySize()];
    private int m_xor_key;

    public TssSdtIntSlot()
    {
        this.m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % this.m_value.Length;
    }

    private static void CollectSlot(TssSdtIntSlot slot)
    {
    }

    public int GetValue()
    {
        int num = this.m_value[this.m_index];
        return (num ^ this.m_xor_key);
    }

    public static TssSdtIntSlot NewSlot(TssSdtIntSlot slot)
    {
        CollectSlot(slot);
        return new TssSdtIntSlot();
    }

    public void SetValue(int v)
    {
        this.m_xor_key = TssSdtDataTypeFactory.GetIntXORKey();
        int index = this.m_index + 1;
        if (index == this.m_value.Length)
        {
            index = 0;
        }
        this.m_value[index] = v ^ this.m_xor_key;
        this.m_index = index;
    }
}

