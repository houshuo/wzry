namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class ApolloCommonService : ApolloObject, IApolloCommonService, IApolloServiceBase
    {
        public static readonly ApolloCommonService Instance = new ApolloCommonService();

        public event OnCrashExtMessageNotifyHandle onCrashExtMessageEvent;

        public event OnFeedbackNotifyHandle onFeedbackEvent;

        public event OnReceivedPushNotifyHandle onReceivedPushEvent;

        private ApolloCommonService()
        {
        }

        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void apollo_common_Feedback([MarshalAs(UnmanagedType.LPStr)] string body);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void apollo_common_GetChannelId([MarshalAs(UnmanagedType.LPStr)] StringBuilder ChannelId, int size);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void apollo_common_GetRegisterChannelId([MarshalAs(UnmanagedType.LPStr)] StringBuilder RegisterChannelId, int size);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool apollo_common_OpenAmsCenter([MarshalAs(UnmanagedType.LPStr)] string param);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void apollo_common_OpenUrl([MarshalAs(UnmanagedType.LPStr)] string openUrl);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void apollo_common_OpenWeiXinDeeplink([MarshalAs(UnmanagedType.LPStr)] string link);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void apollo_common_PushInit();
        public void Feedback(string body)
        {
            apollo_common_Feedback(body);
        }

        public string GetChannelId()
        {
            StringBuilder channelId = new StringBuilder(0x80);
            apollo_common_GetChannelId(channelId, 0x80);
            return channelId.ToString();
        }

        public string GetRegisterChannelId()
        {
            StringBuilder registerChannelId = new StringBuilder(0x80);
            apollo_common_GetRegisterChannelId(registerChannelId, 0x80);
            return registerChannelId.ToString();
        }

        private void OnCrashExtMessageNotify(string msg)
        {
            try
            {
                if (this.onCrashExtMessageEvent != null)
                {
                    this.onCrashExtMessageEvent();
                }
            }
            catch (Exception exception)
            {
                ADebug.LogError("OnCrashExtMessageNotify" + exception);
            }
        }

        private void OnFeedbackNotify(string msg)
        {
            ADebug.Log("onFeedbackEvent:" + msg);
            if (this.onFeedbackEvent != null)
            {
                ApolloStringParser parser = new ApolloStringParser(msg);
                int @int = parser.GetInt("Flag");
                string desc = parser.GetString("Desc");
                try
                {
                    this.onFeedbackEvent(@int, desc);
                }
                catch (Exception exception)
                {
                    ADebug.Log("onFeedbackEvent:" + exception);
                }
            }
        }

        private void OnReceivedPushNotify(string msg)
        {
            ADebug.Log("OnReceivedPushNotify:" + msg);
            if (this.onReceivedPushEvent != null)
            {
                try
                {
                    this.onReceivedPushEvent(msg);
                }
                catch (Exception exception)
                {
                    ADebug.Log("onReceivedPushEvent:" + exception);
                }
            }
        }

        public bool OpenAmsCenter(string param)
        {
            return apollo_common_OpenAmsCenter(param);
        }

        public void OpenUrl(string openUrl)
        {
            apollo_common_OpenUrl(openUrl);
        }

        public void OpenWeiXinDeeplink(string link)
        {
            apollo_common_OpenWeiXinDeeplink(link);
        }

        public void PushInit()
        {
            apollo_common_PushInit();
        }
    }
}

