namespace AGE
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct AssetReferenceMeta
    {
        public AssetReference Reference;
        public FieldInfo MetaFieldInfo;
    }
}

