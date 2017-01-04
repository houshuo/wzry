namespace Apollo.Plugins.Msdk
{
    using Apollo;
    using System;
    using System.Runtime.InteropServices;

    public class MsdkAdapter : PluginBase
    {
        public static MsdkAdapter Instance = new MsdkAdapter();

        private MsdkAdapter()
        {
        }

        public override ApolloBufferBase CreatePayResponseInfo(int action)
        {
            return new ApolloPayResponseInfo();
        }

        public override IApolloExtendPayServiceBase GetPayExtendService()
        {
            return MsdkPayExtendService.Instance;
        }

        public override string GetPluginName()
        {
            return "MSDK";
        }

        public override IApolloServiceBase GetService(int serviceType)
        {
            switch (serviceType)
            {
                case 1:
                    return ApolloSnsService.Instance;

                case 2:
                    return ApolloPayService.Instance;

                case 3:
                    return ApolloReportService.Instance;

                case 5:
                    return Notice.Instance;

                case 6:
                    return ApolloLbsService.Instance;

                case 7:
                    return ApolloQuickLoginService.Instance;

                case 8:
                    return ApolloCommonService.Instance;
            }
            return null;
        }

        public static bool InnerInstall()
        {
            ADebug.Log("InnerInstall");
            return Instance.Install();
        }

        public override bool Install()
        {
            msdk_adapter_install();
            IApolloReportService service = IApollo.Instance.GetService(3) as IApolloReportService;
            IApolloCommonService service2 = IApollo.Instance.GetService(8) as IApolloCommonService;
            if (service2 != null)
            {
                service2.PushInit();
            }
            return true;
        }

        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void msdk_adapter_install();
    }
}

