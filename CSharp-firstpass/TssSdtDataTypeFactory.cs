using System;

public class TssSdtDataTypeFactory
{
    private static byte m_byte_xor_key;
    private static int m_int_xor_key;
    private static long m_long_xor_key;
    private static short m_short_xor_key;
    private static uint m_uint_xor_key;
    private static ulong m_ulong_xor_key;
    private static ushort m_ushort_xor_key;

    public static byte GetByteXORKey()
    {
        if (m_byte_xor_key == 0)
        {
            Random random = new Random();
            m_byte_xor_key = (byte) random.Next(0, 0xff);
        }
        return m_byte_xor_key;
    }

    public static double GetDoubleDecValue(ulong v, byte key)
    {
        byte[] bytes = BitConverter.GetBytes(v);
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = (byte) (bytes[i] ^ key);
        }
        return BitConverter.ToDouble(bytes, 0);
    }

    public static ulong GetDoubleEncValue(double v, byte key)
    {
        byte[] bytes = BitConverter.GetBytes(v);
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = (byte) (bytes[i] ^ key);
        }
        return BitConverter.ToUInt64(bytes, 0);
    }

    public static float GetFloatDecValue(uint v, byte key)
    {
        byte[] bytes = BitConverter.GetBytes(v);
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = (byte) (bytes[i] ^ key);
        }
        return BitConverter.ToSingle(bytes, 0);
    }

    public static uint GetFloatEncValue(float v, byte key)
    {
        byte[] bytes = BitConverter.GetBytes(v);
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = (byte) (bytes[i] ^ key);
        }
        return BitConverter.ToUInt32(bytes, 0);
    }

    public static int GetIntXORKey()
    {
        if (m_int_xor_key == 0)
        {
            m_int_xor_key = new Random().Next(0, 0xffff);
        }
        return m_int_xor_key;
    }

    public static long GetLongXORKey()
    {
        if (m_long_xor_key == 0)
        {
            m_long_xor_key = new Random().Next(0, 0xffff);
        }
        return m_long_xor_key;
    }

    public static int GetRandomValueIndex()
    {
        return m_int_xor_key;
    }

    public static short GetShortXORKey()
    {
        if (m_short_xor_key == 0)
        {
            Random random = new Random();
            m_short_xor_key = (short) random.Next(0, 0xffff);
        }
        return m_short_xor_key;
    }

    public static uint GetUintXORKey()
    {
        if (m_uint_xor_key == 0)
        {
            Random random = new Random();
            m_uint_xor_key = (uint) random.Next(0, 0xffff);
        }
        return m_uint_xor_key;
    }

    public static ulong GetUlongXORKey()
    {
        if (m_ulong_xor_key == 0)
        {
            Random random = new Random();
            m_ulong_xor_key = (ulong) random.Next(0, 0xffff);
        }
        return m_ulong_xor_key;
    }

    public static ushort GetUshortXORKey()
    {
        if (m_ushort_xor_key == 0)
        {
            Random random = new Random();
            m_ushort_xor_key = (ushort) random.Next(0, 0xffff);
        }
        return m_ushort_xor_key;
    }

    public static int GetValueArraySize()
    {
        return 3;
    }

    public static void SetByteXORKey(byte v)
    {
        m_byte_xor_key = v;
    }
}

