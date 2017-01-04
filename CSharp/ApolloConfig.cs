using Apollo;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class ApolloConfig
{
    public static string appID = "1104466820";
    public static string CustomOpenId = string.Empty;
    public static ushort echoPort;
    public static int ISPType;
    public static string loginHostName = "login.smoba.qq.com";
    public static string loginOnlyIp = string.Empty;
    public static string loginOnlyIpOrUrl = string.Empty;
    public static string loginOnlyIpTongCai = string.Empty;
    public static ushort loginOnlyVPort = 0x19e5;
    public static string loginUrl = string.Empty;
    public static int maxMessageBufferSize = 0x7d000;
    public static string offerID = "1450002258";
    public static string Password = string.Empty;
    public static bool payEnabled = true;
    public static string payEnv = "release";
    public static ApolloPlatform platform = ApolloPlatform.Wechat;
    public static string qq_android_port = "60612";
    public static string qq_ios_port = "60622";
    public static string QQAppID = "1104466820";
    public static string QQAppKey = "LX7Pi1KzPZJD9LlL";
    public static string serverUrlPath = "server_url.txt";
    public static string serviceID = "10056";
    public static ulong Uin;
    public static string WXAppID = "wx95a3a4d7c627e07d";
    public static string WXAppKey = "5018f9d67e25213f4f33d546c74624ba";

    public static string GetGameUtilityString()
    {
        return "com.tencent.tmgp.sgame.SGameUtility";
    }

    public static string GetPackageName()
    {
        return "com.tencent.tmgp.sgame";
    }

    public static bool IsUseCEPackage()
    {
        Debug.Log("normal don't usecepakage");
        return false;
    }

    public static string[] LoadLoginUrl()
    {
        CBinaryObject content = Singleton<CResourceManager>.GetInstance().GetResource(serverUrlPath, typeof(TextAsset), enResourceType.Numeric, false, false).m_content as CBinaryObject;
        if (content == null)
        {
            Debug.LogError(string.Format("Can't find file: {0}", serverUrlPath));
            Singleton<CResourceManager>.GetInstance().RemoveCachedResource(serverUrlPath);
            return null;
        }
        string str = Encoding.UTF8.GetString(content.m_data);
        Singleton<CResourceManager>.GetInstance().RemoveCachedResource(serverUrlPath);
        string[] strArray = str.Split(new char[] { '\r', '\t', ' ', '\n', ';', '\0' }, StringSplitOptions.RemoveEmptyEntries);
        List<string> list = new List<string>();
        for (int i = 0; i < strArray.Length; i++)
        {
            string item = strArray[i];
            char[] separator = new char[] { '.' };
            string[] strArray2 = item.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (strArray2.Length == 4)
            {
                bool flag = true;
                for (int j = 0; j < strArray2.Length; j++)
                {
                    try
                    {
                        Convert.ToUInt16(strArray2[j]);
                    }
                    catch
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    list.Add(item);
                }
            }
        }
        if (list.Count != 3)
        {
            Debug.LogError(string.Format("Invalid server list file: {0}", serverUrlPath));
            return null;
        }
        return list.ToArray();
    }
}

