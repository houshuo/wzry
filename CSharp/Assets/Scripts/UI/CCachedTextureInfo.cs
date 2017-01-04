namespace Assets.Scripts.UI
{
    using System;

    public class CCachedTextureInfo : IComparable
    {
        public int m_height;
        public bool m_isGif;
        public string m_key;
        public DateTime m_lastModifyTime;
        public int m_width;

        public int CompareTo(object obj)
        {
            CCachedTextureInfo info = obj as CCachedTextureInfo;
            if (this.m_lastModifyTime > info.m_lastModifyTime)
            {
                return 1;
            }
            if (this.m_lastModifyTime == info.m_lastModifyTime)
            {
                return 0;
            }
            return -1;
        }
    }
}

