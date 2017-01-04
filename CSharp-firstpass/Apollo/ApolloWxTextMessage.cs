namespace Apollo
{
    using System;
    using System.Runtime.InteropServices;

    public class ApolloWxTextMessage : ApolloWxMessageInfo
    {
        public ApolloWxTextMessage(ApolloWxMessageType type) : base(type)
        {
        }

        internal override bool Pack(out string buffer)
        {
            base.Pack(out buffer);
            return true;
        }
    }
}

