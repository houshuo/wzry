using Mono.Xml;
using System;
using System.Collections;
using System.IO;
using System.Security;
using System.Text;
using UnityEngine;

public class TdirConfig
{
    public static int AppID_android = 0x9e00011;
    public static int AppID_android_competition = 0x9e0009e;
    public static int AppID_iOS = 0x9e00012;
    public static int AppID_iOS_competition = 0x9e0009f;
    public static TdirServerType cheatServerType = TdirServerType.Normal;
    public static TdirServerType curServerType = TdirServerType.NULL;
    public static string[] iplist_competition_official = new string[] { "exp.mtcls.qq.com", "61.151.234.47", "182.254.42.103", "140.207.62.111", "140.207.123.164", "117.144.242.28", "117.135.171.74", "103.7.30.91", "101.227.130.79" };
    public static string[] iplist_competition_test = new string[] { "testa4.mtcls.qq.com", "101.227.153.83" };
    public static string[] iplist_experience = new string[] { "exp.mtcls.qq.com", "61.151.234.47", "182.254.42.103", "140.207.62.111", "140.207.123.164", "117.144.242.28", "117.135.171.74", "103.7.30.91", "101.227.130.79" };
    public static string[] iplist_experience_test = new string[] { "testb4.mtcls.qq.com", "101.227.153.86" };
    public static string[] iplist_middle = new string[] { "middle.mtcls.qq.com", "101.226.141.88" };
    public static string[] iplist_normal = new string[] { "mtcls.qq.com", "61.151.224.100", "58.251.61.169", "203.205.151.237", "203.205.147.178", "183.61.49.177", "183.232.103.166", "182.254.4.176", "182.254.10.82", "140.207.127.61", "117.144.242.115" };
    public static string[] iplist_normal_tongcai = new string[] { "ft.smoba.qq.com", "101.226.76.200", "117.135.172.223", "140.206.160.117", "182.254.11.206" };
    public static string[] iplist_test = new string[] { "testa4.mtcls.qq.com", "101.227.153.83" };
    public static string[] iplist_testForTester = new string[] { "testc.mtcls.qq.com", "183.61.39.51" };
    public static int[] portlist_competition_official = new int[] { 0x271c, 0x271e, 0x2720 };
    public static int[] portlist_competition_test = new int[] { 0x2712, 0x2714, 0x2716 };
    public static int[] portlist_experience = new int[] { 0x2712, 0x2714, 0x2716 };
    public static int[] portlist_experience_test = new int[] { 0x2712, 0x2714, 0x2716 };
    public static int[] portlist_middle = new int[] { 0x4e22, 0x4e24, 0x4e26 };
    public static int[] portlist_normal = new int[] { 0xc35c, 0xc35e, 0xc360 };
    public static int[] portlist_normal_tongcai = new int[] { 0xc35c, 0xc35e, 0xc360 };
    public static int[] portlist_test = new int[] { 0x2712, 0x2714, 0x2716 };
    public static int[] portlist_testForTester = new int[] { 0x2712, 0x2714, 0x2716 };
    private static TdirConfigData tdirConfigData = null;
    public static string tdirConfigDataPath = "/TdirConfigData.xml";
    public static TdirServerType WoYaoQiehuanJing = TdirServerType.NULL;

    public static TdirConfigData GetFileTdirAndTverData()
    {
        if ((tdirConfigData == null) && File.Exists(Application.persistentDataPath + tdirConfigDataPath))
        {
            try
            {
                byte[] bytes = CFileManager.ReadFile(Application.persistentDataPath + tdirConfigDataPath);
                if ((bytes != null) && (bytes.Length > 0))
                {
                    tdirConfigData = new TdirConfigData();
                    string xml = Encoding.UTF8.GetString(bytes);
                    Mono.Xml.SecurityParser parser = new Mono.Xml.SecurityParser();
                    parser.LoadXml(xml);
                    IEnumerator enumerator = parser.ToXml().Children.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            SecurityElement current = (SecurityElement) enumerator.Current;
                            if (current.Tag == "serverType")
                            {
                                tdirConfigData.serverType = int.Parse(current.Text);
                            }
                            else if (current.Tag == "versionType")
                            {
                                tdirConfigData.versionType = int.Parse(current.Text);
                            }
                        }
                    }
                    finally
                    {
                        IDisposable disposable = enumerator as IDisposable;
                        if (disposable == null)
                        {
                        }
                        disposable.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                tdirConfigData = null;
            }
        }
        return tdirConfigData;
    }

    public static int GetTdirAppId()
    {
        if ((curServerType != TdirServerType.CompetitionTest) && (curServerType != TdirServerType.CompetitionOfficial))
        {
            return AppID_android;
        }
        return AppID_android_competition;
    }

    public static string[] GetTdirIPList()
    {
        TdirConfigData fileTdirAndTverData = GetFileTdirAndTverData();
        if (fileTdirAndTverData != null)
        {
            curServerType = (TdirServerType) fileTdirAndTverData.serverType;
            if (fileTdirAndTverData.serverType == 1)
            {
                return iplist_test;
            }
            if (fileTdirAndTverData.serverType == 2)
            {
                return iplist_middle;
            }
            if (fileTdirAndTverData.serverType == 3)
            {
                if (MonoSingleton<CTongCaiSys>.GetInstance().IsCanUseTongCai())
                {
                    return iplist_normal_tongcai;
                }
                return iplist_normal;
            }
            if (fileTdirAndTverData.serverType == 4)
            {
                return iplist_experience;
            }
            if (fileTdirAndTverData.serverType == 5)
            {
                return iplist_experience_test;
            }
            if (fileTdirAndTverData.serverType == 6)
            {
                return iplist_testForTester;
            }
            if (fileTdirAndTverData.serverType == 7)
            {
                return iplist_competition_test;
            }
            if (fileTdirAndTverData.serverType == 8)
            {
                return iplist_competition_official;
            }
        }
        curServerType = cheatServerType;
        if (cheatServerType == TdirServerType.Test)
        {
            return iplist_test;
        }
        if (cheatServerType == TdirServerType.Mid)
        {
            return iplist_middle;
        }
        if (cheatServerType == TdirServerType.Normal)
        {
            if (MonoSingleton<CTongCaiSys>.GetInstance().IsCanUseTongCai())
            {
                return iplist_normal_tongcai;
            }
            return iplist_normal;
        }
        if (cheatServerType == TdirServerType.Exp)
        {
            return iplist_experience;
        }
        if (cheatServerType == TdirServerType.ExpTest)
        {
            return iplist_experience_test;
        }
        if (cheatServerType == TdirServerType.TestForTester)
        {
            return iplist_testForTester;
        }
        if (cheatServerType == TdirServerType.CompetitionTest)
        {
            return iplist_competition_test;
        }
        if (cheatServerType == TdirServerType.CompetitionOfficial)
        {
            return iplist_competition_official;
        }
        curServerType = TdirServerType.Normal;
        if (MonoSingleton<CTongCaiSys>.GetInstance().IsCanUseTongCai())
        {
            return iplist_normal_tongcai;
        }
        return iplist_normal;
    }

    public static int[] GetTdirPortList()
    {
        TdirConfigData fileTdirAndTverData = GetFileTdirAndTverData();
        if (fileTdirAndTverData != null)
        {
            if (fileTdirAndTverData.serverType == 1)
            {
                return portlist_test;
            }
            if (fileTdirAndTverData.serverType == 2)
            {
                return portlist_middle;
            }
            if (fileTdirAndTverData.serverType == 3)
            {
                if (MonoSingleton<CTongCaiSys>.GetInstance().IsCanUseTongCai())
                {
                    return portlist_normal_tongcai;
                }
                return portlist_normal;
            }
            if (fileTdirAndTverData.serverType == 4)
            {
                return portlist_experience;
            }
            if (fileTdirAndTverData.serverType == 5)
            {
                return portlist_experience_test;
            }
            if (fileTdirAndTverData.serverType == 6)
            {
                return portlist_testForTester;
            }
            if (fileTdirAndTverData.serverType == 7)
            {
                return portlist_competition_test;
            }
            if (fileTdirAndTverData.serverType == 8)
            {
                return portlist_competition_official;
            }
        }
        if (cheatServerType == TdirServerType.Test)
        {
            return portlist_test;
        }
        if (cheatServerType == TdirServerType.Mid)
        {
            return portlist_middle;
        }
        if (cheatServerType == TdirServerType.Normal)
        {
            if (MonoSingleton<CTongCaiSys>.GetInstance().IsCanUseTongCai())
            {
                return portlist_normal_tongcai;
            }
            return portlist_normal;
        }
        if (cheatServerType == TdirServerType.Exp)
        {
            return portlist_experience;
        }
        if (cheatServerType == TdirServerType.ExpTest)
        {
            return portlist_experience_test;
        }
        if (cheatServerType == TdirServerType.TestForTester)
        {
            return portlist_testForTester;
        }
        if (cheatServerType == TdirServerType.CompetitionTest)
        {
            return portlist_competition_test;
        }
        if (cheatServerType == TdirServerType.CompetitionOfficial)
        {
            return portlist_competition_official;
        }
        if (MonoSingleton<CTongCaiSys>.GetInstance().IsCanUseTongCai())
        {
            return portlist_normal_tongcai;
        }
        return portlist_normal;
    }
}

