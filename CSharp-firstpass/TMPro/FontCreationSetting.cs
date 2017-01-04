namespace TMPro
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct FontCreationSetting
    {
        public string fontSourcePath;
        public int fontSizingMode;
        public int fontSize;
        public int fontPadding;
        public int fontPackingMode;
        public int fontAtlasWidth;
        public int fontAtlasHeight;
        public int fontCharacterSet;
        public int fontStyle;
        public float fontStlyeModifier;
        public int fontRenderMode;
        public bool fontKerning;
    }
}

