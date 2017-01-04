using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public static class LuaBinder
{
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$map1;
    public static List<string> wrapList = new List<string>();

    public static void Bind(IntPtr L, string type = null)
    {
        if ((type != null) && !wrapList.Contains(type))
        {
            wrapList.Add(type);
            type = type + "Wrap";
            string key = type;
            if (key != null)
            {
                int num;
                if (<>f__switch$map1 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(0x23);
                    dictionary.Add("ApplicationWrap", 0);
                    dictionary.Add("AssetBundleWrap", 1);
                    dictionary.Add("BehaviourWrap", 2);
                    dictionary.Add("ComponentWrap", 3);
                    dictionary.Add("com_tencent_pandora_AppConstWrap", 4);
                    dictionary.Add("com_tencent_pandora_CUserDataWrap", 5);
                    dictionary.Add("com_tencent_pandora_DelegateFactoryWrap", 6);
                    dictionary.Add("com_tencent_pandora_GetNewsImageWrap", 7);
                    dictionary.Add("com_tencent_pandora_LoggerWrap", 8);
                    dictionary.Add("com_tencent_pandora_LuaBehaviourWrap", 9);
                    dictionary.Add("com_tencent_pandora_LuaHelperWrap", 10);
                    dictionary.Add("com_tencent_pandora_NetProxcyWrap", 11);
                    dictionary.Add("com_tencent_pandora_NotificationCenterWrap", 12);
                    dictionary.Add("com_tencent_pandora_PdrWrap", 13);
                    dictionary.Add("com_tencent_pandora_ResourceManagerWrap", 14);
                    dictionary.Add("com_tencent_pandora_ThirdSDKWrap", 15);
                    dictionary.Add("com_tencent_pandora_UtilWrap", 0x10);
                    dictionary.Add("DebuggerWrap", 0x11);
                    dictionary.Add("Dictionary_string_ObjectWrap", 0x12);
                    dictionary.Add("EnumWrap", 0x13);
                    dictionary.Add("GameObjectWrap", 20);
                    dictionary.Add("IEnumeratorWrap", 0x15);
                    dictionary.Add("MonoBehaviourWrap", 0x16);
                    dictionary.Add("ObjectWrap", 0x17);
                    dictionary.Add("RectWrap", 0x18);
                    dictionary.Add("stringWrap", 0x19);
                    dictionary.Add("System_ObjectWrap", 0x1a);
                    dictionary.Add("TextureWrap", 0x1b);
                    dictionary.Add("TimeWrap", 0x1c);
                    dictionary.Add("TransformWrap", 0x1d);
                    dictionary.Add("TypeWrap", 30);
                    dictionary.Add("UnityEngine_UI_ImageWrap", 0x1f);
                    dictionary.Add("Vector2Wrap", 0x20);
                    dictionary.Add("Vector3Wrap", 0x21);
                    dictionary.Add("WWWWrap", 0x22);
                    <>f__switch$map1 = dictionary;
                }
                if (<>f__switch$map1.TryGetValue(key, out num))
                {
                    switch (num)
                    {
                        case 0:
                            ApplicationWrap.Register(L);
                            break;

                        case 1:
                            AssetBundleWrap.Register(L);
                            break;

                        case 2:
                            BehaviourWrap.Register(L);
                            break;

                        case 3:
                            ComponentWrap.Register(L);
                            break;

                        case 4:
                            com_tencent_pandora_AppConstWrap.Register(L);
                            break;

                        case 5:
                            com_tencent_pandora_CUserDataWrap.Register(L);
                            break;

                        case 6:
                            com_tencent_pandora_DelegateFactoryWrap.Register(L);
                            break;

                        case 7:
                            com_tencent_pandora_GetNewsImageWrap.Register(L);
                            break;

                        case 8:
                            com_tencent_pandora_LoggerWrap.Register(L);
                            break;

                        case 9:
                            com_tencent_pandora_LuaBehaviourWrap.Register(L);
                            break;

                        case 10:
                            com_tencent_pandora_LuaHelperWrap.Register(L);
                            break;

                        case 11:
                            com_tencent_pandora_NetProxcyWrap.Register(L);
                            break;

                        case 12:
                            com_tencent_pandora_NotificationCenterWrap.Register(L);
                            break;

                        case 13:
                            com_tencent_pandora_PdrWrap.Register(L);
                            break;

                        case 14:
                            com_tencent_pandora_ResourceManagerWrap.Register(L);
                            break;

                        case 15:
                            com_tencent_pandora_ThirdSDKWrap.Register(L);
                            break;

                        case 0x10:
                            com_tencent_pandora_UtilWrap.Register(L);
                            break;

                        case 0x11:
                            DebuggerWrap.Register(L);
                            break;

                        case 0x12:
                            Dictionary_string_ObjectWrap.Register(L);
                            break;

                        case 0x13:
                            EnumWrap.Register(L);
                            break;

                        case 20:
                            GameObjectWrap.Register(L);
                            break;

                        case 0x15:
                            IEnumeratorWrap.Register(L);
                            break;

                        case 0x16:
                            MonoBehaviourWrap.Register(L);
                            break;

                        case 0x17:
                            ObjectWrap.Register(L);
                            break;

                        case 0x18:
                            RectWrap.Register(L);
                            break;

                        case 0x19:
                            stringWrap.Register(L);
                            break;

                        case 0x1a:
                            System_ObjectWrap.Register(L);
                            break;

                        case 0x1b:
                            TextureWrap.Register(L);
                            break;

                        case 0x1c:
                            TimeWrap.Register(L);
                            break;

                        case 0x1d:
                            TransformWrap.Register(L);
                            break;

                        case 30:
                            TypeWrap.Register(L);
                            break;

                        case 0x1f:
                            UnityEngine_UI_ImageWrap.Register(L);
                            break;

                        case 0x20:
                            Vector2Wrap.Register(L);
                            break;

                        case 0x21:
                            Vector3Wrap.Register(L);
                            break;

                        case 0x22:
                            WWWWrap.Register(L);
                            break;
                    }
                }
            }
        }
    }
}

