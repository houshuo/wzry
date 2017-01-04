namespace Apollo
{
    using System;

    public abstract class IApollo
    {
        private static IApollo instance;

        protected IApollo()
        {
        }

        public abstract IApolloConnector CreateApolloConnection(ApolloPlatform platform, string svrUrl);
        [Obsolete("Deprecated since 1.1.2, use CreateApolloConnection(ApolloPlatform platform, UInt32 permission, String svrUrl) instead")]
        public abstract IApolloConnector CreateApolloConnection(ApolloPlatform platform, ApolloPermission permission, string svrUrl);
        [Obsolete("Deprecated since 1.1.13, use CreateApolloConnection(ApolloPlatform platform,  String svrUrl) instead")]
        public abstract IApolloConnector CreateApolloConnection(ApolloPlatform platform, uint permission, string svrUrl);
        public abstract IApolloHttpClient CreateHttpClient();
        public abstract IApolloTalker CreateTalker(IApolloConnector connector);
        public abstract void DestoryHttpClient(IApolloHttpClient client);
        public abstract void DestroyApolloConnector(IApolloConnector Connector);
        public abstract void DestroyTalker(IApolloTalker talker);
        public abstract IApolloAccountService GetAccountService();
        public abstract IApolloServiceBase GetService(int Type);
        public abstract ApolloResult Initialize(ApolloInfo platformInfo);
        public abstract void SetApolloLogger(ApolloLogPriority pri, ApolloLogHandler callback);
        public abstract bool SwitchPlugin(string pluginName);

        public static IApollo Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Apollo.Apollo();
                }
                return instance;
            }
        }
    }
}

