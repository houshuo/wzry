namespace Apollo
{
    using System;
    using System.Runtime.InteropServices;

    public sealed class NoneAccountService
    {
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool apollo_none_account_initialize([MarshalAs(UnmanagedType.LPArray)] byte[] data, int len);
        public static ApolloResult Initialize(NoneAccountInitInfo initInfo)
        {
            if (initInfo == null)
            {
                ADebug.LogError("NoneAccountService initInfo == null");
                return ApolloResult.InvalidArgument;
            }
            byte[] buffer = null;
            if (initInfo.Encode(out buffer) && (buffer != null))
            {
                apollo_none_account_initialize(buffer, buffer.Length);
                return ApolloResult.Success;
            }
            ADebug.LogError("NoneAccountService Encode error!");
            return ApolloResult.InnerError;
        }
    }
}

