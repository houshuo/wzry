namespace ApolloTdr
{
    using System;

    public interface IPackable
    {
        TdrError.ErrorType packTLV(ref byte[] buffer, int size, ref int used, bool useVarInt);
    }
}

