namespace Gif
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct Image
    {
        public ImageDesc desc;
        public byte[] data;
    }
}

