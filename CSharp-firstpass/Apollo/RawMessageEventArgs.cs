namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;

    public class RawMessageEventArgs
    {
        public RawMessageEventArgs(byte[] data)
        {
            this.Data = data;
        }

        public byte[] Data { get; set; }
    }
}

