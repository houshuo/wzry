namespace com.tencent.pandora
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate string LuaChunkReader(IntPtr luaState, ref ReaderInfo data, ref uint size);
}

