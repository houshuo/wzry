namespace behaviac
{
    using System;
    using System.IO;
    using UnityEngine;

    public class FileManager
    {
        private static FileManager ms_instance;

        public FileManager()
        {
            ms_instance = this;
        }

        public virtual void FileClose(string filePath, string ext, byte[] pBuffer)
        {
        }

        public virtual bool FileExist(string filePath, string ext)
        {
            return File.Exists(filePath + ext);
        }

        public virtual byte[] FileOpen(string filePath, string ext)
        {
            try
            {
                filePath = filePath + ext;
                int index = filePath.IndexOf("Resources");
                if (index != -1)
                {
                    index += 10;
                    string fullPathInResources = filePath.Substring(index);
                    CBinaryObject content = Singleton<CResourceManager>.GetInstance().GetResource(fullPathInResources, typeof(TextAsset), enResourceType.BattleScene, false, false).m_content as CBinaryObject;
                    if (content == null)
                    {
                        string str2 = string.Format("FileManager::FileOpen failed:'{0}' not loaded", filePath);
                        return null;
                    }
                    return content.m_data;
                }
                string str3 = string.Format("FileManager::FileOpen failed:'{0}' should be in /Resources", filePath);
            }
            catch
            {
                string str4 = string.Format("FileManager::FileOpen exception:'{0}'", filePath);
            }
            return null;
        }

        public static FileManager Instance
        {
            get
            {
                if (ms_instance == null)
                {
                    ms_instance = new FileManager();
                }
                return ms_instance;
            }
        }
    }
}

