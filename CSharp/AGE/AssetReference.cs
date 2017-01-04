namespace AGE
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class AssetReference : Attribute
    {
        public AssetReference(AssetRefType refType)
        {
            this.RefType = refType;
        }

        public AssetRefType RefType { get; private set; }
    }
}

