using System;
using System.IO;

public class CResourcePackerInfoSet
{
    public string m_buildNumber;
    public string m_ifsPath;
    public string m_publish;
    private DictionaryView<string, ListView<CResourcePackerInfo>> m_resourceMap = new DictionaryView<string, ListView<CResourcePackerInfo>>();
    public ListView<CResourcePackerInfo> m_resourcePackerInfosAll = new ListView<CResourcePackerInfo>();
    private ListView<CResourcePackerInfo> m_resourcePackerInfosInTreeStruct = new ListView<CResourcePackerInfo>();
    public string m_version;
    public static string s_resourceIFSFileName = "sgame_resource.ifs";
    public static string s_resourcePackerInfoSetFileName = "ResourcePackerInfoSet.bytes";

    private void _AddResourcePackerInfoAll(CResourcePackerInfo resourceInfo)
    {
        this.m_resourcePackerInfosAll.Add(resourceInfo);
        for (int i = 0; i < resourceInfo.m_children.Count; i++)
        {
            this._AddResourcePackerInfoAll(resourceInfo.m_children[i]);
        }
    }

    public void AddResourcePackerInfo(CResourcePackerInfo resourceInfo)
    {
        this.m_resourcePackerInfosInTreeStruct.Add(resourceInfo);
        this.m_resourcePackerInfosAll.Add(resourceInfo);
        for (int i = 0; i < resourceInfo.m_children.Count; i++)
        {
            this._AddResourcePackerInfoAll(resourceInfo.m_children[i]);
        }
    }

    public void CreateResourceMap()
    {
        for (int i = 0; i < this.m_resourcePackerInfosAll.Count; i++)
        {
            this.m_resourcePackerInfosAll[i].AddToResourceMap(this.m_resourceMap);
        }
    }

    public void Dispose()
    {
        for (int i = 0; i < this.m_resourcePackerInfosAll.Count; i++)
        {
            if (this.m_resourcePackerInfosAll[i].IsAssetBundleLoaded())
            {
                this.m_resourcePackerInfosAll[i].UnloadAssetBundle(false);
            }
            this.m_resourcePackerInfosAll[i] = null;
        }
        this.m_resourcePackerInfosInTreeStruct.Clear();
        this.m_resourcePackerInfosAll.Clear();
        this.m_resourceMap.Clear();
    }

    public CResourcePackerInfo GetResourceBelongedPackerInfo(string resourceKey)
    {
        ListView<CResourcePackerInfo> view = null;
        if ((!this.m_resourceMap.TryGetValue(resourceKey, out view) || (view == null)) || (view.Count <= 0))
        {
            return null;
        }
        for (int i = 0; i < view.Count; i++)
        {
            if (view[i].IsAssetBundleLoaded())
            {
                return view[i];
            }
        }
        return view[0];
    }

    public void Read(byte[] data, ref int offset)
    {
        if (((data != null) && (offset >= 0)) && (offset < data.Length))
        {
            int num = offset;
            if (CMemoryManager.ReadInt(data, ref offset) <= (data.Length - num))
            {
                this.m_version = CMemoryManager.ReadString(data, ref offset);
                this.m_buildNumber = CMemoryManager.ReadString(data, ref offset);
                this.m_publish = CMemoryManager.ReadString(data, ref offset);
                this.m_ifsPath = CMemoryManager.ReadString(data, ref offset);
                int num2 = CMemoryManager.ReadShort(data, ref offset);
                this.m_resourcePackerInfosInTreeStruct.Clear();
                this.m_resourcePackerInfosAll.Clear();
                for (int i = 0; i < num2; i++)
                {
                    CResourcePackerInfo resourceInfo = new CResourcePackerInfo(false);
                    resourceInfo.Read(data, ref offset);
                    this.AddResourcePackerInfo(resourceInfo);
                }
            }
        }
    }

    public static void ReadVersionAndBuildNumber(byte[] data, ref int offset, ref string version, ref string buildNumber)
    {
        version = CVersion.s_emptyResourceVersion;
        buildNumber = CVersion.s_emptyBuildNumber;
        if (((data != null) && (offset >= 0)) && (offset < data.Length))
        {
            int num = offset;
            if (CMemoryManager.ReadInt(data, ref offset) <= (data.Length - num))
            {
                version = CMemoryManager.ReadString(data, ref offset);
                buildNumber = CMemoryManager.ReadString(data, ref offset);
            }
        }
    }

    public void Write(StreamWriter streamWriter)
    {
        streamWriter.WriteLine("=========================================================================================================");
        streamWriter.WriteLine("CResourcePackerInfoSet : Version = " + this.m_version);
        streamWriter.WriteLine("=========================================================================================================");
        for (int i = 0; i < this.m_resourcePackerInfosInTreeStruct.Count; i++)
        {
            CResourcePackerInfo info = this.m_resourcePackerInfosInTreeStruct[i];
            streamWriter.WriteLine(string.Concat(new object[] { "    CResourcePackerInfo : Path = ", info.m_pathInIFS, ", IsAssetBundle = ", info.m_isAssetBundle }));
            for (int j = 0; j < info.m_resourceInfos.Count; j++)
            {
                stResourceInfo info2 = info.m_resourceInfos[j];
                streamWriter.WriteLine(string.Concat(new object[] { "        Resource : Path = ", info2.m_fullPathInResourcesWithoutExtension, ", Extension = ", info2.m_extension, ", Flags = ", info2.m_flags }));
            }
            if (info.m_children != null)
            {
                for (int k = 0; k < info.m_children.Count; k++)
                {
                    CResourcePackerInfo info3 = info.m_children[k];
                    streamWriter.WriteLine(string.Concat(new object[] { "        Child CResourcePackerInfo : Path = ", info3.m_pathInIFS, ", IsAssetBundle = ", info3.m_isAssetBundle }));
                    for (int m = 0; m < info3.m_resourceInfos.Count; m++)
                    {
                        stResourceInfo info4 = info3.m_resourceInfos[m];
                        streamWriter.WriteLine(string.Concat(new object[] { "            Resource : Path = ", info4.m_fullPathInResourcesWithoutExtension, ", Extension = ", info4.m_extension, ", Flags = ", info4.m_flags }));
                    }
                }
            }
            streamWriter.WriteLine(" ");
        }
        streamWriter.WriteLine("=========================================================================================================");
    }

    public void Write(byte[] data, ref int offset)
    {
        if (((data != null) && (offset >= 0)) && (offset < data.Length))
        {
            int num = offset;
            offset += 4;
            CMemoryManager.WriteString(this.m_version, data, ref offset);
            CMemoryManager.WriteString(this.m_buildNumber, data, ref offset);
            CMemoryManager.WriteString(this.m_publish, data, ref offset);
            CMemoryManager.WriteString(this.m_ifsPath, data, ref offset);
            CMemoryManager.WriteShort((short) this.m_resourcePackerInfosInTreeStruct.Count, data, ref offset);
            for (int i = 0; i < this.m_resourcePackerInfosInTreeStruct.Count; i++)
            {
                this.m_resourcePackerInfosInTreeStruct[i].Write(data, ref offset);
            }
            CMemoryManager.WriteInt(offset, data, ref num);
        }
    }
}

