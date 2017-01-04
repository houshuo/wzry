namespace tsf4g_tdr_csharp
{
    using System;

    public interface IUnpackable
    {
        TdrError.ErrorType unpack(ref byte[] buffer, int size, ref int usedSize, uint cutVer);
    }
}

