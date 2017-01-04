namespace Apollo
{
    using System;
    using System.Runtime.InteropServices;

    public class ApolloWxMessageInfo
    {
        public ApolloWxMessageType Type;

        public ApolloWxMessageInfo(ApolloWxMessageType type)
        {
            this.Type = type;
        }

        internal virtual bool Pack(out string buffer)
        {
            buffer = "type=" + ((int) this.Type);
            return true;
        }
    }
}

