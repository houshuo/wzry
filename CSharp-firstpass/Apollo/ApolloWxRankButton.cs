namespace Apollo
{
    using System;
    using System.Runtime.InteropServices;

    public class ApolloWxRankButton : ApolloWxButtonInfo
    {
        public string ButtonName;
        public string MessageExt;
        public string Name;
        public string Title;

        public ApolloWxRankButton(ApolloWxButtonType type, string name, string title, string buttonName, string messageExt) : base(type)
        {
            this.Name = name;
            this.Title = title;
            this.ButtonName = buttonName;
            this.MessageExt = messageExt;
        }

        internal override bool Pack(out string buffer)
        {
            base.Pack(out buffer);
            buffer = buffer + "&name=" + this.Name;
            buffer = buffer + "&title=" + this.Title;
            buffer = buffer + "&buttonName=" + this.ButtonName;
            buffer = buffer + "&messageExt=" + this.MessageExt;
            return true;
        }
    }
}

