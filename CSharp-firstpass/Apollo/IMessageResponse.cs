namespace Apollo
{
    using System;

    public interface IMessageResponse
    {
        int unpack(ref byte[] buffer, int size, ref int usedSize);
    }
}

