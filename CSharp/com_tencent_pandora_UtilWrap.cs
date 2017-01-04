using com.tencent.pandora;
using System;
using UnityEngine;

public class com_tencent_pandora_UtilWrap
{
    private static System.Type classType = typeof(Util);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _Createcom_tencent_pandora_Util(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            Util o = new Util();
            LuaScriptMgr.PushObject(L, o);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: com.tencent.pandora.Util.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int AddComponent(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        GameObject go = (GameObject) LuaScriptMgr.GetUnityObject(L, 1, typeof(GameObject));
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        string classname = LuaScriptMgr.GetLuaString(L, 3);
        Component component = Util.AddComponent(go, luaString, classname);
        LuaScriptMgr.Push(L, component);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int AddLuaPath(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Util.AddLuaPath(LuaScriptMgr.GetLuaString(L, 1));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int AppContentPath(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        string str = Util.AppContentPath();
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CallMethod(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        string luaString = LuaScriptMgr.GetLuaString(L, 1);
        string func = LuaScriptMgr.GetLuaString(L, 2);
        object[] args = LuaScriptMgr.GetParamsObject(L, 3, num - 2);
        object[] o = Util.CallMethod(luaString, func, args);
        LuaScriptMgr.PushArray(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CheckEnvironment(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        bool b = Util.CheckEnvironment();
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Child(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(Transform), typeof(string)))
        {
            Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
            string subnode = LuaScriptMgr.GetString(L, 2);
            GameObject obj2 = Util.Child(luaObject, subnode);
            LuaScriptMgr.Push(L, obj2);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(string)))
        {
            GameObject go = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
            string str2 = LuaScriptMgr.GetString(L, 2);
            GameObject obj4 = Util.Child(go, str2);
            LuaScriptMgr.Push(L, obj4);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: com.tencent.pandora.Util.Child");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ClearChild(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Transform go = (Transform) LuaScriptMgr.GetUnityObject(L, 1, typeof(Transform));
        Util.ClearChild(go);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ClearMemory(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        Util.ClearMemory();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Decode(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string str = Util.Decode(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Encode(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string str = Util.Encode(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Float(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        float d = Util.Float(LuaScriptMgr.GetVarObject(L, 1));
        LuaScriptMgr.Push(L, d);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_DataPath(IntPtr L)
    {
        LuaScriptMgr.Push(L, Util.DataPath);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isApplePlatform(IntPtr L)
    {
        LuaScriptMgr.Push(L, Util.isApplePlatform);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isFight(IntPtr L)
    {
        LuaScriptMgr.Push(L, Util.isFight);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isLogin(IntPtr L)
    {
        LuaScriptMgr.Push(L, Util.isLogin);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isMain(IntPtr L)
    {
        LuaScriptMgr.Push(L, Util.isMain);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsWifi(IntPtr L)
    {
        LuaScriptMgr.Push(L, Util.IsWifi);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_NetAvailable(IntPtr L)
    {
        LuaScriptMgr.Push(L, Util.NetAvailable);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int getassetbundle(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string str = Util.getassetbundle(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetFileText(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string fileText = Util.GetFileText(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, fileText);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetInt(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int @int = Util.GetInt(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, @int);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetKey(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string key = Util.GetKey(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, key);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetString(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string str = Util.GetString(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetTime(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        long time = Util.GetTime();
        LuaScriptMgr.Push(L, time);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int HashToMD5Hex(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string str = Util.HashToMD5Hex(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int HasKey(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        bool b = Util.HasKey(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Int(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int d = Util.Int(LuaScriptMgr.GetVarObject(L, 1));
        LuaScriptMgr.Push(L, d);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IsNumber(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        bool b = Util.IsNumber(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IsNumeric(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        bool b = Util.IsNumeric(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LoadAsset(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        AssetBundle bundle = (AssetBundle) LuaScriptMgr.GetUnityObject(L, 1, typeof(AssetBundle));
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        GameObject obj2 = Util.LoadAsset(bundle, luaString);
        LuaScriptMgr.Push(L, obj2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LoadPrefab(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        GameObject obj2 = Util.LoadPrefab(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, obj2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Log(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Util.Log(LuaScriptMgr.GetLuaString(L, 1));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LogError(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Util.LogError(LuaScriptMgr.GetLuaString(L, 1));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LogWarning(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Util.LogWarning(LuaScriptMgr.GetLuaString(L, 1));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Long(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        long d = Util.Long(LuaScriptMgr.GetVarObject(L, 1));
        LuaScriptMgr.Push(L, d);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LuaPath(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string str = Util.LuaPath(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int md5(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string str = Util.md5(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int md5file(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string str = Util.md5file(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Peer(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(Transform), typeof(string)))
        {
            Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
            string subnode = LuaScriptMgr.GetString(L, 2);
            GameObject obj2 = Util.Peer(luaObject, subnode);
            LuaScriptMgr.Push(L, obj2);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(string)))
        {
            GameObject go = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
            string str2 = LuaScriptMgr.GetString(L, 2);
            GameObject obj4 = Util.Peer(go, str2);
            LuaScriptMgr.Push(L, obj4);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: com.tencent.pandora.Util.Peer");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Random(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(float), typeof(float)))
        {
            float min = (float) LuaDLL.lua_tonumber(L, 1);
            float max = (float) LuaDLL.lua_tonumber(L, 2);
            float d = Util.Random(min, max);
            LuaScriptMgr.Push(L, d);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(int), typeof(int)))
        {
            int num5 = (int) LuaDLL.lua_tonumber(L, 1);
            int num6 = (int) LuaDLL.lua_tonumber(L, 2);
            int num7 = Util.Random(num5, num6);
            LuaScriptMgr.Push(L, num7);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: com.tencent.pandora.Util.Random");
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { 
            new LuaMethod("Int", new LuaCSFunction(com_tencent_pandora_UtilWrap.Int)), new LuaMethod("Float", new LuaCSFunction(com_tencent_pandora_UtilWrap.Float)), new LuaMethod("Long", new LuaCSFunction(com_tencent_pandora_UtilWrap.Long)), new LuaMethod("Random", new LuaCSFunction(com_tencent_pandora_UtilWrap.Random)), new LuaMethod("Uid", new LuaCSFunction(com_tencent_pandora_UtilWrap.Uid)), new LuaMethod("GetTime", new LuaCSFunction(com_tencent_pandora_UtilWrap.GetTime)), new LuaMethod("Child", new LuaCSFunction(com_tencent_pandora_UtilWrap.Child)), new LuaMethod("Peer", new LuaCSFunction(com_tencent_pandora_UtilWrap.Peer)), new LuaMethod("Vibrate", new LuaCSFunction(com_tencent_pandora_UtilWrap.Vibrate)), new LuaMethod("Encode", new LuaCSFunction(com_tencent_pandora_UtilWrap.Encode)), new LuaMethod("Decode", new LuaCSFunction(com_tencent_pandora_UtilWrap.Decode)), new LuaMethod("IsNumeric", new LuaCSFunction(com_tencent_pandora_UtilWrap.IsNumeric)), new LuaMethod("HashToMD5Hex", new LuaCSFunction(com_tencent_pandora_UtilWrap.HashToMD5Hex)), new LuaMethod("md5", new LuaCSFunction(com_tencent_pandora_UtilWrap.md5)), new LuaMethod("md5file", new LuaCSFunction(com_tencent_pandora_UtilWrap.md5file)), new LuaMethod("ClearChild", new LuaCSFunction(com_tencent_pandora_UtilWrap.ClearChild)), 
            new LuaMethod("GetKey", new LuaCSFunction(com_tencent_pandora_UtilWrap.GetKey)), new LuaMethod("GetInt", new LuaCSFunction(com_tencent_pandora_UtilWrap.GetInt)), new LuaMethod("HasKey", new LuaCSFunction(com_tencent_pandora_UtilWrap.HasKey)), new LuaMethod("SetInt", new LuaCSFunction(com_tencent_pandora_UtilWrap.SetInt)), new LuaMethod("GetString", new LuaCSFunction(com_tencent_pandora_UtilWrap.GetString)), new LuaMethod("SetString", new LuaCSFunction(com_tencent_pandora_UtilWrap.SetString)), new LuaMethod("RemoveData", new LuaCSFunction(com_tencent_pandora_UtilWrap.RemoveData)), new LuaMethod("ClearMemory", new LuaCSFunction(com_tencent_pandora_UtilWrap.ClearMemory)), new LuaMethod("IsNumber", new LuaCSFunction(com_tencent_pandora_UtilWrap.IsNumber)), new LuaMethod("GetFileText", new LuaCSFunction(com_tencent_pandora_UtilWrap.GetFileText)), new LuaMethod("AppContentPath", new LuaCSFunction(com_tencent_pandora_UtilWrap.AppContentPath)), new LuaMethod("getassetbundle", new LuaCSFunction(com_tencent_pandora_UtilWrap.getassetbundle)), new LuaMethod("LuaPath", new LuaCSFunction(com_tencent_pandora_UtilWrap.LuaPath)), new LuaMethod("SearchLuaPath", new LuaCSFunction(com_tencent_pandora_UtilWrap.SearchLuaPath)), new LuaMethod("AddLuaPath", new LuaCSFunction(com_tencent_pandora_UtilWrap.AddLuaPath)), new LuaMethod("RemoveLuaPath", new LuaCSFunction(com_tencent_pandora_UtilWrap.RemoveLuaPath)), 
            new LuaMethod("Log", new LuaCSFunction(com_tencent_pandora_UtilWrap.Log)), new LuaMethod("LogWarning", new LuaCSFunction(com_tencent_pandora_UtilWrap.LogWarning)), new LuaMethod("LogError", new LuaCSFunction(com_tencent_pandora_UtilWrap.LogError)), new LuaMethod("LoadAsset", new LuaCSFunction(com_tencent_pandora_UtilWrap.LoadAsset)), new LuaMethod("AddComponent", new LuaCSFunction(com_tencent_pandora_UtilWrap.AddComponent)), new LuaMethod("LoadPrefab", new LuaCSFunction(com_tencent_pandora_UtilWrap.LoadPrefab)), new LuaMethod("CallMethod", new LuaCSFunction(com_tencent_pandora_UtilWrap.CallMethod)), new LuaMethod("CheckEnvironment", new LuaCSFunction(com_tencent_pandora_UtilWrap.CheckEnvironment)), new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_UtilWrap._Createcom_tencent_pandora_Util)), new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_UtilWrap.GetClassType))
         };
        LuaField[] fields = new LuaField[] { new LuaField("DataPath", new LuaCSFunction(com_tencent_pandora_UtilWrap.get_DataPath), null), new LuaField("NetAvailable", new LuaCSFunction(com_tencent_pandora_UtilWrap.get_NetAvailable), null), new LuaField("IsWifi", new LuaCSFunction(com_tencent_pandora_UtilWrap.get_IsWifi), null), new LuaField("isLogin", new LuaCSFunction(com_tencent_pandora_UtilWrap.get_isLogin), null), new LuaField("isMain", new LuaCSFunction(com_tencent_pandora_UtilWrap.get_isMain), null), new LuaField("isFight", new LuaCSFunction(com_tencent_pandora_UtilWrap.get_isFight), null), new LuaField("isApplePlatform", new LuaCSFunction(com_tencent_pandora_UtilWrap.get_isApplePlatform), null) };
        LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.Util", typeof(Util), regs, fields, typeof(object));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int RemoveData(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Util.RemoveData(LuaScriptMgr.GetLuaString(L, 1));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int RemoveLuaPath(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Util.RemoveLuaPath(LuaScriptMgr.GetLuaString(L, 1));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SearchLuaPath(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string str = Util.SearchLuaPath(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetInt(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        string luaString = LuaScriptMgr.GetLuaString(L, 1);
        int number = (int) LuaScriptMgr.GetNumber(L, 2);
        Util.SetInt(luaString, number);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetString(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        string luaString = LuaScriptMgr.GetLuaString(L, 1);
        string str2 = LuaScriptMgr.GetLuaString(L, 2);
        Util.SetString(luaString, str2);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Uid(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string str = Util.Uid(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Vibrate(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        Util.Vibrate();
        return 0;
    }
}

