using System;
using System.IO;

public class AkBankPathUtil
{
    private static string defaultBasePath = Path.Combine("Audio", "GeneratedSoundBanks");
    private static bool isToAppendTrailingPathSeparator = true;
    private static bool isToUsePosixPathSeparator = false;

    static AkBankPathUtil()
    {
        isToUsePosixPathSeparator = false;
    }

    public static void ConvertToPosixPath(ref string path)
    {
        path.Trim();
        path = path.Replace(@"\", "/");
        char[] trimChars = new char[] { '\\' };
        path = path.TrimStart(trimChars);
    }

    public static void ConvertToWindowsCommandPath(ref string path)
    {
        path.Trim();
        path = path.Replace("/", @"\\");
        path = path.Replace(@"\", @"\\");
        char[] trimChars = new char[] { '\\' };
        path = path.TrimStart(trimChars);
    }

    public static void ConvertToWindowsPath(ref string path)
    {
        path.Trim();
        path = path.Replace("/", @"\");
        char[] trimChars = new char[] { '\\' };
        path = path.TrimStart(trimChars);
    }

    public static bool Exists(string path)
    {
        return Directory.Exists(path);
    }

    public static string GetDefaultPath()
    {
        return defaultBasePath;
    }

    public static string GetFullBasePath()
    {
        string basePath = AkInitializer.GetBasePath();
        LazyAppendTrailingSeparator(ref basePath);
        LazyConvertPathConvention(ref basePath);
        return basePath;
    }

    public static string GetPlatformBasePath()
    {
        string path = string.Empty;
        path = Path.Combine(GetFullBasePath(), GetPlatformSubDirectory());
        LazyAppendTrailingSeparator(ref path);
        LazyConvertPathConvention(ref path);
        return path;
    }

    public static string GetPlatformSubDirectory()
    {
        return "Android";
    }

    public static void LazyAppendTrailingSeparator(ref string path)
    {
        if (isToAppendTrailingPathSeparator && !path.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            path = path + Path.DirectorySeparatorChar;
        }
    }

    public static void LazyConvertPathConvention(ref string path)
    {
        if (isToUsePosixPathSeparator)
        {
            ConvertToPosixPath(ref path);
        }
        else if (Path.DirectorySeparatorChar == '/')
        {
            ConvertToPosixPath(ref path);
        }
        else
        {
            ConvertToWindowsPath(ref path);
        }
    }

    public static void SetToAppendTrailingPathSeparator(bool add)
    {
        isToAppendTrailingPathSeparator = add;
    }

    public static void UsePlatformSpecificPath()
    {
        isToUsePosixPathSeparator = false;
    }

    public static void UsePosixPath()
    {
        isToUsePosixPathSeparator = true;
    }
}

