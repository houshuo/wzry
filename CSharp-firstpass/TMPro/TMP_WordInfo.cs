namespace TMPro
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct TMP_WordInfo
    {
        public int firstCharacterIndex;
        public int lastCharacterIndex;
        public int characterCount;
        public float length;
        public string GetWord(TMP_CharacterInfo[] charInfo)
        {
            string str = string.Empty;
            for (int i = this.firstCharacterIndex; i < (this.lastCharacterIndex + 1); i++)
            {
                str = str + charInfo[i].character;
            }
            return str;
        }
    }
}

