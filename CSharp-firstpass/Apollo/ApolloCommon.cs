namespace Apollo
{
    using System;

    public class ApolloCommon
    {
        public const string ApolloExPluginName = "apollo";
        private static Apollo.ApolloInfo apolloInfo;
        public const string ApolloPluginPluginName = "apollo";
        public const string ApolloTXPluginName = "apollo";
        private static uint msgSeq = 1;
        public static byte MsgVersion = 1;
        public const string PluginName = "apollo";

        public static Apollo.ApolloInfo ApolloInfo
        {
            get
            {
                if (apolloInfo == null)
                {
                    throw new Exception("IApollo.Instance.Initialize must be called before using Apollo!");
                }
                return apolloInfo;
            }
            set
            {
                apolloInfo = value;
            }
        }

        public static uint MsgSeq
        {
            get
            {
                msgSeq += 2;
                return msgSeq;
            }
        }
    }
}

