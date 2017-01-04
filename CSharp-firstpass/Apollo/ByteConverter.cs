namespace Apollo
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;

    internal class ByteConverter
    {
        public static string Bytes2String(byte[] bytes)
        {
            string str = string.Empty;
            int index = 0;
            while (index < bytes.Length)
            {
                if (bytes[index] == 0)
                {
                    break;
                }
                index++;
            }
            byte[] destinationArray = new byte[index];
            Array.Copy(bytes, destinationArray, destinationArray.Length);
            List<int> list = new List<int>();
            for (int i = 0; i < (destinationArray.Length - 1); i++)
            {
                if (destinationArray[i] == 0)
                {
                    break;
                }
                if (destinationArray[i] == 20)
                {
                    list.Add(i);
                    i++;
                }
            }
            if (list.Count > 0)
            {
                if (list[0] > 0)
                {
                    str = str + Encoding.UTF8.GetString(destinationArray, 0, list[0]);
                }
                str = str + ((char) destinationArray[list[0]]) + ((char) destinationArray[list[0] + 1]);
            }
            for (int j = 1; j < list.Count; j++)
            {
                int count = (list[j] - list[j - 1]) - 2;
                if (count > 0)
                {
                    str = str + Encoding.UTF8.GetString(destinationArray, list[j - 1] + 2, count);
                }
                str = str + ((char) destinationArray[list[j]]) + ((char) destinationArray[list[j] + 1]);
            }
            int num5 = 0;
            if (list.Count > 0)
            {
                num5 = list[list.Count - 1] + 2;
            }
            if (num5 < destinationArray.Length)
            {
                str = str + Encoding.UTF8.GetString(destinationArray, num5, destinationArray.Length - num5);
            }
            return str;
        }

        public static bool IsCharValidate(char ch)
        {
            byte num = (byte) ((ch >> 8) & '\x00ff');
            byte num2 = (byte) (ch & '\x00ff');
            if ((num == 0) && ((num2 & 0x80) != 0))
            {
                return false;
            }
            return true;
        }

        public static byte[] ReverseBytes(byte[] inArray)
        {
            int index = inArray.Length - 1;
            for (int i = 0; i < (inArray.Length / 2); i++)
            {
                byte num = inArray[i];
                inArray[i] = inArray[index];
                inArray[index] = num;
                index--;
            }
            return inArray;
        }

        public static short ReverseEndian(short value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return IPAddress.NetworkToHostOrder(value);
            }
            return value;
        }

        public static int ReverseEndian(int value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return IPAddress.NetworkToHostOrder(value);
            }
            return value;
        }

        public static long ReverseEndian(long value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return IPAddress.NetworkToHostOrder(value);
            }
            return value;
        }

        public static ushort ReverseEndian(ushort value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return (ushort) IPAddress.NetworkToHostOrder((short) value);
            }
            return value;
        }

        public static uint ReverseEndian(uint value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return (uint) IPAddress.NetworkToHostOrder((int) value);
            }
            return value;
        }

        public static ulong ReverseEndian(ulong value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return (ulong) IPAddress.NetworkToHostOrder((long) value);
            }
            return value;
        }

        public static byte[] String2Bytes(string strInput)
        {
            if (strInput == null)
            {
                return null;
            }
            return Encoding.UTF8.GetBytes(strInput);
        }
    }
}

