namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;

    internal class ApolloAccountService : ApolloObject, IApolloAccountService, IApolloServiceBase
    {
        public static readonly ApolloAccountService Instance = new ApolloAccountService();

        public event AccountInitializeHandle InitializeEvent;

        public event AccountLoginHandle LoginEvent;

        public event AccountLogoutHandle LogoutEvent;

        public event RefreshAccessTokenHandler RefreshAtkEvent;

        private ApolloAccountService()
        {
        }

        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Ansi)]
        private static extern ApolloResult apollo_account_getRecord(ulong objId, [MarshalAs(UnmanagedType.LPStr)] StringBuilder pAccountInfo, int size);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool apollo_account_initialize([MarshalAs(UnmanagedType.LPArray)] byte[] data, int len);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Ansi)]
        private static extern bool apollo_account_IsPlatformInstalled(ApolloPlatform platformType);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Ansi)]
        private static extern bool apollo_account_IsPlatformSupportApi(ApolloPlatform platformType);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern void apollo_account_login(ulong objId, ApolloPlatform platform);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern void apollo_account_logout(ulong objId);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern void apollo_account_refreshAtk(ulong objId);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern void apollo_account_reset();
        public ApolloResult GetRecord(ref ApolloAccountInfo pAccountInfo)
        {
            StringBuilder builder = new StringBuilder(0x1000);
            ApolloResult result = apollo_account_getRecord(base.ObjectId, builder, 0x1000);
            string src = builder.ToString();
            ADebug.Log(string.Concat(new object[] { "GetRecord:", result, ", ", src }));
            if (src.Length > 0)
            {
                pAccountInfo.FromString(src);
            }
            return result;
        }

        public bool Initialize(ApolloBufferBase initInfo)
        {
            if (initInfo != null)
            {
                byte[] buffer;
                initInfo.Encode(out buffer);
                if (buffer != null)
                {
                    return apollo_account_initialize(buffer, buffer.Length);
                }
                ADebug.LogError("Account Initialize Encode Error");
            }
            else
            {
                ADebug.LogError("Account Initialize param is null");
            }
            return false;
        }

        public bool IsPlatformInstalled(ApolloPlatform platform)
        {
            return apollo_account_IsPlatformInstalled(platform);
        }

        public bool IsPlatformSupportApi(ApolloPlatform platform)
        {
            return apollo_account_IsPlatformSupportApi(platform);
        }

        public void Login(ApolloPlatform platform)
        {
            ADebug.Log("Login");
            apollo_account_login(base.ObjectId, platform);
        }

        public void Logout()
        {
            apollo_account_logout(base.ObjectId);
        }

        private void onAccessTokenRefresedProc(string msg)
        {
            ADebug.Log("onAccessTokenRefresedProc: " + msg);
            ApolloStringParser parser = new ApolloStringParser(msg);
            ListView<ApolloToken> tokenList = null;
            ApolloResult @int = (ApolloResult) parser.GetInt("Result");
            if (@int == ApolloResult.Success)
            {
                string src = parser.GetString("tokens");
                if (src != null)
                {
                    src = ApolloStringParser.ReplaceApolloString(src);
                    ADebug.Log("onAccessTokenRefresedProc tokens:" + src);
                    if ((src != null) && (src.Length > 0))
                    {
                        char[] separator = new char[] { ',' };
                        string[] strArray = src.Split(separator);
                        tokenList = new ListView<ApolloToken>();
                        foreach (string str2 in strArray)
                        {
                            string str3 = ApolloStringParser.ReplaceApolloString(ApolloStringParser.ReplaceApolloString(str2));
                            ApolloToken item = new ApolloToken();
                            item.FromString(str3);
                            ADebug.Log(string.Format("onAccessTokenRefresedProc str:{0} |||||| {1}   |||||{2}", str3, item.Type, item.Value));
                            tokenList.Add(item);
                        }
                    }
                }
            }
            if (this.RefreshAtkEvent != null)
            {
                this.RefreshAtkEvent(@int, tokenList);
            }
        }

        private void OnAccountInitializeProc(int ret, byte[] buf)
        {
            ApolloResult result = (ApolloResult) ret;
            ADebug.Log("OnAccountInitializeProc result:" + result);
            if (this.InitializeEvent != null)
            {
                try
                {
                    this.InitializeEvent(result, null);
                }
                catch (Exception exception)
                {
                    ADebug.LogError("OnAccountInitializeProc:" + exception);
                }
            }
        }

        private void OnAccountLogoutProc(int ret)
        {
            ApolloResult result = (ApolloResult) ret;
            ADebug.Log("OnAccountLogoutProc result:" + result);
            if (this.LogoutEvent != null)
            {
                try
                {
                    this.LogoutEvent(result);
                }
                catch (Exception exception)
                {
                    ADebug.LogError("OnAccountLogoutProc:" + exception);
                }
            }
        }

        private void onLoginProc(string msg)
        {
            BugLocateLogSys.Log("ApolloAccountService onLoginProc:" + msg);
            if (!string.IsNullOrEmpty(msg))
            {
                ApolloStringParser parser = new ApolloStringParser(msg);
                ApolloAccountInfo accountInfo = null;
                ApolloResult @int = (ApolloResult) parser.GetInt("Result");
                BugLocateLogSys.Log("ApolloAccountService onLoginProc: result" + @int);
                if (@int == ApolloResult.Success)
                {
                    accountInfo = parser.GetObject<ApolloAccountInfo>("AccountInfo");
                    if ((accountInfo != null) && (accountInfo.TokenList != null))
                    {
                        BugLocateLogSys.Log(string.Concat(new object[] { "C# onLoginProc|", @int, " platform:", accountInfo.Platform, " openid:", accountInfo.OpenId, " tokensize:", accountInfo.TokenList.Count, " pf:", accountInfo.Pf, " pfkey:", accountInfo.PfKey }));
                    }
                    else
                    {
                        BugLocateLogSys.Log("parser.GetObject<ApolloAccountInfo>() return null");
                        Debug.LogError("parser.GetObject<ApolloAccountInfo>() return null");
                    }
                }
                else
                {
                    BugLocateLogSys.Log("C# onLoginProc error:" + @int);
                    DebugHelper.Assert(false, "C# onLoginProc error:" + @int);
                }
                Debug.LogWarning(string.Format("LoginEvent:{0}", this.LoginEvent));
                if (this.LoginEvent != null)
                {
                    try
                    {
                        this.LoginEvent(@int, accountInfo);
                    }
                    catch (Exception exception)
                    {
                        DebugHelper.Assert(false, "onLoginProc:" + exception);
                        BugLocateLogSys.Log("onLoginProc catch exception :" + exception.Message + "|" + exception.ToString());
                    }
                }
            }
        }

        public void RefreshAccessToken()
        {
            apollo_account_refreshAtk(base.ObjectId);
        }

        public void Reset()
        {
            apollo_account_reset();
        }

        [Obsolete("Obsolete since 1.1.6, use Initialize instead")]
        public void SetPermission(uint permission)
        {
            throw new Exception("Obsolete since 1.1.6, use Initialize instead");
        }
    }
}

