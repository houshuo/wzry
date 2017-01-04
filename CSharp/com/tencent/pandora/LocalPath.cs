namespace com.tencent.pandora
{
    using System;
    using System.IO;
    using UnityEngine;

    public class LocalPath
    {
        private static string path;

        public static string GetCurFilePath(string readtype)
        {
            if (string.IsNullOrEmpty(path))
            {
                SetSelfPath();
            }
            if (readtype.ToLower() == "io")
            {
                return path;
            }
            if (!(readtype.ToLower() == "www"))
            {
                return string.Empty;
            }
            if (path.StartsWith(Application.streamingAssetsPath))
            {
                return path;
            }
            return ("file://" + path);
        }

        public static void SetPathWithSA()
        {
            path = Application.streamingAssetsPath + "/vercache/";
        }

        private static void SetSelfPath()
        {
            if ((!string.IsNullOrEmpty(Configer.m_CurHotUpdatePath) && Directory.Exists(Configer.m_CurHotUpdatePath)) && (Directory.GetFiles(Configer.m_CurHotUpdatePath).Length > 0))
            {
                path = Configer.m_CurHotUpdatePath;
            }
            else
            {
                path = Application.streamingAssetsPath + "/vercache/";
            }
        }
    }
}

