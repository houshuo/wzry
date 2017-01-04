namespace Apollo
{
    using System;

    public interface IMessageRequest
    {
        int pack(ref byte[] buffer, int size, ref int usedSize);
    }
}

