using System;

public class TssSdtFloatSlot
{
    private int m_index;
    private uint[] m_value = new uint[TssSdtDataTypeFactory.GetValueArraySize()];
    private byte m_xor_key;

    public TssSdtFloatSlot()
    {
        this.m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % this.m_value.Length;
    }

    private static void CollectSlot(TssSdtFloatSlot slot)
    {
    }

    public float GetValue()
    {
        uint v = this.m_value[this.m_index];
        return TssSdtDataTypeFactory.GetFloatDecValue(v, this.m_xor_key);
    }

    public static TssSdtFloatSlot NewSlot(TssSdtFloatSlot slot)
    {
        CollectSlot(slot);
        return new TssSdtFloatSlot();
    }

    public void SetValue(float v)
    {
        this.m_xor_key = TssSdtDataTypeFactory.GetByteXORKey();
        int index = this.m_index + 1;
        if (index == this.m_value.Length)
        {
            index = 0;
        }
        this.m_value[index] = TssSdtDataTypeFactory.GetFloatEncValue(v, this.m_xor_key);
        this.m_index = index;
    }
}

