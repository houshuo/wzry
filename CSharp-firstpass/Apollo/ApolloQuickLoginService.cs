namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class ApolloQuickLoginService : ApolloObject, IApolloQuickLoginService, IApolloServiceBase
    {
        public static readonly ApolloQuickLoginService Instance = new ApolloQuickLoginService();
        private static ApolloQuickLoginNotify m_callback = null;

        private ApolloQuickLoginService()
        {
        }

        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool apollo_account_GetWakeupInfo([MarshalAs(UnmanagedType.LPStr)] StringBuilder pAccountInfo, int size);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void apollo_account_SetQuickLoginBaseCallback([MarshalAs(UnmanagedType.FunctionPtr)] ApolloQuickLoginBaseDelegate callback);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void apollo_account_SwitchUser(bool useExternalAccount);
        [MonoPInvokeCallback(typeof(ApolloQuickLoginBaseDelegate))]
        private static void QuickLoginOccur()
        {
            ADebug.Log("C# Apollo pulled up by quicklogin");
            ApolloWakeupInfo wakeupInfo = new ApolloWakeupInfo();
            bool flag = s_GetWakeupInfo(out wakeupInfo);
            if (m_callback != null)
            {
                m_callback(wakeupInfo);
            }
            else
            {
                ADebug.Log("QuickLoginOccur m_callback is null");
            }
        }

        private static bool s_GetWakeupInfo(out ApolloWakeupInfo wakeupInfo)
        {
            wakeupInfo = null;
            StringBuilder pAccountInfo = new StringBuilder(0x5000);
            bool flag = apollo_account_GetWakeupInfo(pAccountInfo, 0x5000);
            ADebug.Log("s_GetWakeupInfo : " + flag);
            if (flag)
            {
                string src = pAccountInfo.ToString();
                ADebug.Log("s_GetWakeupInfo: " + src);
                if ((src != null) && (src.Length > 0))
                {
                    ApolloStringParser parser = new ApolloStringParser(src);
                    if (parser != null)
                    {
                        wakeupInfo = parser.GetObject<ApolloWakeupInfo>("WakeupInfo");
                        if (wakeupInfo != null)
                        {
                            ADebug.Log("s_GetWakeupInfo parser.GetObject success");
                            return true;
                        }
                        ADebug.Log("s_GetWakeupInfo parser.GetObject error");
                    }
                    else
                    {
                        ADebug.Log("GetWakeupInfo parser.GetObjec error");
                    }
                }
            }
            return false;
        }

        public void SetQuickLoginNotify(ApolloQuickLoginNotify callback)
        {
            ADebug.Log("C# ApolloQuickLoginService::SetCallback");
            m_callback = callback;
            apollo_account_SetQuickLoginBaseCallback(new ApolloQuickLoginBaseDelegate(ApolloQuickLoginService.QuickLoginOccur));
        }

        public void SwitchUser(bool useExternalAccount)
        {
            apollo_account_SwitchUser(useExternalAccount);
        }

        private delegate void ApolloQuickLoginBaseDelegate();
    }
}

