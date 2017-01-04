using System;
using UnityEngine;

public class CVersion
{
    private static string s_androidVersionCode;
    private static string s_androidVersionCodeKey = "AndroidVersionCode";
    private static string s_appVersion;
    private static string s_buildNumber;
    private static string s_buildNumberKey = "Build";
    private static string s_codeVersion;
    private static string s_codeVersionKey = "CodeVersion";
    public static string s_emptyBuildNumber = "0000";
    public static string s_emptyResourceVersion = "0.0.0.0";
    private static string s_iOSBundleVersion;
    private static string s_iOSBundleVersionKey = "iOSBundleVersion";
    private static string s_publish;
    private static string s_publishKey = "Publish";
    private static string s_resourceVersion;
    private static string s_resourceVersionKey = "ResourceVersion";
    private static string s_revisionNumber;
    private static string s_usedResourceVersion;
    private static string s_versionTxtPathInResources = "Config/Version";

    public static int GetAndroidVersionCode()
    {
        if (s_androidVersionCode == null)
        {
            Initialize();
        }
        return int.Parse(s_androidVersionCode.Trim());
    }

    public static string GetAppVersion()
    {
        if (s_appVersion == null)
        {
            Initialize();
        }
        return s_appVersion;
    }

    public static string GetBuildNumber()
    {
        if (s_buildNumber == null)
        {
            Initialize();
        }
        return s_buildNumber;
    }

    public static string GetCodeVersion()
    {
        if (s_codeVersion == null)
        {
            Initialize();
        }
        return s_codeVersion;
    }

    public static string GetiOSBundleVersion()
    {
        if (s_iOSBundleVersion == null)
        {
            Initialize();
        }
        return s_iOSBundleVersion;
    }

    public static string GetPublish()
    {
        if (s_publish == null)
        {
            Initialize();
        }
        return s_publish;
    }

    public static string GetResourceVersion()
    {
        if (s_resourceVersion == null)
        {
            Initialize();
        }
        return s_resourceVersion;
    }

    public static string GetRevisonNumber()
    {
        if (s_revisionNumber == null)
        {
            Initialize();
        }
        return s_revisionNumber;
    }

    public static string GetUsedResourceVersion()
    {
        return s_usedResourceVersion;
    }

    public static uint GetVersionNumber(string versionStr)
    {
        uint num = 0;
        char[] separator = new char[] { '.' };
        string[] strArray = versionStr.Split(separator);
        for (int i = 0; i < strArray.Length; i++)
        {
            num += uint.Parse(strArray[i].Trim()) * ((uint) Mathf.Pow(10f, (float) (((strArray.Length - i) - 1) * 2)));
        }
        return num;
    }

    private static void Initialize()
    {
        s_publish = string.Empty;
        s_codeVersion = string.Empty;
        s_resourceVersion = string.Empty;
        s_appVersion = string.Empty;
        s_usedResourceVersion = string.Empty;
        s_buildNumber = string.Empty;
        s_revisionNumber = string.Empty;
        s_androidVersionCode = string.Empty;
        s_iOSBundleVersion = string.Empty;
        TextAsset asset = Resources.Load(s_versionTxtPathInResources, typeof(TextAsset)) as TextAsset;
        if (asset != null)
        {
            char[] separator = new char[] { '[', ']' };
            string[] strArray = asset.text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (strArray != null)
            {
                for (int i = 0; i < strArray.Length; i++)
                {
                    char[] chArray2 = new char[] { '=' };
                    string[] strArray2 = strArray[i].Split(chArray2, StringSplitOptions.RemoveEmptyEntries);
                    if ((strArray2 != null) && (strArray2.Length == 2))
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            strArray2[j] = strArray2[j].Trim();
                        }
                        if (string.Equals(strArray2[0], s_publishKey, StringComparison.OrdinalIgnoreCase))
                        {
                            s_publish = strArray2[1];
                        }
                        else if (string.Equals(strArray2[0], s_codeVersionKey, StringComparison.OrdinalIgnoreCase))
                        {
                            s_codeVersion = strArray2[1];
                        }
                        else if (string.Equals(strArray2[0], s_resourceVersionKey, StringComparison.OrdinalIgnoreCase))
                        {
                            s_resourceVersion = strArray2[1];
                        }
                        else if (string.Equals(strArray2[0], s_buildNumberKey, StringComparison.OrdinalIgnoreCase))
                        {
                            s_buildNumber = strArray2[1];
                        }
                        else if (string.Equals(strArray2[0], s_androidVersionCodeKey, StringComparison.OrdinalIgnoreCase))
                        {
                            s_androidVersionCode = strArray2[1];
                        }
                        else if (string.Equals(strArray2[0], s_iOSBundleVersionKey, StringComparison.OrdinalIgnoreCase))
                        {
                            s_iOSBundleVersion = strArray2[1];
                        }
                    }
                }
            }
        }
        if (string.IsNullOrEmpty(s_iOSBundleVersion))
        {
            s_iOSBundleVersion = s_codeVersion;
        }
        asset = Resources.Load("Revision", typeof(TextAsset)) as TextAsset;
        if (asset != null)
        {
            s_revisionNumber = asset.text;
        }
        s_appVersion = string.Format("{0}.{1}", s_codeVersion, s_resourceVersion);
        s_usedResourceVersion = s_appVersion;
    }

    public static bool IsSynchronizedVersion(string appVersion, string usedResourceVersion)
    {
        if (string.IsNullOrEmpty(appVersion) || string.IsNullOrEmpty(usedResourceVersion))
        {
            return false;
        }
        if (!string.Equals(usedResourceVersion, s_usedResourceVersion))
        {
            return false;
        }
        int length = appVersion.LastIndexOf(".");
        if (length < 0)
        {
            return false;
        }
        if (!string.Equals(appVersion.Substring(0, length), s_codeVersion))
        {
            return false;
        }
        return true;
    }

    public static void SetUsedResourceVersion(string usedResourceVersion)
    {
        s_usedResourceVersion = usedResourceVersion;
    }
}

