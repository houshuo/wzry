namespace com.tencent.pandora
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct ReaderInfo
    {
        public string chunkData;
        public bool finished;
    }
}

