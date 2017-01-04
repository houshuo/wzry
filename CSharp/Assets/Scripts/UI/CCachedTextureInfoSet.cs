namespace Assets.Scripts.UI
{
    using System;

    public class CCachedTextureInfoSet
    {
        public const int c_version = 0x2713;
        public DictionaryView<string, CCachedTextureInfo> m_cachedTextureInfoMap = new DictionaryView<string, CCachedTextureInfo>();
        public ListView<CCachedTextureInfo> m_cachedTextureInfos = new ListView<CCachedTextureInfo>();

        public void AddTextureInfo(string key, CCachedTextureInfo cachedTextureInfo)
        {
            if (!this.m_cachedTextureInfoMap.ContainsKey(key))
            {
                this.m_cachedTextureInfoMap.Add(key, cachedTextureInfo);
                this.m_cachedTextureInfos.Add(cachedTextureInfo);
            }
        }

        public CCachedTextureInfo GetCachedTextureInfo(string key)
        {
            if (this.m_cachedTextureInfoMap.ContainsKey(key))
            {
                CCachedTextureInfo info = null;
                this.m_cachedTextureInfoMap.TryGetValue(key, out info);
                return info;
            }
            return null;
        }

        public void Read(byte[] data, ref int offset)
        {
            this.m_cachedTextureInfos.Clear();
            this.m_cachedTextureInfoMap.Clear();
            if (data != null)
            {
                int num = data.Length - offset;
                if (num >= 6)
                {
                    int num2 = CMemoryManager.ReadInt(data, ref offset);
                    if (((num2 >= 6) && (num2 <= num)) && (CMemoryManager.ReadShort(data, ref offset) == 0x2713))
                    {
                        int num4 = CMemoryManager.ReadShort(data, ref offset);
                        for (int i = 0; i < num4; i++)
                        {
                            CCachedTextureInfo info = new CCachedTextureInfo {
                                m_key = CMemoryManager.ReadString(data, ref offset),
                                m_width = CMemoryManager.ReadShort(data, ref offset),
                                m_height = CMemoryManager.ReadShort(data, ref offset),
                                m_lastModifyTime = CMemoryManager.ReadDateTime(data, ref offset),
                                m_isGif = CMemoryManager.ReadByte(data, ref offset) > 0
                            };
                            if (!this.m_cachedTextureInfoMap.ContainsKey(info.m_key))
                            {
                                this.m_cachedTextureInfoMap.Add(info.m_key, info);
                                this.m_cachedTextureInfos.Add(info);
                            }
                        }
                        this.m_cachedTextureInfos.Sort();
                    }
                }
            }
        }

        public string RemoveEarliestTextureInfo()
        {
            if (this.m_cachedTextureInfos.Count <= 0)
            {
                return null;
            }
            CCachedTextureInfo info = this.m_cachedTextureInfos[0];
            this.m_cachedTextureInfos.RemoveAt(0);
            this.m_cachedTextureInfoMap.Remove(info.m_key);
            return info.m_key;
        }

        public void SortTextureInfo()
        {
            this.m_cachedTextureInfos.Sort();
        }

        public void Write(byte[] data, ref int offset)
        {
            int num = offset;
            offset += 4;
            CMemoryManager.WriteShort(0x2713, data, ref offset);
            CMemoryManager.WriteShort((short) this.m_cachedTextureInfos.Count, data, ref offset);
            for (int i = 0; i < this.m_cachedTextureInfos.Count; i++)
            {
                CMemoryManager.WriteString(this.m_cachedTextureInfos[i].m_key, data, ref offset);
                CMemoryManager.WriteShort((short) this.m_cachedTextureInfos[i].m_width, data, ref offset);
                CMemoryManager.WriteShort((short) this.m_cachedTextureInfos[i].m_height, data, ref offset);
                CMemoryManager.WriteDateTime(ref this.m_cachedTextureInfos[i].m_lastModifyTime, data, ref offset);
                CMemoryManager.WriteByte(!this.m_cachedTextureInfos[i].m_isGif ? ((byte) 0) : ((byte) 1), data, ref offset);
            }
            CMemoryManager.WriteInt(offset - num, data, ref num);
        }
    }
}

