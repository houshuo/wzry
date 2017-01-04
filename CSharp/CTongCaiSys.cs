using Apollo;
using Assets.Scripts.GameSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CTongCaiSys : MonoSingleton<CTongCaiSys>
{
    public readonly string Channel = "1100_WzryApp";
    public readonly string CheckUrlFormat = "http://chong.qq.com/tws/flowpackage/QueryOrderRelation?OutUid={0}&OutUidType={1}&Timestamp={2}&Channel=1100_WzryApp&Token={3}";
    public bool isCanUseTongCai = true;
    public bool isChecked;
    public bool isOpenTongcai = true;
    public bool isTongCaiValid;
    public readonly string ScanH5UrlFormat = "http://chong.qq.com/mobile/traffic_king.shtml?OutUid={0}&OutUidType={1}&Token={2}&Timestamp={3}&AccessToken={4}&Channel=1100_WzryApp";
    private float timeOut = 6f;
    public readonly string[] TongcaiIps = new string[] { string.Empty, "101.226.76.200", "140.206.160.117", "117.135.172.223", "182.254.11.206" };
    public readonly string TongcaiUrl = "ft.smoba.qq.com";

    [DebuggerHidden]
    public IEnumerator CheckCanUseTongCai(ApolloAccountInfo info)
    {
        return new <CheckCanUseTongCai>c__Iterator24 { info = info, <$>info = info, <>f__this = this };
    }

    public void CheckDataString(string dataString)
    {
        dataString = dataString.Replace("\"", string.Empty);
        dataString = dataString.Replace("\t", string.Empty);
        dataString = dataString.Replace("\r", string.Empty);
        dataString = dataString.Replace("\n", string.Empty);
        char[] separator = new char[] { ',' };
        string[] strArray = dataString.Split(separator);
        bool flag = false;
        for (int i = 0; i < strArray.Length; i++)
        {
            if (strArray[i].IndexOf("retCode") != -1)
            {
                char[] chArray2 = new char[] { ':' };
                string[] strArray2 = strArray[i].Split(chArray2);
                if ((strArray2.Length == 2) && strArray2[1].Trim().Equals("0"))
                {
                    flag = true;
                    break;
                }
            }
        }
        if (flag)
        {
            for (int j = 0; j < strArray.Length; j++)
            {
                if (strArray[j].IndexOf("state") != -1)
                {
                    char[] chArray3 = new char[] { ':' };
                    string[] strArray3 = strArray[j].Split(chArray3);
                    if (strArray3.Length == 2)
                    {
                        string str2 = strArray3[1].Trim();
                        if ((str2.Equals("3") || str2.Equals("4")) || str2.Equals("5"))
                        {
                            this.isTongCaiValid = true;
                        }
                        break;
                    }
                }
            }
        }
        else
        {
            this.isTongCaiValid = false;
        }
    }

    public static string DealStr(string text)
    {
        char[] chArray = text.ToCharArray();
        string str = string.Empty;
        for (int i = chArray.Length - 1; i > -1; i--)
        {
            str = str + chArray[i];
        }
        return str;
    }

    public string GetCheckUrl(ApolloAccountInfo info)
    {
        string str = (DateTime.Now.Ticks / 0x989680L).ToString();
        object[] objArray1 = new object[] { info.OpenId, (int) info.Platform, str, this.Channel, DealStr(MonoSingleton<GameFramework>.instance.tongCaiKey), DealStr(MonoSingleton<GameFramework>.instance.tongCaiKey1) };
        string str3 = Utility.CreateMD5Hash(string.Concat(objArray1)).ToLower();
        object[] args = new object[] { info.OpenId, (int) info.Platform, str, str3 };
        return string.Format(this.CheckUrlFormat, args);
    }

    protected override void Init()
    {
        this.isChecked = false;
        this.isCanUseTongCai = false;
    }

    public bool IsCanUseTongCai()
    {
        if ((TdirConfig.curServerType == TdirServerType.Normal) || (TdirConfig.curServerType == TdirServerType.Mid))
        {
            return ((this.isCanUseTongCai && this.isTongCaiValid) && (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork));
        }
        return ((ApolloConfig.loginOnlyVPort >= 0xea60) && ((this.isCanUseTongCai && this.isTongCaiValid) && (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)));
    }

    public bool IsLianTongIp()
    {
        return ApolloConfig.loginOnlyIpTongCai.Equals(this.TongcaiIps[2]);
    }

    public void OpenTongCaiH5(ApolloAccountInfo info)
    {
        string str = string.Empty;
        if (ApolloConfig.platform == ApolloPlatform.QQ)
        {
            for (int i = 0; i < info.TokenList.Count; i++)
            {
                ApolloToken token = info.TokenList[i];
                if ((token != null) && (token.Type == ApolloTokenType.Access))
                {
                    str = token.Value;
                }
            }
        }
        string str2 = (DateTime.Now.Ticks / 0x989680L).ToString();
        object[] objArray1 = new object[] { info.OpenId, (int) info.Platform, str2, this.Channel, DealStr(MonoSingleton<GameFramework>.instance.tongCaiKey), DealStr(MonoSingleton<GameFramework>.instance.tongCaiKey1) };
        string str4 = Utility.CreateMD5Hash(string.Concat(objArray1)).ToLower();
        object[] args = new object[] { info.OpenId, (int) info.Platform, str4, str2, str };
        CUICommonSystem.OpenUrl(string.Format(this.ScanH5UrlFormat, args), true);
    }

    public void StartCheck(ApolloAccountInfo info)
    {
        base.StartCoroutine(this.CheckCanUseTongCai(info));
    }

    [CompilerGenerated]
    private sealed class <CheckCanUseTongCai>c__Iterator24 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal ApolloAccountInfo <$>info;
        internal CTongCaiSys <>f__this;
        internal string <checkUrl>__2;
        internal string <dataString>__4;
        internal bool <failed>__1;
        internal float <timer>__0;
        internal WWW <www>__3;
        internal ApolloAccountInfo info;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    if (!this.<>f__this.isOpenTongcai)
                    {
                        goto Label_013D;
                    }
                    this.<timer>__0 = 0f;
                    this.<failed>__1 = false;
                    this.<>f__this.isTongCaiValid = false;
                    this.<checkUrl>__2 = this.<>f__this.GetCheckUrl(this.info);
                    this.<www>__3 = new WWW(this.<checkUrl>__2);
                    break;

                case 1:
                    break;

                default:
                    goto Label_0157;
            }
            if (!this.<www>__3.isDone)
            {
                if (this.<timer>__0 <= this.<>f__this.timeOut)
                {
                    this.<timer>__0 += Time.deltaTime;
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                this.<failed>__1 = true;
            }
            if (this.<failed>__1)
            {
                this.<www>__3.Dispose();
            }
            else if (string.IsNullOrEmpty(this.<www>__3.error))
            {
                this.<dataString>__4 = this.<www>__3.text;
                this.<>f__this.CheckDataString(this.<dataString>__4);
                this.<>f__this.isCanUseTongCai = true;
            }
            this.<>f__this.isChecked = true;
        Label_013D:
            MonoSingleton<TdirMgr>.GetInstance().TdirAsync(this.info, null, null, true);
            this.$PC = -1;
        Label_0157:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }
}

