using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class AkPositionArray : IDisposable
{
    public IntPtr m_Buffer;
    private uint m_Count;
    private IntPtr m_Current;
    private uint m_MaxCount;

    public AkPositionArray(uint in_Count)
    {
        this.m_Buffer = Marshal.AllocHGlobal((int) ((in_Count * 4) * 6));
        this.m_Current = this.m_Buffer;
        this.m_MaxCount = in_Count;
        this.m_Count = 0;
    }

    public void Add(Vector3 in_Pos, Vector3 in_Forward)
    {
        if (this.m_Count >= this.m_MaxCount)
        {
            throw new IndexOutOfRangeException("Out of range access in AkPositionArray");
        }
        Marshal.WriteInt32(this.m_Current, BitConverter.ToInt32(BitConverter.GetBytes(in_Pos.x), 0));
        this.m_Current = (IntPtr) (this.m_Current.ToInt64() + 4L);
        Marshal.WriteInt32(this.m_Current, BitConverter.ToInt32(BitConverter.GetBytes(in_Pos.y), 0));
        this.m_Current = (IntPtr) (this.m_Current.ToInt64() + 4L);
        Marshal.WriteInt32(this.m_Current, BitConverter.ToInt32(BitConverter.GetBytes(in_Pos.z), 0));
        this.m_Current = (IntPtr) (this.m_Current.ToInt64() + 4L);
        Marshal.WriteInt32(this.m_Current, BitConverter.ToInt32(BitConverter.GetBytes(in_Forward.x), 0));
        this.m_Current = (IntPtr) (this.m_Current.ToInt64() + 4L);
        Marshal.WriteInt32(this.m_Current, BitConverter.ToInt32(BitConverter.GetBytes(in_Forward.y), 0));
        this.m_Current = (IntPtr) (this.m_Current.ToInt64() + 4L);
        Marshal.WriteInt32(this.m_Current, BitConverter.ToInt32(BitConverter.GetBytes(in_Forward.z), 0));
        this.m_Current = (IntPtr) (this.m_Current.ToInt64() + 4L);
        this.m_Count++;
    }

    public void Dispose()
    {
        if (this.m_Buffer != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(this.m_Buffer);
            this.m_Buffer = IntPtr.Zero;
            this.m_MaxCount = 0;
        }
    }

    ~AkPositionArray()
    {
        this.Dispose();
    }

    public void Reset()
    {
        this.m_Current = this.m_Buffer;
        this.m_Count = 0;
    }

    public uint Count
    {
        get
        {
            return this.m_Count;
        }
    }
}

