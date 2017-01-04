namespace com.tencent.pandora
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct LuaField
    {
        public string name;
        public LuaCSFunction getter;
        public LuaCSFunction setter;
        public LuaField(string str, LuaCSFunction g, LuaCSFunction s)
        {
            this.name = str;
            this.getter = g;
            this.setter = s;
        }
    }
}

