namespace Pathfinding.Util
{
    using System;

    public static class Memory
    {
        public static void MemSet<T>(T[] array, T value, int byteSize) where T: struct
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            int num = 0x20;
            int index = 0;
            int length = Math.Min(num, array.Length);
            while (index < length)
            {
                array[index] = value;
                index++;
            }
            length = array.Length;
            while (index < length)
            {
                Buffer.BlockCopy(array, 0, array, index * byteSize, Math.Min(num, length - index) * byteSize);
                index += num;
                num *= 2;
            }
        }

        public static void MemSet<T>(T[] array, T value, int totalSize, int byteSize) where T: struct
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            int num = 0x20;
            int index = 0;
            int num3 = Math.Min(num, totalSize);
            while (index < num3)
            {
                array[index] = value;
                index++;
            }
            num3 = totalSize;
            while (index < num3)
            {
                Buffer.BlockCopy(array, 0, array, index * byteSize, Math.Min(num, totalSize - index) * byteSize);
                index += num;
                num *= 2;
            }
        }
    }
}

