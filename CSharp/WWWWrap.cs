using com.tencent.pandora;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class WWWWrap
{
    private static System.Type classType = typeof(WWW);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateWWW(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if (num == 1)
        {
            WWW o = new WWW(LuaScriptMgr.GetString(L, 1));
            LuaScriptMgr.PushObject(L, o);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(byte[])))
        {
            string url = LuaScriptMgr.GetString(L, 1);
            byte[] arrayNumber = LuaScriptMgr.GetArrayNumber<byte>(L, 2);
            WWW www2 = new WWW(url, arrayNumber);
            LuaScriptMgr.PushObject(L, www2);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(WWWForm)))
        {
            string str3 = LuaScriptMgr.GetString(L, 1);
            WWWForm form = (WWWForm) LuaScriptMgr.GetNetObject(L, 2, typeof(WWWForm));
            WWW www3 = new WWW(str3, form);
            LuaScriptMgr.PushObject(L, www3);
            return 1;
        }
        if (num == 3)
        {
            string str4 = LuaScriptMgr.GetString(L, 1);
            byte[] postData = LuaScriptMgr.GetArrayNumber<byte>(L, 2);
            Dictionary<string, string> headers = (Dictionary<string, string>) LuaScriptMgr.GetNetObject(L, 3, typeof(Dictionary<string, string>));
            WWW www4 = new WWW(str4, postData, headers);
            LuaScriptMgr.PushObject(L, www4);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: WWW.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Dispose(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((WWW) LuaScriptMgr.GetNetObjectSelf(L, 1, "WWW")).Dispose();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int EscapeURL(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                string str = WWW.EscapeURL(LuaScriptMgr.GetLuaString(L, 1));
                LuaScriptMgr.Push(L, str);
                return 1;
            }
            case 2:
            {
                string luaString = LuaScriptMgr.GetLuaString(L, 1);
                Encoding e = (Encoding) LuaScriptMgr.GetNetObject(L, 2, typeof(Encoding));
                string str4 = WWW.EscapeURL(luaString, e);
                LuaScriptMgr.Push(L, str4);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: WWW.EscapeURL");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_assetBundle(IntPtr L)
    {
        WWW luaObject = (WWW) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name assetBundle");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index assetBundle on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.assetBundle);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_audioClip(IntPtr L)
    {
        WWW luaObject = (WWW) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name audioClip");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index audioClip on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.audioClip);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_bytes(IntPtr L)
    {
        WWW luaObject = (WWW) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name bytes");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index bytes on a nil value");
            }
        }
        LuaScriptMgr.PushArray(L, luaObject.bytes);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_bytesDownloaded(IntPtr L)
    {
        WWW luaObject = (WWW) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name bytesDownloaded");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index bytesDownloaded on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.bytesDownloaded);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_error(IntPtr L)
    {
        WWW luaObject = (WWW) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name error");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index error on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.error);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isDone(IntPtr L)
    {
        WWW luaObject = (WWW) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name isDone");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index isDone on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.isDone);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_progress(IntPtr L)
    {
        WWW luaObject = (WWW) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name progress");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index progress on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.progress);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_responseHeaders(IntPtr L)
    {
        WWW luaObject = (WWW) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name responseHeaders");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index responseHeaders on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.responseHeaders);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_size(IntPtr L)
    {
        WWW luaObject = (WWW) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name size");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index size on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.size);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_text(IntPtr L)
    {
        WWW luaObject = (WWW) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name text");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index text on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.text);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_texture(IntPtr L)
    {
        WWW luaObject = (WWW) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name texture");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index texture on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.texture);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_textureNonReadable(IntPtr L)
    {
        WWW luaObject = (WWW) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name textureNonReadable");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index textureNonReadable on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.textureNonReadable);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_threadPriority(IntPtr L)
    {
        WWW luaObject = (WWW) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name threadPriority");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index threadPriority on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.threadPriority);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_uploadProgress(IntPtr L)
    {
        WWW luaObject = (WWW) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name uploadProgress");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index uploadProgress on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.uploadProgress);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_url(IntPtr L)
    {
        WWW luaObject = (WWW) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name url");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index url on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.url);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetAudioClip(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                WWW www = (WWW) LuaScriptMgr.GetNetObjectSelf(L, 1, "WWW");
                bool boolean = LuaScriptMgr.GetBoolean(L, 2);
                AudioClip audioClip = www.GetAudioClip(boolean);
                LuaScriptMgr.Push(L, audioClip);
                return 1;
            }
            case 3:
            {
                WWW www2 = (WWW) LuaScriptMgr.GetNetObjectSelf(L, 1, "WWW");
                bool threeD = LuaScriptMgr.GetBoolean(L, 2);
                bool stream = LuaScriptMgr.GetBoolean(L, 3);
                AudioClip clip2 = www2.GetAudioClip(threeD, stream);
                LuaScriptMgr.Push(L, clip2);
                return 1;
            }
            case 4:
            {
                WWW www3 = (WWW) LuaScriptMgr.GetNetObjectSelf(L, 1, "WWW");
                bool flag4 = LuaScriptMgr.GetBoolean(L, 2);
                bool flag5 = LuaScriptMgr.GetBoolean(L, 3);
                AudioType audioType = (AudioType) ((int) LuaScriptMgr.GetNetObject(L, 4, typeof(AudioType)));
                AudioClip clip3 = www3.GetAudioClip(flag4, flag5, audioType);
                LuaScriptMgr.Push(L, clip3);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: WWW.GetAudioClip");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetAudioClipCompressed(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                AudioClip audioClipCompressed = ((WWW) LuaScriptMgr.GetNetObjectSelf(L, 1, "WWW")).GetAudioClipCompressed();
                LuaScriptMgr.Push(L, audioClipCompressed);
                return 1;
            }
            case 2:
            {
                WWW www2 = (WWW) LuaScriptMgr.GetNetObjectSelf(L, 1, "WWW");
                bool boolean = LuaScriptMgr.GetBoolean(L, 2);
                AudioClip clip2 = www2.GetAudioClipCompressed(boolean);
                LuaScriptMgr.Push(L, clip2);
                return 1;
            }
            case 3:
            {
                WWW www3 = (WWW) LuaScriptMgr.GetNetObjectSelf(L, 1, "WWW");
                bool threeD = LuaScriptMgr.GetBoolean(L, 2);
                AudioType audioType = (AudioType) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(AudioType)));
                AudioClip clip3 = www3.GetAudioClipCompressed(threeD, audioType);
                LuaScriptMgr.Push(L, clip3);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: WWW.GetAudioClipCompressed");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int InitWWW(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 4);
        WWW www = (WWW) LuaScriptMgr.GetNetObjectSelf(L, 1, "WWW");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        byte[] arrayNumber = LuaScriptMgr.GetArrayNumber<byte>(L, 3);
        string[] arrayString = LuaScriptMgr.GetArrayString(L, 4);
        www.InitWWW(luaString, arrayNumber, arrayString);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LoadFromCacheOrDownload(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                string luaString = LuaScriptMgr.GetLuaString(L, 1);
                int number = (int) LuaScriptMgr.GetNumber(L, 2);
                WWW o = WWW.LoadFromCacheOrDownload(luaString, number);
                LuaScriptMgr.PushObject(L, o);
                return 1;
            }
            case 3:
            {
                string url = LuaScriptMgr.GetLuaString(L, 1);
                int version = (int) LuaScriptMgr.GetNumber(L, 2);
                uint crc = (uint) LuaScriptMgr.GetNumber(L, 3);
                WWW www2 = WWW.LoadFromCacheOrDownload(url, version, crc);
                LuaScriptMgr.PushObject(L, www2);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: WWW.LoadFromCacheOrDownload");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LoadImageIntoTexture(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        WWW www = (WWW) LuaScriptMgr.GetNetObjectSelf(L, 1, "WWW");
        Texture2D tex = (Texture2D) LuaScriptMgr.GetUnityObject(L, 2, typeof(Texture2D));
        www.LoadImageIntoTexture(tex);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LoadUnityWeb(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((WWW) LuaScriptMgr.GetNetObjectSelf(L, 1, "WWW")).LoadUnityWeb();
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("Dispose", new LuaCSFunction(WWWWrap.Dispose)), new LuaMethod("InitWWW", new LuaCSFunction(WWWWrap.InitWWW)), new LuaMethod("EscapeURL", new LuaCSFunction(WWWWrap.EscapeURL)), new LuaMethod("UnEscapeURL", new LuaCSFunction(WWWWrap.UnEscapeURL)), new LuaMethod("GetAudioClip", new LuaCSFunction(WWWWrap.GetAudioClip)), new LuaMethod("GetAudioClipCompressed", new LuaCSFunction(WWWWrap.GetAudioClipCompressed)), new LuaMethod("LoadImageIntoTexture", new LuaCSFunction(WWWWrap.LoadImageIntoTexture)), new LuaMethod("LoadUnityWeb", new LuaCSFunction(WWWWrap.LoadUnityWeb)), new LuaMethod("LoadFromCacheOrDownload", new LuaCSFunction(WWWWrap.LoadFromCacheOrDownload)), new LuaMethod("New", new LuaCSFunction(WWWWrap._CreateWWW)), new LuaMethod("GetClassType", new LuaCSFunction(WWWWrap.GetClassType)) };
        LuaField[] fields = new LuaField[] { new LuaField("responseHeaders", new LuaCSFunction(WWWWrap.get_responseHeaders), null), new LuaField("text", new LuaCSFunction(WWWWrap.get_text), null), new LuaField("bytes", new LuaCSFunction(WWWWrap.get_bytes), null), new LuaField("size", new LuaCSFunction(WWWWrap.get_size), null), new LuaField("error", new LuaCSFunction(WWWWrap.get_error), null), new LuaField("texture", new LuaCSFunction(WWWWrap.get_texture), null), new LuaField("textureNonReadable", new LuaCSFunction(WWWWrap.get_textureNonReadable), null), new LuaField("audioClip", new LuaCSFunction(WWWWrap.get_audioClip), null), new LuaField("isDone", new LuaCSFunction(WWWWrap.get_isDone), null), new LuaField("progress", new LuaCSFunction(WWWWrap.get_progress), null), new LuaField("uploadProgress", new LuaCSFunction(WWWWrap.get_uploadProgress), null), new LuaField("bytesDownloaded", new LuaCSFunction(WWWWrap.get_bytesDownloaded), null), new LuaField("url", new LuaCSFunction(WWWWrap.get_url), null), new LuaField("assetBundle", new LuaCSFunction(WWWWrap.get_assetBundle), null), new LuaField("threadPriority", new LuaCSFunction(WWWWrap.get_threadPriority), new LuaCSFunction(WWWWrap.set_threadPriority)) };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.WWW", typeof(WWW), regs, fields, typeof(object));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_threadPriority(IntPtr L)
    {
        WWW luaObject = (WWW) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name threadPriority");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index threadPriority on a nil value");
            }
        }
        luaObject.threadPriority = (ThreadPriority) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(ThreadPriority)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int UnEscapeURL(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                string str = WWW.UnEscapeURL(LuaScriptMgr.GetLuaString(L, 1));
                LuaScriptMgr.Push(L, str);
                return 1;
            }
            case 2:
            {
                string luaString = LuaScriptMgr.GetLuaString(L, 1);
                Encoding e = (Encoding) LuaScriptMgr.GetNetObject(L, 2, typeof(Encoding));
                string str4 = WWW.UnEscapeURL(luaString, e);
                LuaScriptMgr.Push(L, str4);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: WWW.UnEscapeURL");
        return 0;
    }
}

