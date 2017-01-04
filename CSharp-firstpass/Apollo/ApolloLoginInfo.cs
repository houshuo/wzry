namespace Apollo
{
    using System;

    public class ApolloLoginInfo : ApolloStruct<ApolloLoginInfo>
    {
        public ApolloAccountInfo AccountInfo;
        public ApolloServerRouteInfo ServerInfo;
        public ApolloWaitingInfo WaitingInfo;

        public override ApolloLoginInfo FromString(string src)
        {
            ApolloStringParser parser = new ApolloStringParser(src);
            this.AccountInfo = parser.GetObject<ApolloAccountInfo>("AccountInfo");
            this.WaitingInfo = parser.GetObject<ApolloWaitingInfo>("WaitingInfo");
            string str = parser.GetString("ServerInfo");
            if (str != null)
            {
                str = ApolloStringParser.ReplaceApolloStringQuto(str);
                this.ServerInfo = new ApolloServerRouteInfo();
                this.ServerInfo.FromString(str);
            }
            return this;
        }
    }
}

