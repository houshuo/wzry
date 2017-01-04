namespace Apollo
{
    using ApolloTdr;
    using System;
    using System.Runtime.CompilerServices;

    internal delegate void TalkerMessageWithReceiptHandler(IUnpackable resp, ref IPackable receipt);
}

