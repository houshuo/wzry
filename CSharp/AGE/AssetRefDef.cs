namespace AGE
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct AssetRefDef
    {
        public int markID;
        public AssetRefType assetType;
    }
}

