namespace IIPSMobile
{
    using System;

    public interface IIPSMobileDataReaderInterface
    {
        uint GetLastReaderError();
        bool Read(uint fileId, ulong offset, byte[] buff, ref uint readlength);
    }
}

