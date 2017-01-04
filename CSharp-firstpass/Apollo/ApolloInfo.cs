namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class ApolloInfo
    {
        public ApolloInfo(string pluginName = "")
        {
            this.ServiceId = 0x2710;
            this.MaxMessageBufferSize = 0x19000;
            this.PluginName = pluginName;
        }

        public ApolloInfo(int maxMessageBufferSize, string pluginName = "")
        {
            this.ServiceId = 0x2710;
            this.MaxMessageBufferSize = maxMessageBufferSize;
            this.PluginName = pluginName;
        }

        public ApolloInfo(int serviceId, int maxMessageBufferSize, string pluginName = "")
        {
            this.ServiceId = serviceId;
            this.MaxMessageBufferSize = maxMessageBufferSize;
            this.PluginName = pluginName;
        }

        public ApolloInfo(string qqAppId, string wxAppId, int maxMessageBufferSize, string pluginName = "")
        {
            this.ServiceId = 0xff;
            this.MaxMessageBufferSize = maxMessageBufferSize;
            this.QQAppId = qqAppId;
            this.WXAppId = wxAppId;
            this.PluginName = pluginName;
        }

        public int MaxMessageBufferSize { get; set; }

        public string PluginName { get; set; }

        public string QQAppId { get; set; }

        public int ServiceId { get; set; }

        public string WXAppId { get; set; }
    }
}

