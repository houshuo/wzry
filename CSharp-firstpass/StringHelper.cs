using System;
using System.Text;
using UnityEngine;

public class StringHelper
{
    public static StringBuilder Formater = new StringBuilder(0x400);

    public static string ASCIIBytesToString(byte[] data)
    {
        if (data == null)
        {
            return null;
        }
        try
        {
            return Encoding.ASCII.GetString(data).TrimEnd(new char[1]);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static string BytesToString(byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes).TrimEnd(new char[1]);
    }

    public static string BytesToString(string str)
    {
        return str;
    }

    public static string BytesToString_FindFristZero(byte[] bytes)
    {
        if (bytes == null)
        {
            return string.Empty;
        }
        int index = 0;
        while (index < bytes.Length)
        {
            if (bytes[index] == 0)
            {
                break;
            }
            index++;
        }
        return Encoding.UTF8.GetString(bytes, 0, index);
    }

    public static void ClearFormater()
    {
        Formater.Remove(0, Formater.Length);
    }

    public static bool IsAvailableString(string str)
    {
        int num = 0;
        int num2 = 0;
        char ch = '\0';
        bool flag = false;
        int length = str.Length;
        while (num2 < length)
        {
            ch = str[num2];
            if (flag)
            {
                if ((ch >= 0xdc00) && (ch <= 0xdfff))
                {
                    num += 4;
                }
                else
                {
                    Debug.Log(string.Format("invalid utf-16 sequence at {0} (missing surrogate tail)", num2));
                    return false;
                }
                flag = false;
            }
            else
            {
                if (ch < '\x0080')
                {
                    while (num2 < length)
                    {
                        if (str[num2] >= '\x0080')
                        {
                            break;
                        }
                        num++;
                        num2++;
                    }
                    continue;
                }
                if (ch < 'ࠀ')
                {
                    num += 2;
                }
                else if ((ch >= 0xd800) && (ch <= 0xdbff))
                {
                    flag = true;
                }
                else
                {
                    if ((ch >= 0xdc00) && (ch <= 0xdfff))
                    {
                        Debug.Log(string.Format("invalid utf-16 sequence at {0} (missing surrogate head)", num2));
                        return false;
                    }
                    num += 3;
                }
            }
            num2++;
        }
        return true;
    }

    public static void StringToUTF8Bytes(string str, ref byte[] buffer)
    {
        if ((str != null) && (buffer != null))
        {
            if (!str.EndsWith("\0"))
            {
                str = str + "\0";
            }
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            int count = (bytes.Length <= buffer.Length) ? bytes.Length : buffer.Length;
            Buffer.BlockCopy(bytes, 0, buffer, 0, count);
            buffer[buffer.Length - 1] = 0;
        }
    }

    public static void StringToUTF8Bytes(string str, ref sbyte[] buffer)
    {
        if ((str != null) && (buffer != null))
        {
            if (!str.EndsWith("\0"))
            {
                str = str + "\0";
            }
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            int count = (bytes.Length <= buffer.Length) ? bytes.Length : buffer.Length;
            Buffer.BlockCopy(bytes, 0, buffer, 0, count);
            buffer[buffer.Length - 1] = 0;
        }
    }

    public static string UTF8BytesToString(ref byte[] str)
    {
        try
        {
            return ((str == null) ? null : Encoding.UTF8.GetString(str).TrimEnd(new char[1]));
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static string UTF8BytesToString(ref string str)
    {
        return str;
    }
}

