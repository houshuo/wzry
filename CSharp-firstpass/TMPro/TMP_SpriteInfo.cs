namespace TMPro
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct TMP_SpriteInfo
    {
        public int spriteIndex;
        public int characterIndex;
        public int vertexIndex;
    }
}

