using System;
using System.Runtime.InteropServices;

public class AkAuxSendArray
{
    public IntPtr m_Buffer;
    public uint m_Count;
    private IntPtr m_Current;
    private uint m_MaxCount;

    public AkAuxSendArray(uint in_Count)
    {
        this.m_Buffer = Marshal.AllocHGlobal((int) (in_Count * 8));
        this.m_Current = this.m_Buffer;
        this.m_MaxCount = in_Count;
        this.m_Count = 0;
    }

    public void Add(uint in_EnvID, float in_fValue)
    {
        if (this.m_Count >= this.m_MaxCount)
        {
            this.Resize(this.m_Count * 2);
        }
        Marshal.WriteInt32(this.m_Current, (int) in_EnvID);
        this.m_Current = (IntPtr) (this.m_Current.ToInt64() + 4L);
        Marshal.WriteInt32(this.m_Current, BitConverter.ToInt32(BitConverter.GetBytes(in_fValue), 0));
        this.m_Current = (IntPtr) (this.m_Current.ToInt64() + 4L);
        this.m_Count++;
    }

    public bool Contains(uint in_EnvID)
    {
        IntPtr buffer = this.m_Buffer;
        for (int i = 0; i < this.m_Count; i++)
        {
            if (in_EnvID == Marshal.ReadInt32(buffer))
            {
                return true;
            }
            buffer = (IntPtr) ((buffer.ToInt64() + 4L) + 4L);
        }
        return false;
    }

    ~AkAuxSendArray()
    {
        Marshal.FreeHGlobal(this.m_Buffer);
        this.m_Buffer = IntPtr.Zero;
    }

    public int OffsetOf(uint in_EnvID)
    {
        IntPtr buffer = this.m_Buffer;
        for (int i = 0; i < this.m_Count; i++)
        {
            if (in_EnvID == Marshal.ReadInt32(buffer))
            {
                return (buffer.ToInt32() - this.m_Buffer.ToInt32());
            }
            buffer = (IntPtr) ((buffer.ToInt64() + 4L) + 4L);
        }
        return -1;
    }

    public void Remove(uint in_EnvID)
    {
        IntPtr buffer = this.m_Buffer;
        for (int i = 0; i < this.m_Count; i++)
        {
            if (in_EnvID == Marshal.ReadInt32(buffer))
            {
                IntPtr ptr = (IntPtr) (this.m_Buffer.ToInt64() + ((this.m_Count - 1) * 8));
                Marshal.WriteInt32(buffer, Marshal.ReadInt32(ptr));
                buffer = (IntPtr) (buffer.ToInt64() + 4L);
                ptr = (IntPtr) (ptr.ToInt64() + 4L);
                Marshal.WriteInt32(buffer, Marshal.ReadInt32(ptr));
                this.m_Count--;
                break;
            }
            buffer = (IntPtr) ((buffer.ToInt64() + 4L) + 4L);
        }
    }

    public void RemoveAt(int in_offset)
    {
        IntPtr ptr = (IntPtr) (this.m_Buffer.ToInt64() + in_offset);
        IntPtr ptr2 = (IntPtr) (this.m_Buffer.ToInt64() + ((this.m_Count - 1) * 8));
        Marshal.WriteInt32(ptr, Marshal.ReadInt32(ptr2));
        ptr = (IntPtr) (ptr.ToInt64() + 4L);
        ptr2 = (IntPtr) (ptr2.ToInt64() + 4L);
        Marshal.WriteInt32(ptr, Marshal.ReadInt32(ptr2));
        this.m_Count--;
    }

    public void ReplaceAt(int in_offset, uint in_EnvID, float in_fValue)
    {
        IntPtr ptr = (IntPtr) (this.m_Buffer.ToInt64() + in_offset);
        Marshal.WriteInt32(ptr, (int) in_EnvID);
        ptr = (IntPtr) (ptr.ToInt64() + 4L);
        Marshal.WriteInt32(ptr, BitConverter.ToInt32(BitConverter.GetBytes(in_fValue), 0));
    }

    public void Reset()
    {
        this.m_Current = this.m_Buffer;
        this.m_Count = 0;
    }

    public void Resize(uint in_size)
    {
        if (in_size <= this.m_Count)
        {
            this.m_Count = in_size;
        }
        else
        {
            this.m_MaxCount = in_size;
            IntPtr ptr = Marshal.AllocHGlobal((int) (this.m_MaxCount * 8));
            IntPtr buffer = this.m_Buffer;
            this.m_Current = ptr;
            for (int i = 0; i < this.m_Count; i++)
            {
                Marshal.WriteInt32(this.m_Current, Marshal.ReadInt32(buffer));
                this.m_Current = (IntPtr) (this.m_Current.ToInt64() + 4L);
                buffer = (IntPtr) (buffer.ToInt64() + 4L);
                Marshal.WriteInt32(this.m_Current, Marshal.ReadInt32(buffer));
                this.m_Current = (IntPtr) (this.m_Current.ToInt64() + 4L);
                buffer = (IntPtr) (buffer.ToInt64() + 4L);
            }
            Marshal.FreeHGlobal(this.m_Buffer);
            this.m_Buffer = ptr;
        }
    }
}

