namespace com.tencent.pandora
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class LuaHideAttribute : Attribute
    {
    }
}

