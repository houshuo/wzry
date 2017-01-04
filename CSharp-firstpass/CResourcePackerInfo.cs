using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class CResourcePackerInfo
{
    public const string DEFAULT_TAG = "DT";
    public AssetBundle m_assetBundle;
    public enAssetBundleState m_assetBundleState;
    public ListView<CResourcePackerInfo> m_children = new ListView<CResourcePackerInfo>();
    public Dictionary<string, string> m_fileExtMap = new Dictionary<string, string>();
    public int m_flags;
    public bool m_isAssetBundle;
    private CResourcePackerInfo m_parent;
    public string m_pathInIFS;
    public Dictionary<string, string> m_renameMap;
    public List<stResourceInfo> m_resourceInfos = new List<stResourceInfo>();
    public string m_tag = "DT";
    public bool m_useASyncLoadingData;

    public CResourcePackerInfo(bool isAssetBundle)
    {
        this.m_isAssetBundle = isAssetBundle;
        this.m_assetBundleState = enAssetBundleState.Unload;
        this.m_useASyncLoadingData = false;
    }

    public void AddResourceInfo(ref stResourceInfo resourceInfo)
    {
        this.m_resourceInfos.Add(resourceInfo);
    }

    public void AddToResourceMap(DictionaryView<string, ListView<CResourcePackerInfo>> map)
    {
        for (int i = 0; i < this.m_resourceInfos.Count; i++)
        {
            stResourceInfo info = this.m_resourceInfos[i];
            string key = info.m_fullPathInResourcesWithoutExtension.ToLower();
            stResourceInfo info2 = this.m_resourceInfos[i];
            if (info2.m_isRenamed)
            {
                if (this.m_renameMap == null)
                {
                    this.m_renameMap = new Dictionary<string, string>();
                }
                stResourceInfo info3 = this.m_resourceInfos[i];
                this.m_renameMap.Add(key, info3.m_fullPathInResourcesWithoutExtension_Renamed.ToLower());
            }
            ListView<CResourcePackerInfo> view = null;
            if (!map.TryGetValue(key, out view))
            {
                view = new ListView<CResourcePackerInfo>();
                map.Add(key, view);
            }
            view.Add(this);
        }
    }

    public bool AlreadyContainResourcePath(string fullPath)
    {
        for (int i = 0; i < this.m_resourceInfos.Count; i++)
        {
            stResourceInfo info = this.m_resourceInfos[i];
            stResourceInfo info2 = this.m_resourceInfos[i];
            if (string.Equals(info.m_fullPathInResourcesWithoutExtension + info2.m_extension, fullPath))
            {
                return true;
            }
        }
        return false;
    }

    [DebuggerHidden]
    public IEnumerator ASyncLoadAssetBundle(string ifsExtractPath)
    {
        return new <ASyncLoadAssetBundle>c__Iterator11 { ifsExtractPath = ifsExtractPath, <$>ifsExtractPath = ifsExtractPath, <>f__this = this };
    }

    public string GetRename(string pathInResourcesFolderWithoutExt)
    {
        if (this.m_renameMap != null)
        {
            string str = string.Empty;
            if (this.m_renameMap.TryGetValue(pathInResourcesFolderWithoutExt.ToLower(), out str))
            {
                return str;
            }
        }
        return string.Empty;
    }

    public bool HasFlag(enBundleFlag flag)
    {
        int num = (int) flag;
        return ((this.m_flags & num) > 0);
    }

    public bool IsAllowDuplicateNames()
    {
        return this.HasFlag(enBundleFlag.AllowDuplicateNames);
    }

    public bool IsAlreadyContainResourceFileName(string fullPath, out string log)
    {
        log = string.Empty;
        string fullName = CFileManager.GetFullName(fullPath);
        for (int i = 0; i < this.m_resourceInfos.Count; i++)
        {
            stResourceInfo info = this.m_resourceInfos[i];
            stResourceInfo info2 = this.m_resourceInfos[i];
            if (string.Equals(CFileManager.GetFullName(info.m_fullPathInResourcesWithoutExtension) + info2.m_extension, fullName))
            {
                string[] textArray1 = new string[8];
                textArray1[0] = "Duplicated File \"";
                textArray1[1] = fullPath;
                textArray1[2] = "\" and \"";
                stResourceInfo info3 = this.m_resourceInfos[i];
                textArray1[3] = info3.m_fullPathInResourcesWithoutExtension;
                stResourceInfo info4 = this.m_resourceInfos[i];
                textArray1[4] = info4.m_extension;
                textArray1[5] = "\" in resource bundle:\"";
                textArray1[6] = this.m_pathInIFS;
                textArray1[7] = "\"";
                log = string.Concat(textArray1);
                return true;
            }
        }
        return false;
    }

    public bool IsAssetBundleInLoading()
    {
        return (this.m_isAssetBundle && (this.m_assetBundleState == enAssetBundleState.Loading));
    }

    public bool IsAssetBundleLoaded()
    {
        return (this.m_isAssetBundle && (this.m_assetBundleState == enAssetBundleState.Loaded));
    }

    public bool IsCompleteAssets()
    {
        return !this.HasFlag(enBundleFlag.UnCompleteAsset);
    }

    public bool IsKeepInResources()
    {
        return this.HasFlag(enBundleFlag.KeepInResources);
    }

    public bool IsReplaceDuplicateNames()
    {
        return this.HasFlag(enBundleFlag.ReplaceDuplicateNames);
    }

    public bool IsResident()
    {
        return ((this.dependency == null) && this.HasFlag(enBundleFlag.Resident));
    }

    public bool IsUnCompress()
    {
        return this.HasFlag(enBundleFlag.UnCompress);
    }

    public void LoadAssetBundle(string ifsExtractPath)
    {
        if (!this.m_isAssetBundle)
        {
            return;
        }
        if (((this.dependency != null) && this.dependency.m_isAssetBundle) && !this.dependency.IsAssetBundleLoaded())
        {
            this.dependency.LoadAssetBundle(ifsExtractPath);
        }
        if (this.m_assetBundleState != enAssetBundleState.Unload)
        {
            return;
        }
        this.m_useASyncLoadingData = false;
        string filePath = CFileManager.CombinePath(ifsExtractPath, this.m_pathInIFS);
        if (!CFileManager.IsFileExist(filePath))
        {
            Debug.Log("File " + filePath + " can not be found!!!");
            goto Label_019A;
        }
        if (this.IsUnCompress())
        {
            int num = 0;
            do
            {
                try
                {
                    this.m_assetBundle = AssetBundle.CreateFromFile(filePath);
                }
                catch (Exception)
                {
                    this.m_assetBundle = null;
                }
                if (this.m_assetBundle != null)
                {
                    goto Label_0112;
                }
                Debug.Log(string.Concat(new object[] { "Create AssetBundle ", filePath, " From File Error! Try Count = ", num }));
                num++;
            }
            while (num < 3);
            CFileManager.s_delegateOnOperateFileFail(filePath, enFileOperation.ReadFile);
        }
        else
        {
            this.m_assetBundle = AssetBundle.CreateFromMemoryImmediate(CFileManager.ReadFile(filePath));
        }
    Label_0112:
        if (this.m_assetBundle == null)
        {
            string str2 = string.Empty;
            try
            {
                str2 = CFileManager.GetFileMd5(filePath);
            }
            catch (Exception)
            {
                str2 = string.Empty;
            }
            object[] args = new object[] { filePath, CVersion.GetAppVersion(), CVersion.GetBuildNumber(), CVersion.GetRevisonNumber(), CVersion.GetUsedResourceVersion(), str2 };
            Debug.Log(string.Format("Load AssetBundle {0} Error!!! App version = {1}, Build = {2}, Reversion = {3}, Resource version = {4}, File md5 = {5}", args));
        }
    Label_019A:
        this.m_assetBundleState = enAssetBundleState.Loaded;
    }

    public virtual void Read(byte[] data, ref int offset)
    {
        this.m_isAssetBundle = CMemoryManager.ReadByte(data, ref offset) > 0;
        this.m_pathInIFS = CMemoryManager.ReadString(data, ref offset);
        this.m_tag = CMemoryManager.ReadString(data, ref offset);
        this.m_flags = CMemoryManager.ReadInt(data, ref offset);
        int num = CMemoryManager.ReadShort(data, ref offset);
        this.m_resourceInfos.Clear();
        for (int i = 0; i < num; i++)
        {
            stResourceInfo item = new stResourceInfo {
                m_fullPathInResourcesWithoutExtension = CMemoryManager.ReadString(data, ref offset),
                m_extension = CMemoryManager.ReadString(data, ref offset),
                m_flags = CMemoryManager.ReadInt(data, ref offset),
                m_isRenamed = (CMemoryManager.ReadByte(data, ref offset) <= 0) ? false : true
            };
            if (item.m_isRenamed)
            {
                item.m_fullPathInResourcesWithoutExtension_Renamed = CMemoryManager.ReadString(data, ref offset);
            }
            string str = item.m_extension.Replace(".", string.Empty);
            this.m_fileExtMap[item.m_fullPathInResourcesWithoutExtension.ToLower()] = str;
            this.m_resourceInfos.Add(item);
        }
        num = CMemoryManager.ReadShort(data, ref offset);
        this.m_children.Clear();
        for (int j = 0; j < num; j++)
        {
            CResourcePackerInfo info2 = new CResourcePackerInfo(true);
            info2.Read(data, ref offset);
            info2.dependency = this;
        }
    }

    public void UnloadAssetBundle(bool force = false)
    {
        if (this.m_isAssetBundle && (!this.IsResident() || force))
        {
            if (this.m_assetBundleState == enAssetBundleState.Loaded)
            {
                if (this.m_assetBundle != null)
                {
                    this.m_assetBundle.Unload(false);
                    this.m_assetBundle = null;
                }
                this.m_assetBundleState = enAssetBundleState.Unload;
            }
            else if (this.m_assetBundleState == enAssetBundleState.Loading)
            {
                this.m_useASyncLoadingData = false;
            }
            if (this.dependency != null)
            {
                this.dependency.UnloadAssetBundle(force);
            }
        }
    }

    public virtual void Write(byte[] data, ref int offset)
    {
        CMemoryManager.WriteByte(!this.m_isAssetBundle ? ((byte) 0) : ((byte) 1), data, ref offset);
        CMemoryManager.WriteString(this.m_pathInIFS, data, ref offset);
        CMemoryManager.WriteString(this.m_tag, data, ref offset);
        CMemoryManager.WriteInt(this.m_flags, data, ref offset);
        CMemoryManager.WriteShort((short) this.m_resourceInfos.Count, data, ref offset);
        for (int i = 0; i < this.m_resourceInfos.Count; i++)
        {
            stResourceInfo info = this.m_resourceInfos[i];
            CMemoryManager.WriteString(info.m_fullPathInResourcesWithoutExtension, data, ref offset);
            stResourceInfo info2 = this.m_resourceInfos[i];
            CMemoryManager.WriteString(info2.m_extension, data, ref offset);
            stResourceInfo info3 = this.m_resourceInfos[i];
            CMemoryManager.WriteInt(info3.m_flags, data, ref offset);
            stResourceInfo info4 = this.m_resourceInfos[i];
            CMemoryManager.WriteByte(!info4.m_isRenamed ? ((byte) 0) : ((byte) 1), data, ref offset);
            stResourceInfo info5 = this.m_resourceInfos[i];
            if (info5.m_isRenamed)
            {
                stResourceInfo info6 = this.m_resourceInfos[i];
                CMemoryManager.WriteString(info6.m_fullPathInResourcesWithoutExtension_Renamed, data, ref offset);
            }
        }
        CMemoryManager.WriteShort((short) this.m_children.Count, data, ref offset);
        for (int j = 0; j < this.m_children.Count; j++)
        {
            this.m_children[j].Write(data, ref offset);
        }
    }

    public CResourcePackerInfo dependency
    {
        get
        {
            return this.m_parent;
        }
        set
        {
            this.m_parent = value;
            value.m_children.Add(this);
        }
    }

    [CompilerGenerated]
    private sealed class <ASyncLoadAssetBundle>c__Iterator11 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal string <$>ifsExtractPath;
        internal CResourcePackerInfo <>f__this;
        internal AssetBundleCreateRequest <assetBundleLoader>__0;
        internal string ifsExtractPath;

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
                    if (!this.<>f__this.m_isAssetBundle)
                    {
                        break;
                    }
                    if (this.<>f__this.dependency != null)
                    {
                    }
                    this.<>f__this.m_useASyncLoadingData = true;
                    this.<>f__this.m_assetBundleState = enAssetBundleState.Loading;
                    this.<assetBundleLoader>__0 = AssetBundle.CreateFromMemory(CFileManager.ReadFile(CFileManager.CombinePath(this.ifsExtractPath, this.<>f__this.m_pathInIFS)));
                    this.$current = this.<assetBundleLoader>__0;
                    this.$PC = 1;
                    return true;

                case 1:
                    if (this.<>f__this.m_useASyncLoadingData)
                    {
                        this.<>f__this.m_assetBundle = this.<assetBundleLoader>__0.assetBundle;
                    }
                    this.<>f__this.m_assetBundleState = enAssetBundleState.Loaded;
                    break;

                default:
                    goto Label_00D0;
            }
            this.$PC = -1;
        Label_00D0:
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

