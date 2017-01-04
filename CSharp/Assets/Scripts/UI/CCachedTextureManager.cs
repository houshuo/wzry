namespace Assets.Scripts.UI
{
    using System;
    using System.IO;
    using UnityEngine;

    public class CCachedTextureManager
    {
        private const int c_cachedTextureMaxAmount = 100;
        private CCachedTextureInfoSet m_cachedTextureInfoSet = new CCachedTextureInfoSet();
        private static byte[] s_buffer = new byte[0x2710];
        private static string s_cachedTextureDirectory = CFileManager.CombinePath(CFileManager.GetCachePath(), "HttpImage");
        private static string s_cachedTextureInfoSetFileFullPath = CFileManager.CombinePath(s_cachedTextureDirectory, "httpimage.bytes");

        public CCachedTextureManager()
        {
            if (!CFileManager.IsDirectoryExist(s_cachedTextureDirectory))
            {
                CFileManager.CreateDirectory(s_cachedTextureDirectory);
            }
            if (CFileManager.IsFileExist(s_cachedTextureInfoSetFileFullPath))
            {
                byte[] data = CFileManager.ReadFile(s_cachedTextureInfoSetFileFullPath);
                int offset = 0;
                this.m_cachedTextureInfoSet.Read(data, ref offset);
            }
        }

        public void AddCachedTexture(string url, int width, int height, bool isGif, byte[] data)
        {
            string key = CFileManager.GetMd5(url.ToLower());
            if (this.m_cachedTextureInfoSet.m_cachedTextureInfoMap.ContainsKey(key))
            {
                CCachedTextureInfo info = null;
                this.m_cachedTextureInfoSet.m_cachedTextureInfoMap.TryGetValue(key, out info);
                DebugHelper.Assert(this.m_cachedTextureInfoSet.m_cachedTextureInfos.Contains(info), "zen me ke neng?");
                info.m_width = width;
                info.m_height = height;
                info.m_lastModifyTime = DateTime.Now;
                info.m_isGif = isGif;
            }
            else
            {
                if (this.m_cachedTextureInfoSet.m_cachedTextureInfos.Count >= 100)
                {
                    string str2 = this.m_cachedTextureInfoSet.RemoveEarliestTextureInfo();
                    if (!string.IsNullOrEmpty(str2))
                    {
                        string str3 = CFileManager.CombinePath(s_cachedTextureDirectory, str2 + ".bytes");
                        if (CFileManager.IsFileExist(str3))
                        {
                            CFileManager.DeleteFile(str3);
                        }
                    }
                }
                CCachedTextureInfo cachedTextureInfo = new CCachedTextureInfo {
                    m_key = key,
                    m_width = width,
                    m_height = height,
                    m_lastModifyTime = DateTime.Now,
                    m_isGif = isGif
                };
                this.m_cachedTextureInfoSet.AddTextureInfo(key, cachedTextureInfo);
            }
            this.m_cachedTextureInfoSet.SortTextureInfo();
            int offset = 0;
            this.m_cachedTextureInfoSet.Write(s_buffer, ref offset);
            if (CFileManager.IsFileExist(s_cachedTextureInfoSetFileFullPath))
            {
                CFileManager.DeleteFile(s_cachedTextureInfoSetFileFullPath);
            }
            CFileManager.WriteFile(s_cachedTextureInfoSetFileFullPath, s_buffer, 0, offset);
            string filePath = CFileManager.CombinePath(s_cachedTextureDirectory, key + ".bytes");
            if (CFileManager.IsFileExist(filePath))
            {
                CFileManager.DeleteFile(filePath);
            }
            CFileManager.WriteFile(filePath, data);
        }

        public Texture2D GetCachedTexture(string url, float validDays)
        {
            string key = CFileManager.GetMd5(url.ToLower());
            CCachedTextureInfo cachedTextureInfo = this.m_cachedTextureInfoSet.GetCachedTextureInfo(key);
            if (cachedTextureInfo != null)
            {
                TimeSpan span = (TimeSpan) (DateTime.Now - cachedTextureInfo.m_lastModifyTime);
                if (span.TotalDays < validDays)
                {
                    string filePath = CFileManager.CombinePath(s_cachedTextureDirectory, key + ".bytes");
                    if (!CFileManager.IsFileExist(filePath))
                    {
                        return null;
                    }
                    byte[] buffer = CFileManager.ReadFile(filePath);
                    if ((buffer == null) || (buffer.Length <= 0))
                    {
                        return null;
                    }
                    Texture2D textured = null;
                    if (cachedTextureInfo.m_isGif)
                    {
                        using (MemoryStream stream = new MemoryStream(buffer))
                        {
                            return GifHelper.GifToTexture(stream, 0);
                        }
                    }
                    textured = new Texture2D(cachedTextureInfo.m_width, cachedTextureInfo.m_height, TextureFormat.ARGB32, false);
                    textured.LoadImage(buffer);
                    return textured;
                }
            }
            return null;
        }
    }
}

