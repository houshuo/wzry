namespace Apollo
{
    using System;
    using System.Runtime.InteropServices;

    public class ApolloWxAppButton : ApolloWxButtonInfo
    {
        public string MessageExt;
        public string Name;

        public ApolloWxAppButton(ApolloWxButtonType type, string name, string messageExt) : base(type)
        {
            this.Name = name;
            this.MessageExt = messageExt;
        }

        internal override bool Pack(out string buffer)
        {
            base.Pack(out buffer);
            buffer = buffer + "&name=" + this.Name;
            buffer = buffer + "&messageExt=" + this.MessageExt;
            return true;
        }
    }
}

