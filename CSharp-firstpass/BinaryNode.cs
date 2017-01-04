using System;
using System.Text;

public class BinaryNode
{
    public static readonly int INT_LEN = 4;
    private int m_attrNum;
    private int m_attrOffset;
    private int m_childNum;
    private int m_childOffset;
    private byte[] m_data;
    private int m_nameLen;
    private int m_nameOffset;
    private BinaryDomDocument m_owner;
    private BinaryNode m_parent;
    private int m_valueOffset;

    internal BinaryNode(byte[] data, int offset, BinaryDomDocument owner, BinaryNode parent)
    {
        this.m_data = data;
        this.m_owner = owner;
        this.m_parent = parent;
        BitConverter.ToInt32(data, offset);
        this.m_nameOffset = offset + INT_LEN;
        this.m_nameLen = BitConverter.ToInt32(data, this.m_nameOffset) - INT_LEN;
        this.m_attrOffset = (this.m_nameOffset + this.m_nameLen) + INT_LEN;
        int num = BitConverter.ToInt32(data, this.m_attrOffset);
        if (num > INT_LEN)
        {
            this.m_attrNum = BitConverter.ToInt32(data, this.m_attrOffset + INT_LEN);
        }
        else
        {
            this.m_attrNum = 0;
        }
        this.m_valueOffset = this.m_attrOffset + num;
        int num2 = BitConverter.ToInt32(data, this.m_valueOffset);
        this.m_childOffset = this.m_valueOffset + num2;
        if (BitConverter.ToInt32(data, this.m_childOffset) > INT_LEN)
        {
            this.m_childNum = BitConverter.ToInt32(data, this.m_childOffset + INT_LEN);
        }
        else
        {
            this.m_childNum = 0;
        }
    }

    public BinaryAttr GetAttr(int index)
    {
        DebugHelper.Assert(index < this.m_attrNum);
        int startIndex = (this.m_attrOffset + INT_LEN) + INT_LEN;
        for (int i = 0; i < index; i++)
        {
            int num3 = BitConverter.ToInt32(this.m_data, startIndex);
            startIndex += num3;
        }
        return new BinaryAttr(this.m_data, startIndex);
    }

    public int GetAttrNum()
    {
        return this.m_attrNum;
    }

    public BinaryNode GetChild(int index)
    {
        DebugHelper.Assert(index < this.m_childNum);
        int startIndex = (this.m_childOffset + INT_LEN) + INT_LEN;
        for (int i = 0; i < index; i++)
        {
            int num3 = BitConverter.ToInt32(this.m_data, startIndex);
            startIndex += num3;
        }
        return new BinaryNode(this.m_data, startIndex, this.OwnerDocument, this);
    }

    public int GetChildNum()
    {
        return this.m_childNum;
    }

    public string GetName()
    {
        return Encoding.UTF8.GetString(this.m_data, this.m_nameOffset + INT_LEN, this.m_nameLen);
    }

    public byte[] GetValue()
    {
        int num = BitConverter.ToInt32(this.m_data, this.m_valueOffset);
        if (num == INT_LEN)
        {
            return null;
        }
        byte[] destinationArray = new byte[num - INT_LEN];
        Array.Copy(this.m_data, this.m_valueOffset + INT_LEN, destinationArray, 0, destinationArray.Length);
        return destinationArray;
    }

    public string GetValueString()
    {
        return this.GetValueString(Encoding.UTF8);
    }

    public string GetValueString(Encoding encoding)
    {
        int num = BitConverter.ToInt32(this.m_data, this.m_valueOffset);
        if (num == INT_LEN)
        {
            return null;
        }
        return Encoding.UTF8.GetString(this.m_data, this.m_valueOffset + INT_LEN, num - INT_LEN);
    }

    public BinaryNode SelectSingleNode(string name)
    {
        for (int i = 0; i < this.GetChildNum(); i++)
        {
            BinaryNode child = this.GetChild(i);
            if (child.GetName() == name)
            {
                return child;
            }
        }
        return null;
    }

    public BinaryDomDocument OwnerDocument
    {
        get
        {
            return this.m_owner;
        }
    }

    public BinaryNode ParentNode
    {
        get
        {
            return this.m_parent;
        }
    }
}

