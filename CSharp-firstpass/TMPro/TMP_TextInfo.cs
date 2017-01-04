namespace TMPro
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class TMP_TextInfo
    {
        public int characterCount;
        public TMP_CharacterInfo[] characterInfo = new TMP_CharacterInfo[0];
        public int lineCount;
        public TMP_LineInfo[] lineInfo = new TMP_LineInfo[0x10];
        public TMP_MeshInfo meshInfo = new TMP_MeshInfo();
        public int pageCount;
        public TMP_PageInfo[] pageInfo = new TMP_PageInfo[0x10];
        public int spaceCount;
        public int spriteCount;
        public int wordCount;
        public List<TMP_WordInfo> wordInfo = new List<TMP_WordInfo>(0x20);

        public TMP_TextInfo()
        {
            this.meshInfo.meshArrays = new UIVertex[0x11][];
        }

        public void Clear()
        {
            this.characterCount = 0;
            this.spaceCount = 0;
            this.wordCount = 0;
            this.lineCount = 0;
            this.pageCount = 0;
            this.spriteCount = 0;
            Array.Clear(this.characterInfo, 0, this.characterInfo.Length);
            this.wordInfo.Clear();
            Array.Clear(this.lineInfo, 0, this.lineInfo.Length);
            Array.Clear(this.pageInfo, 0, this.pageInfo.Length);
        }
    }
}

