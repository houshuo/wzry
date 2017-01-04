namespace Apollo
{
    using Apollo.Plugins.Msdk;
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class Apollo : IApollo
    {
        private event ApolloLogHandler logEvent;

        public Apollo()
        {
            Tx.Instance.Initialize();
        }

        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern int apollo_init(int nServiceId, int nMaxMessageBuffSize, [MarshalAs(UnmanagedType.LPStr)] string pluginName);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern void apollo_setApolloLogger(ApolloLogPriority pri, [MarshalAs(UnmanagedType.FunctionPtr)] ApolloLogDelegate callback);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern void apollo_setLogLevel(ApolloLogPriority pri);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool apollo_switchplugin([MarshalAs(UnmanagedType.LPStr)] string pluginName);
        public override IApolloConnector CreateApolloConnection(ApolloPlatform platform, string url)
        {
            return this.CreateApolloConnection(platform, (uint) 0xffffff, url);
        }

        [Obsolete("Deprecated since 1.1.2, use CreateApolloConnection(ApolloPlatform platform, UInt32 permission, String svrUrl) instead")]
        public override IApolloConnector CreateApolloConnection(ApolloPlatform platform, ApolloPermission permission, string url)
        {
            return this.CreateApolloConnection(platform, (uint) permission, url);
        }

        [Obsolete("Deprecated since 1.1.13, use CreateApolloConnection(ApolloPlatform platform,  String svrUrl) instead")]
        public override IApolloConnector CreateApolloConnection(ApolloPlatform platform, uint permission, string url)
        {
            ADebug.Log("CreateApolloConnection");
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("Argument Error: url can not be null or empty");
            }
            if (ApolloCommon.ApolloInfo == null)
            {
                throw new Exception("IApollo.Instance.Initialize must be called first!");
            }
            if (platform == ApolloPlatform.None)
            {
            }
            ApolloConnector connector = new ApolloConnector();
            ApolloResult result = connector.Initialize(platform, permission, url);
            if (result != ApolloResult.Success)
            {
                throw new Exception("connector Initialize Error:" + result);
            }
            return connector;
        }

        public override IApolloHttpClient CreateHttpClient()
        {
            if (ApolloCommon.ApolloInfo == null)
            {
                throw new Exception("IApollo.Instance.Initialize must be called first!");
            }
            return new ApolloHttpClient();
        }

        public override IApolloTalker CreateTalker(IApolloConnector connector)
        {
            if (ApolloCommon.ApolloInfo == null)
            {
                throw new Exception("IApollo.Instance.Initialize must be called first!");
            }
            return new ApolloTalker(connector);
        }

        public override void DestoryHttpClient(IApolloHttpClient client)
        {
        }

        public override void DestroyApolloConnector(IApolloConnector connector)
        {
            ADebug.Log("DestroyApolloConnector");
            if (connector != null)
            {
                ApolloConnector connector2 = connector as ApolloConnector;
                if (connector2 != null)
                {
                    connector2.Destroy();
                }
            }
        }

        public override void DestroyTalker(IApolloTalker talker)
        {
            ApolloTalker talker2 = talker as ApolloTalker;
            if (talker2 != null)
            {
                talker2.Destroy();
            }
        }

        ~Apollo()
        {
        }

        public override IApolloAccountService GetAccountService()
        {
            return ApolloAccountService.Instance;
        }

        public override IApolloServiceBase GetService(int type)
        {
            switch (type)
            {
                case 0x3e8:
                    return TssService.Instance;

                case 0x3e9:
                    return ApolloNetworkService.Intance;

                case 2:
                    return ApolloPayService.Instance;
            }
            PluginBase currentPlugin = PluginManager.Instance.GetCurrentPlugin();
            if (currentPlugin == null)
            {
                return null;
            }
            return currentPlugin.GetService(type);
        }

        public static System.Type GetType2(string typeName)
        {
            System.Type type = UtilityPlugin.GetType(typeName);
            if (type != null)
            {
                return type;
            }
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName);
                if (type != null)
                {
                    Debug.Log("GetType2 find " + typeName + "FullName= " + assembly.FullName);
                    return type;
                }
            }
            return null;
        }

        public override ApolloResult Initialize(ApolloInfo platformInfo)
        {
            ApolloCommon.ApolloInfo = platformInfo;
            if (platformInfo == null)
            {
                throw new Exception("ApolloInfo could not be null!!");
            }
            ADebug.Log(string.Format("Apollo Init QQAppId:{0}, WXAppId:{1}, pluginName:{2}", platformInfo.QQAppId, platformInfo.WXAppId, platformInfo.PluginName));
            if (string.IsNullOrEmpty(platformInfo.PluginName))
            {
                ApolloCommon.ApolloInfo.PluginName = "MSDK";
            }
            MsdkAdapter.InnerInstall();
            return (ApolloResult) apollo_init(platformInfo.ServiceId, platformInfo.MaxMessageBufferSize, ApolloCommon.ApolloInfo.PluginName);
        }

        [MonoPInvokeCallback(typeof(ApolloLogDelegate))]
        private static void OnApolloLogDelegate(ApolloLogPriority pri, IntPtr msg)
        {
            Apollo.Apollo instance = IApollo.Instance as Apollo.Apollo;
            if (instance.logEvent != null)
            {
                instance.logEvent(pri, Marshal.PtrToStringAnsi(msg));
            }
        }

        public override void SetApolloLogger(ApolloLogPriority pri, ApolloLogHandler handler)
        {
            ADebug.Log("SetApolloLogger");
            this.logEvent = handler;
            apollo_setApolloLogger(pri, new ApolloLogDelegate(Apollo.Apollo.OnApolloLogDelegate));
        }

        public override bool SwitchPlugin(string pluginName)
        {
            return apollo_switchplugin(pluginName);
        }
    }
}

