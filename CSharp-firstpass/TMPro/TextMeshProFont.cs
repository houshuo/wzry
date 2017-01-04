namespace TMPro
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class TextMeshProFont : ScriptableObject
    {
        [SerializeField]
        public Texture2D atlas;
        public float BoldStyle = 0.75f;
        [SerializeField]
        public FontCreationSetting fontCreationSettings;
        public int fontHashCode;
        public byte ItalicStyle = 0x23;
        private Dictionary<int, GlyphInfo> m_characterDictionary;
        private int[] m_characterSet;
        [SerializeField]
        private FaceInfo m_fontInfo;
        [SerializeField]
        private List<GlyphInfo> m_glyphInfoList;
        private Dictionary<int, KerningPair> m_kerningDictionary;
        [SerializeField]
        private KerningTable m_kerningInfo;
        [SerializeField]
        private KerningPair m_kerningPair;
        [SerializeField]
        private LineBreakingTable m_lineBreakingInfo;
        [SerializeField]
        public Material material;
        public int materialHashCode;
        public float NormalStyle;
        [SerializeField]
        public bool propertiesChanged;
        public byte TabSize = 10;

        public void AddFaceInfo(FaceInfo faceInfo)
        {
            this.m_fontInfo = faceInfo;
        }

        public void AddGlyphInfo(GlyphInfo[] glyphInfo)
        {
            this.m_glyphInfoList = new List<GlyphInfo>();
            this.m_characterSet = new int[this.m_fontInfo.CharacterCount];
            for (int i = 0; i < this.m_fontInfo.CharacterCount; i++)
            {
                GlyphInfo item = new GlyphInfo {
                    id = glyphInfo[i].id,
                    x = glyphInfo[i].x,
                    y = glyphInfo[i].y,
                    width = glyphInfo[i].width,
                    height = glyphInfo[i].height,
                    xOffset = glyphInfo[i].xOffset,
                    yOffset = glyphInfo[i].yOffset + this.m_fontInfo.Padding,
                    xAdvance = glyphInfo[i].xAdvance
                };
                this.m_glyphInfoList.Add(item);
                this.m_characterSet[i] = item.id;
            }
        }

        public void AddKerningInfo(KerningTable kerningTable)
        {
            this.m_kerningInfo = kerningTable;
        }

        private Dictionary<int, char> GetCharacters(TextAsset file)
        {
            Dictionary<int, char> dictionary = new Dictionary<int, char>();
            foreach (char ch in file.text)
            {
                if (!dictionary.ContainsKey(ch))
                {
                    dictionary.Add(ch, ch);
                }
            }
            return dictionary;
        }

        private void OnDisable()
        {
        }

        private void OnEnable()
        {
            if (this.m_characterDictionary == null)
            {
                this.ReadFontDefinition();
            }
        }

        public void ReadFontDefinition()
        {
            if (this.m_fontInfo != null)
            {
                this.m_characterDictionary = new Dictionary<int, GlyphInfo>();
                foreach (GlyphInfo info in this.m_glyphInfoList)
                {
                    if (!this.m_characterDictionary.ContainsKey(info.id))
                    {
                        this.m_characterDictionary.Add(info.id, info);
                    }
                }
                GlyphInfo info2 = new GlyphInfo();
                if (this.m_characterDictionary.ContainsKey(0x20))
                {
                    this.m_characterDictionary[0x20].width = this.m_fontInfo.Ascender / 5f;
                    this.m_characterDictionary[0x20].height = this.m_fontInfo.Ascender - this.m_fontInfo.Descender;
                    this.m_characterDictionary[0x20].yOffset = this.m_fontInfo.Ascender;
                }
                else
                {
                    info2 = new GlyphInfo {
                        id = 0x20,
                        x = 0f,
                        y = 0f,
                        width = this.m_fontInfo.Ascender / 5f,
                        height = this.m_fontInfo.Ascender - this.m_fontInfo.Descender,
                        xOffset = 0f,
                        yOffset = this.m_fontInfo.Ascender,
                        xAdvance = this.m_fontInfo.PointSize / 4f
                    };
                    this.m_characterDictionary.Add(0x20, info2);
                }
                if (!this.m_characterDictionary.ContainsKey(10))
                {
                    info2 = new GlyphInfo {
                        id = 10,
                        x = 0f,
                        y = 0f,
                        width = 0f,
                        height = 0f,
                        xOffset = 0f,
                        yOffset = 0f,
                        xAdvance = 0f
                    };
                    this.m_characterDictionary.Add(10, info2);
                    this.m_characterDictionary.Add(13, info2);
                }
                if (!this.m_characterDictionary.ContainsKey(9))
                {
                    info2 = new GlyphInfo {
                        id = 9,
                        x = this.m_characterDictionary[0x20].x,
                        y = this.m_characterDictionary[0x20].y,
                        width = this.m_characterDictionary[0x20].width * this.TabSize,
                        height = this.m_characterDictionary[0x20].height,
                        xOffset = this.m_characterDictionary[0x20].xOffset,
                        yOffset = this.m_characterDictionary[0x20].yOffset,
                        xAdvance = this.m_characterDictionary[0x20].xAdvance * this.TabSize
                    };
                    this.m_characterDictionary.Add(9, info2);
                }
                this.m_fontInfo.TabWidth = this.m_characterDictionary[0x20].xAdvance;
                this.m_kerningDictionary = new Dictionary<int, KerningPair>();
                List<KerningPair> kerningPairs = this.m_kerningInfo.kerningPairs;
                for (int i = 0; i < kerningPairs.Count; i++)
                {
                    KerningPair pair = kerningPairs[i];
                    KerningPairKey key = new KerningPairKey(pair.AscII_Left, pair.AscII_Right);
                    if (!this.m_kerningDictionary.ContainsKey(key.key))
                    {
                        this.m_kerningDictionary.Add(key.key, pair);
                    }
                    else
                    {
                        Debug.Log(string.Concat(new object[] { "Kerning Key for [", key.ascii_Left, "] and [", key.ascii_Right, "] already exists." }));
                    }
                }
                this.m_lineBreakingInfo = new LineBreakingTable();
                TextAsset file = Resources.Load("LineBreaking Leading Characters", typeof(TextAsset)) as TextAsset;
                if (file != null)
                {
                    this.m_lineBreakingInfo.leadingCharacters = this.GetCharacters(file);
                }
                TextAsset asset2 = Resources.Load("LineBreaking Following Characters", typeof(TextAsset)) as TextAsset;
                if (asset2 != null)
                {
                    this.m_lineBreakingInfo.followingCharacters = this.GetCharacters(asset2);
                }
                string name = base.name;
                this.fontHashCode = 0;
                for (int j = 0; j < name.Length; j++)
                {
                    this.fontHashCode = ((this.fontHashCode << 5) - this.fontHashCode) + name[j];
                }
                string str2 = this.material.name;
                this.materialHashCode = 0;
                for (int k = 0; k < str2.Length; k++)
                {
                    this.materialHashCode = ((this.materialHashCode << 5) - this.materialHashCode) + str2[k];
                }
            }
        }

        public Dictionary<int, GlyphInfo> characterDictionary
        {
            get
            {
                return this.m_characterDictionary;
            }
        }

        public FaceInfo fontInfo
        {
            get
            {
                return this.m_fontInfo;
            }
        }

        public Dictionary<int, KerningPair> kerningDictionary
        {
            get
            {
                return this.m_kerningDictionary;
            }
        }

        public KerningTable kerningInfo
        {
            get
            {
                return this.m_kerningInfo;
            }
        }

        public LineBreakingTable lineBreakingInfo
        {
            get
            {
                return this.m_lineBreakingInfo;
            }
        }
    }
}

