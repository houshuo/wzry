namespace com.tencent.pandora
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct LuaMethod
    {
        public string name;
        public LuaCSFunction func;
        public LuaMethod(string str, LuaCSFunction f)
        {
            this.name = str;
            this.func = f;
        }
    }
}

