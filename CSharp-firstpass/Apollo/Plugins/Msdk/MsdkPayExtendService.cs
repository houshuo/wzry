namespace Apollo.Plugins.Msdk
{
    using Apollo;
    using System;

    public class MsdkPayExtendService : IApolloExtendPayService, IApolloExtendPayServiceBase
    {
        public static MsdkPayExtendService Instance = new MsdkPayExtendService();

        private MsdkPayExtendService()
        {
        }
    }
}

