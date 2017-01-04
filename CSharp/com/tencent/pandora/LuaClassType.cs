namespace com.tencent.pandora
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct LuaClassType
    {
        public Type klass;
        public Type[][] returnTypes;
    }
}

