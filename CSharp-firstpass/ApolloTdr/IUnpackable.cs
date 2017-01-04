namespace ApolloTdr
{
    using System;

    public interface IUnpackable
    {
        TdrError.ErrorType unpackTLV(ref byte[] buffer, int size, ref int used);
    }
}

