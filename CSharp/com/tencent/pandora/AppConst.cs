namespace com.tencent.pandora
{
    using System;
    using UnityEngine;

    public class AppConst
    {
        public const string AppName = "6l";
        public const string AppPrefix = "6l_";
        public const bool AutoWrapMode = true;
        public const bool DebugMode = false;
        public const bool ExampleMode = true;
        public const int GameFrameRate = 30;
        public const bool LuaEncode = false;
        public static string SocketAddress = string.Empty;
        public static int SocketPort = 0;
        public const int TimerInterval = 1;
        public const bool UpdateMode = false;
        public const bool UseCJson = true;
        public const bool UseLpeg = false;
        public const bool UsePbc = false;
        public const bool UsePbLua = false;
        public static string UserId = string.Empty;
        public const bool UseSproto = false;
        public const string WebUrl = "http://localhost:6688/";

        public static string LuaBasePath
        {
            get
            {
                return (Application.dataPath + "/Resources/Pandora/Lua/uLua/Source/");
            }
        }

        public static string LuaWrapPath
        {
            get
            {
                return (LuaBasePath + "LuaWrap/");
            }
        }
    }
}

