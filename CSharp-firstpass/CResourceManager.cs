using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class CResourceManager : Singleton<CResourceManager>
{
    public static bool isBattleState;
    private DictionaryView<string, CResource> m_cachedResourceMap;
    private bool m_clearUnusedAssets;
    private int m_clearUnusedAssetsExecuteFrame;
    private CResourcePackerInfoSet m_resourcePackerInfoSet;
    private static int s_frameCounter;

    public bool CheckCachedResource(string fullPathInResources)
    {
        string key = CFileManager.EraseExtension(fullPathInResources).ToLower();
        CResource resource = null;
        return this.m_cachedResourceMap.TryGetValue(key, out resource);
    }

    public void CustomUpdate()
    {
        s_frameCounter++;
        if (this.m_clearUnusedAssets && (this.m_clearUnusedAssetsExecuteFrame == s_frameCounter))
        {
            this.ExecuteUnloadUnusedAssets();
            this.m_clearUnusedAssets = false;
        }
    }

    private void ExecuteUnloadUnusedAssets()
    {
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    public string GetAssetBundleInfoString()
    {
        if (this.m_resourcePackerInfoSet == null)
        {
            return string.Empty;
        }
        string str = string.Empty;
        int num = 0;
        for (int i = 0; i < this.m_resourcePackerInfoSet.m_resourcePackerInfosAll.Count; i++)
        {
            if (this.m_resourcePackerInfoSet.m_resourcePackerInfosAll[i].IsAssetBundleLoaded())
            {
                num++;
                str = str + CFileManager.GetFullName(this.m_resourcePackerInfoSet.m_resourcePackerInfosAll[i].m_pathInIFS);
            }
        }
        return str;
    }

    public DictionaryView<string, CResource> GetCachedResourceMap()
    {
        return this.m_cachedResourceMap;
    }

    public CResource GetResource(string fullPathInResources, System.Type resourceContentType, enResourceType resourceType, bool needCached = false, bool unloadBelongedAssetBundleAfterLoaded = false)
    {
        if (string.IsNullOrEmpty(fullPathInResources))
        {
            return new CResource(string.Empty, string.Empty, null, resourceType, unloadBelongedAssetBundleAfterLoaded);
        }
        string key = CFileManager.EraseExtension(fullPathInResources).ToLower();
        CResource resource = null;
        if (this.m_cachedResourceMap.TryGetValue(key, out resource))
        {
            if (resource.m_resourceType != resourceType)
            {
                resource.m_resourceType = resourceType;
            }
            return resource;
        }
        resource = new CResource(key, fullPathInResources, resourceContentType, resourceType, unloadBelongedAssetBundleAfterLoaded);
        this.LoadResource(resource);
        if (needCached)
        {
            this.m_cachedResourceMap.Add(key, resource);
        }
        return resource;
    }

    private CResourcePackerInfo GetResourceBelongedPackerInfo(CResource resource)
    {
        if (this.m_resourcePackerInfoSet == null)
        {
            return null;
        }
        CResourcePackerInfo resourceBelongedPackerInfo = this.m_resourcePackerInfoSet.GetResourceBelongedPackerInfo(resource.m_key);
        if (resourceBelongedPackerInfo != null)
        {
            string str = string.Empty;
            if (!resourceBelongedPackerInfo.m_fileExtMap.TryGetValue(resource.m_fullPathInResourcesWithoutExtension.ToLower(), out str))
            {
                Debug.LogError("No Resource " + resource.m_fullPathInResourcesWithoutExtension + " found in ext name map of bundle:" + resourceBelongedPackerInfo.m_pathInIFS);
            }
            resource.m_fileFullPathInResources = resource.m_fullPathInResourcesWithoutExtension + "." + str;
        }
        return resourceBelongedPackerInfo;
    }

    public CResourcePackerInfo GetResourceBelongedPackerInfo(string fullPathInResources)
    {
        if (!string.IsNullOrEmpty(fullPathInResources) && (this.m_resourcePackerInfoSet != null))
        {
            return this.m_resourcePackerInfoSet.GetResourceBelongedPackerInfo(CFileManager.EraseExtension(fullPathInResources).ToLower());
        }
        return null;
    }

    public System.Type GetResourceContentType(string extension)
    {
        System.Type type = null;
        if (string.Equals(extension, ".prefab", StringComparison.OrdinalIgnoreCase))
        {
            return typeof(GameObject);
        }
        if (string.Equals(extension, ".bytes", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".xml", StringComparison.OrdinalIgnoreCase))
        {
            return typeof(TextAsset);
        }
        if (string.Equals(extension, ".asset", StringComparison.OrdinalIgnoreCase))
        {
            type = typeof(ScriptableObject);
        }
        return type;
    }

    private CResourcePackerInfo GetResourcePackerInfo(string fullPathInIFS)
    {
        if (!string.IsNullOrEmpty(fullPathInIFS) && (this.m_resourcePackerInfoSet != null))
        {
            for (int i = 0; i < this.m_resourcePackerInfoSet.m_resourcePackerInfosAll.Count; i++)
            {
                if (string.Equals(fullPathInIFS, this.m_resourcePackerInfoSet.m_resourcePackerInfosAll[i].m_pathInIFS))
                {
                    return this.m_resourcePackerInfoSet.m_resourcePackerInfosAll[i];
                }
            }
        }
        return null;
    }

    public override void Init()
    {
        this.m_resourcePackerInfoSet = null;
        this.m_cachedResourceMap = new DictionaryView<string, CResource>();
    }

    public void LoadAllResourceInResourcePackerInfo(CResourcePackerInfo resourcePackerInfo, enResourceType resourceType)
    {
        if (resourcePackerInfo != null)
        {
            for (int i = 0; i < resourcePackerInfo.m_resourceInfos.Count; i++)
            {
                stResourceInfo info = resourcePackerInfo.m_resourceInfos[i];
                stResourceInfo info2 = resourcePackerInfo.m_resourceInfos[i];
                this.GetResource(info.m_fullPathInResourcesWithoutExtension, this.GetResourceContentType(info2.m_extension), resourceType, true, i == (resourcePackerInfo.m_resourceInfos.Count - 1));
            }
        }
    }

    public void LoadAllResourceInResourcePackerInfo(string fullPathInIFS, enResourceType resourceType)
    {
        this.LoadAllResourceInResourcePackerInfo(this.GetResourcePackerInfo(fullPathInIFS), resourceType);
    }

    public void LoadAssetBundle(CResourcePackerInfo resourcePackerInfo)
    {
        if (((resourcePackerInfo != null) && resourcePackerInfo.m_isAssetBundle) && !resourcePackerInfo.IsAssetBundleLoaded())
        {
            resourcePackerInfo.LoadAssetBundle(CFileManager.GetIFSExtractPath());
        }
    }

    public void LoadAssetBundle(string fullPathInIFS)
    {
        this.LoadAssetBundle(this.GetResourcePackerInfo(fullPathInIFS));
    }

    [DebuggerHidden]
    public IEnumerator LoadResidentAssetBundles()
    {
        return new <LoadResidentAssetBundles>c__Iterator10 { <>f__this = this };
    }

    private void LoadResource(CResource resource)
    {
        CResourcePackerInfo resourceBelongedPackerInfo = this.GetResourceBelongedPackerInfo(resource);
        if (resourceBelongedPackerInfo != null)
        {
            if (resourceBelongedPackerInfo.m_isAssetBundle)
            {
                if (!resourceBelongedPackerInfo.IsAssetBundleLoaded())
                {
                    resourceBelongedPackerInfo.LoadAssetBundle(CFileManager.GetIFSExtractPath());
                }
                resource.LoadFromAssetBundle(resourceBelongedPackerInfo);
                if (resource.m_unloadBelongedAssetBundleAfterLoaded)
                {
                    resourceBelongedPackerInfo.UnloadAssetBundle(false);
                }
            }
            else
            {
                resource.Load(CFileManager.GetIFSExtractPath());
            }
        }
        else
        {
            resource.Load();
        }
    }

    public void LoadResourcePackerInfoSet()
    {
        if (this.m_resourcePackerInfoSet != null)
        {
            this.m_resourcePackerInfoSet.Dispose();
            this.m_resourcePackerInfoSet = null;
        }
        string filePath = CFileManager.CombinePath(CFileManager.GetIFSExtractPath(), CResourcePackerInfoSet.s_resourcePackerInfoSetFileName);
        if (CFileManager.IsFileExist(filePath))
        {
            byte[] data = CFileManager.ReadFile(filePath);
            int offset = 0;
            this.m_resourcePackerInfoSet = new CResourcePackerInfoSet();
            this.m_resourcePackerInfoSet.Read(data, ref offset);
            CVersion.SetUsedResourceVersion(this.m_resourcePackerInfoSet.m_version);
            this.m_resourcePackerInfoSet.CreateResourceMap();
        }
    }

    public void RemoveAllCachedResources()
    {
        this.RemoveCachedResources((enResourceType[]) Enum.GetValues(typeof(enResourceType)));
    }

    public void RemoveCachedResource(string fullPathInResources)
    {
        string key = CFileManager.EraseExtension(fullPathInResources).ToLower();
        CResource resource = null;
        if (this.m_cachedResourceMap.TryGetValue(key, out resource))
        {
            resource.Unload();
            this.m_cachedResourceMap.Remove(key);
        }
    }

    public void RemoveCachedResources(enResourceType[] resourceTypes)
    {
        for (int i = 0; i < resourceTypes.Length; i++)
        {
            this.RemoveCachedResources(resourceTypes[i], false);
        }
        this.UnloadUnusedAssetBundles();
        this.UnloadUnusedAssets();
    }

    public void RemoveCachedResources(enResourceType resourceType, bool clearImmediately = true)
    {
        List<string> list = new List<string>();
        DictionaryView<string, CResource>.Enumerator enumerator = this.m_cachedResourceMap.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<string, CResource> current = enumerator.Current;
            CResource resource = current.Value;
            if (resource.m_resourceType == resourceType)
            {
                resource.Unload();
                list.Add(resource.m_key);
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            this.m_cachedResourceMap.Remove(list[i]);
        }
        if (clearImmediately)
        {
            this.UnloadUnusedAssetBundles();
            this.UnloadUnusedAssets();
        }
    }

    public void UnloadAssetBundlesByTag(string tag)
    {
        if (this.m_resourcePackerInfoSet != null)
        {
            for (int i = 0; i < this.m_resourcePackerInfoSet.m_resourcePackerInfosAll.Count; i++)
            {
                if (this.m_resourcePackerInfoSet.m_resourcePackerInfosAll[i].m_tag.Equals(tag))
                {
                    this.m_resourcePackerInfoSet.m_resourcePackerInfosAll[i].UnloadAssetBundle(false);
                }
            }
        }
    }

    public void UnloadBelongedAssetbundle(string fullPathInResources)
    {
        CResourcePackerInfo resourceBelongedPackerInfo = this.GetResourceBelongedPackerInfo(fullPathInResources);
        if ((resourceBelongedPackerInfo != null) && resourceBelongedPackerInfo.IsAssetBundleLoaded())
        {
            resourceBelongedPackerInfo.UnloadAssetBundle(false);
        }
    }

    private void UnloadUnusedAssetBundles()
    {
        if (this.m_resourcePackerInfoSet != null)
        {
            for (int i = 0; i < this.m_resourcePackerInfoSet.m_resourcePackerInfosAll.Count; i++)
            {
                CResourcePackerInfo info = this.m_resourcePackerInfoSet.m_resourcePackerInfosAll[i];
                if (info.IsAssetBundleLoaded())
                {
                    bool flag = true;
                    for (int j = 0; j < info.m_resourceInfos.Count; j++)
                    {
                        stResourceInfo info2 = info.m_resourceInfos[j];
                        if (this.CheckCachedResource(info2.m_fullPathInResourcesWithoutExtension))
                        {
                            flag = false;
                        }
                    }
                    if (flag)
                    {
                        info.UnloadAssetBundle(false);
                    }
                }
            }
        }
    }

    public void UnloadUnusedAssets()
    {
        this.m_clearUnusedAssets = true;
        this.m_clearUnusedAssetsExecuteFrame = s_frameCounter + 1;
    }

    [CompilerGenerated]
    private sealed class <LoadResidentAssetBundles>c__Iterator10 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal CResourceManager <>f__this;
        internal int <i>__0;
        internal CResourcePackerInfo <resourcePackerInfo>__1;

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
                    if (this.<>f__this.m_resourcePackerInfoSet != null)
                    {
                        this.<i>__0 = 0;
                        while (this.<i>__0 < this.<>f__this.m_resourcePackerInfoSet.m_resourcePackerInfosAll.Count)
                        {
                            this.<resourcePackerInfo>__1 = this.<>f__this.m_resourcePackerInfoSet.m_resourcePackerInfosAll[this.<i>__0];
                            if ((this.<resourcePackerInfo>__1.m_isAssetBundle && this.<resourcePackerInfo>__1.IsResident()) && !this.<resourcePackerInfo>__1.IsAssetBundleLoaded())
                            {
                                this.<resourcePackerInfo>__1.LoadAssetBundle(CFileManager.GetIFSExtractPath());
                                this.$current = null;
                                this.$PC = 1;
                                return true;
                            }
                        Label_00B6:
                            this.<i>__0++;
                        }
                        this.$PC = -1;
                        break;
                    }
                    break;

                case 1:
                    goto Label_00B6;
            }
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

    public delegate void OnResourceLoaded(CResource resource);
}

