namespace apollo_talker
{
    using ApolloTdr;
    using System;

    public interface tsf4g_csharp_interface : IPackable, IUnpackable
    {
        TdrError.ErrorType construct();
        TdrError.ErrorType packTLV(ref TdrWriteBuf destBuf, bool useVarInt);
        TdrError.ErrorType unpackTLV(ref TdrReadBuf srcBuf, int length, bool useVarInt);
    }
}

