using System;
using System.Text;
using UnityEngine;

public class CMemoryManager
{
    private const int c_arraySize = 100;
    public static GameObject[] s_gameObjectArray = new GameObject[100];
    public static int[] s_intArray = new int[100];
    public static MonoBehaviour[] s_scriptArray = new MonoBehaviour[100];
    public static string[] s_stringArray = new string[100];

    public static int ReadByte(byte[] data, ref int offset)
    {
        int num = data[offset];
        offset++;
        return num;
    }

    public static DateTime ReadDateTime(byte[] data, ref int offset)
    {
        long ticks = BitConverter.ToInt64(data, offset);
        offset += 8;
        if ((ticks < DateTime.MaxValue.Ticks) && (ticks > DateTime.MinValue.Ticks))
        {
            return new DateTime(ticks);
        }
        return new DateTime();
    }

    public static int ReadInt(byte[] data, ref int offset)
    {
        int num = (((data[offset + 3] << 0x18) | (data[offset + 2] << 0x10)) | (data[offset + 1] << 8)) | data[offset];
        offset += 4;
        return num;
    }

    public static long ReadLong(byte[] data, ref int offset)
    {
        int num = ReadInt(data, ref offset);
        return ((ReadInt(data, ref offset) << 0x20) | num);
    }

    public static int ReadShort(byte[] data, ref int offset)
    {
        int num = (data[offset + 1] << 8) | data[offset];
        offset += 2;
        return num;
    }

    public static string ReadString(byte[] data, ref int offset)
    {
        int count = ReadShort(data, ref offset);
        string str = Encoding.UTF8.GetString(data, offset, count);
        offset += count;
        return str;
    }

    public static void WriteByte(byte value, byte[] data, ref int offset)
    {
        data[offset] = value;
        offset++;
    }

    public static void WriteDateTime(ref DateTime dateTime, byte[] data, ref int offset)
    {
        byte[] bytes = BitConverter.GetBytes(dateTime.Ticks);
        for (int i = 0; i < bytes.Length; i++)
        {
            data[offset] = bytes[i];
            offset++;
        }
    }

    public static void WriteInt(int value, byte[] data, ref int offset)
    {
        data[offset] = (byte) value;
        data[offset + 1] = (byte) (value >> 8);
        data[offset + 2] = (byte) (value >> 0x10);
        data[offset + 3] = (byte) (value >> 0x18);
        offset += 4;
    }

    public static void WriteLong(long value, byte[] data, ref int offset)
    {
        int num = (int) value;
        int num2 = (int) (value >> 0x20);
        WriteInt(num, data, ref offset);
        WriteInt(num2, data, ref offset);
    }

    public static void WriteShort(short value, byte[] data, ref int offset)
    {
        data[offset] = (byte) value;
        data[offset + 1] = (byte) (value >> 8);
        offset += 2;
    }

    public static void WriteString(string str, byte[] data, ref int offset)
    {
        int num = Encoding.UTF8.GetBytes(str, 0, str.Length, data, offset + 2);
        WriteShort((short) num, data, ref offset);
        offset += num;
    }
}

