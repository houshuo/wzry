using System;
using System.Text;

public class AkUtilities
{
    public class ShortIDGenerator
    {
        private static byte s_hashSize;
        private static uint s_mask;
        private const uint s_offsetBasis32 = 0x811c9dc5;
        private const uint s_prime32 = 0x1000193;

        static ShortIDGenerator()
        {
            HashSize = 0x20;
        }

        public static uint Compute(string in_name)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(in_name.ToLower());
            uint num = 0x811c9dc5;
            for (int i = 0; i < bytes.Length; i++)
            {
                num *= 0x1000193;
                num ^= bytes[i];
            }
            if (s_hashSize == 0x20)
            {
                return num;
            }
            return ((num >> s_hashSize) ^ (num & s_mask));
        }

        public static byte HashSize
        {
            get
            {
                return s_hashSize;
            }
            set
            {
                s_hashSize = value;
                s_mask = (uint) ((((int) 1) << s_hashSize) - 1);
            }
        }
    }
}

