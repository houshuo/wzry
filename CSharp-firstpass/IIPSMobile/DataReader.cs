namespace IIPSMobile
{
    using System;
    using System.Runtime.InteropServices;

    public class DataReader : IIPSMobileDataReaderInterface
    {
        public IntPtr mDataReader = IntPtr.Zero;

        public DataReader(IntPtr Reader)
        {
            this.mDataReader = Reader;
        }

        public uint GetLastReaderError()
        {
            if (this.mDataReader == IntPtr.Zero)
            {
                return 0;
            }
            return GetLastReaderError(this.mDataReader);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern uint GetLastReaderError(IntPtr reader);
        public bool Read(uint fileId, ulong offset, byte[] buff, ref uint readlength)
        {
            if (this.mDataReader == IntPtr.Zero)
            {
                return false;
            }
            return (Read(this.mDataReader, fileId, offset, buff, ref readlength) > 0);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte Read(IntPtr dataReader, uint fileId, ulong offset, byte[] buff, ref uint readlength);
    }
}

