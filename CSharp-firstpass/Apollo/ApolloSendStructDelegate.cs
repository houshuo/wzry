namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal delegate void ApolloSendStructDelegate(ulong objectId, [MarshalAs(UnmanagedType.LPStr)] string function, IntPtr param);
}

