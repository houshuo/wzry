namespace tsf4g_tdr_csharp
{
    using System;

    public interface IPackable
    {
        TdrError.ErrorType pack(ref byte[] buffer, int size, ref int usedSize, uint cutVer);
    }
}

