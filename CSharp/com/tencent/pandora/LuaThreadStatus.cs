namespace com.tencent.pandora
{
    using System;

    public enum LuaThreadStatus
    {
        LUA_ERRERR = 5,
        LUA_ERRMEM = 4,
        LUA_ERRRUN = 2,
        LUA_ERRSYNTAX = 3,
        LUA_YIELD = 1
    }
}

