namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal delegate void ApolloSendBufferDelegate(ulong objectId, [MarshalAs(UnmanagedType.LPStr)] string function, IntPtr buffer, int size);
}

