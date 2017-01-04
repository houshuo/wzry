using System;
using System.Runtime.InteropServices;
using System.Text;

[StructLayout(LayoutKind.Sequential)]
public struct BinaryAttr
{
    public static readonly int INT_LEN;
    private byte[] m_data;
    private int m_offset;
    private int m_dataLen;
    private int m_nameOffset;
    private int m_nameLen;
    private int m_valueOffset;
    private int m_valueLen;
    internal BinaryAttr(byte[] data, int offset = 0)
    {
        this.m_data = data;
        this.m_offset = offset;
        this.m_dataLen = BitConverter.ToInt32(data, offset);
        this.m_nameOffset = offset + INT_LEN;
        this.m_nameLen = BitConverter.ToInt32(data, this.m_nameOffset) - INT_LEN;
        this.m_valueOffset = (this.m_nameOffset + this.m_nameLen) + INT_LEN;
        this.m_valueLen = (this.m_offset + this.m_dataLen) - this.m_valueOffset;
    }

    static BinaryAttr()
    {
        INT_LEN = 4;
    }

    public byte[] GetValue()
    {
        if (this.m_valueLen == 0)
        {
            return null;
        }
        byte[] destinationArray = new byte[this.m_valueLen];
        Array.Copy(this.m_data, this.m_valueOffset, destinationArray, 0, destinationArray.Length);
        return destinationArray;
    }

    public string GetValueString()
    {
        return this.GetValueString(Encoding.UTF8);
    }

    public string GetValueString(Encoding encoding)
    {
        if (this.m_valueLen == 0)
        {
            return null;
        }
        return Encoding.UTF8.GetString(this.m_data, this.m_valueOffset, this.m_valueLen);
    }

    public string GetName()
    {
        return Encoding.UTF8.GetString(this.m_data, this.m_nameOffset + INT_LEN, this.m_nameLen);
    }
}

