namespace com.tencent.pandora
{
    using System;

    public enum LuaTypes
    {
        LUA_TBOOLEAN = 1,
        LUA_TFUNCTION = 6,
        LUA_TLIGHTUSERDATA = 2,
        LUA_TNIL = 0,
        LUA_TNONE = -1,
        LUA_TNUMBER = 3,
        LUA_TSTRING = 4,
        LUA_TTABLE = 5,
        LUA_TTHREAD = 8,
        LUA_TUSERDATA = 7
    }
}

