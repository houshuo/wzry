﻿namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal delegate void ApolloSendResultBufferDelegate(ulong objectId, [MarshalAs(UnmanagedType.LPStr)] string function, int result, IntPtr buffer, int size);
}

