namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;

    public static class DelegateFactory
    {
        private static Dictionary<System.Type, DelegateValue> dict = new Dictionary<System.Type, DelegateValue>();

        public static Delegate Action(LuaFunction func)
        {
            <Action>c__AnonStorey34 storey = new <Action>c__AnonStorey34 {
                func = func
            };
            return new System.Action(storey.<>m__A);
        }

        public static Delegate Action_AssetBundle(LuaFunction func)
        {
            <Action_AssetBundle>c__AnonStorey39 storey = new <Action_AssetBundle>c__AnonStorey39 {
                func = func
            };
            return new Action<AssetBundle>(storey.<>m__F);
        }

        public static Delegate Action_bool(LuaFunction func)
        {
            <Action_bool>c__AnonStorey38 storey = new <Action_bool>c__AnonStorey38 {
                func = func
            };
            return new Action<bool>(storey.<>m__E);
        }

        public static Delegate Action_GameObject(LuaFunction func)
        {
            <Action_GameObject>c__AnonStorey33 storey = new <Action_GameObject>c__AnonStorey33 {
                func = func
            };
            return new Action<GameObject>(storey.<>m__9);
        }

        public static Delegate Action_WWW_string(LuaFunction func)
        {
            <Action_WWW_string>c__AnonStorey3A storeya = new <Action_WWW_string>c__AnonStorey3A {
                func = func
            };
            return new Action<WWW, string>(storeya.<>m__10);
        }

        public static Delegate Application_LogCallback(LuaFunction func)
        {
            <Application_LogCallback>c__AnonStorey3B storeyb = new <Application_LogCallback>c__AnonStorey3B {
                func = func
            };
            return new Application.LogCallback(storeyb.<>m__11);
        }

        public static void Clear()
        {
            dict.Clear();
        }

        public static Delegate com_tencent_pandora_GetNewsImage_Callback(LuaFunction func)
        {
            <com_tencent_pandora_GetNewsImage_Callback>c__AnonStorey3D storeyd = new <com_tencent_pandora_GetNewsImage_Callback>c__AnonStorey3D {
                func = func
            };
            return new GetNewsImage.Callback(storeyd.<>m__13);
        }

        public static Delegate com_tencent_pandora_GetNewsImage_callbackFuc(LuaFunction func)
        {
            <com_tencent_pandora_GetNewsImage_callbackFuc>c__AnonStorey3C storeyc = new <com_tencent_pandora_GetNewsImage_callbackFuc>c__AnonStorey3C {
                func = func
            };
            return new GetNewsImage.callbackFuc(storeyc.<>m__12);
        }

        public static Delegate com_tencent_pandora_NetProxcy_CallBack(LuaFunction func)
        {
            <com_tencent_pandora_NetProxcy_CallBack>c__AnonStorey3E storeye = new <com_tencent_pandora_NetProxcy_CallBack>c__AnonStorey3E {
                func = func
            };
            return new NetProxcy.CallBack(storeye.<>m__14);
        }

        [NoToLua]
        public static Delegate CreateDelegate(System.Type t, LuaFunction func)
        {
            DelegateValue value2 = null;
            if (!dict.TryGetValue(t, out value2))
            {
                object[] args = new object[] { t.FullName };
                Debugger.LogError("Delegate {0} not register", args);
                return null;
            }
            return value2(func);
        }

        [NoToLua]
        public static void Register(IntPtr L)
        {
            dict.Add(typeof(Action<GameObject>), new DelegateValue(DelegateFactory.Action_GameObject));
            dict.Add(typeof(System.Action), new DelegateValue(DelegateFactory.Action));
            dict.Add(typeof(UnityAction), new DelegateValue(DelegateFactory.UnityEngine_Events_UnityAction));
            dict.Add(typeof(MemberFilter), new DelegateValue(DelegateFactory.System_Reflection_MemberFilter));
            dict.Add(typeof(TypeFilter), new DelegateValue(DelegateFactory.System_Reflection_TypeFilter));
            dict.Add(typeof(Action<bool>), new DelegateValue(DelegateFactory.Action_bool));
            dict.Add(typeof(Action<AssetBundle>), new DelegateValue(DelegateFactory.Action_AssetBundle));
            dict.Add(typeof(Action<WWW, string>), new DelegateValue(DelegateFactory.Action_WWW_string));
            dict.Add(typeof(Application.LogCallback), new DelegateValue(DelegateFactory.Application_LogCallback));
            dict.Add(typeof(GetNewsImage.callbackFuc), new DelegateValue(DelegateFactory.com_tencent_pandora_GetNewsImage_callbackFuc));
            dict.Add(typeof(GetNewsImage.Callback), new DelegateValue(DelegateFactory.com_tencent_pandora_GetNewsImage_Callback));
            dict.Add(typeof(NetProxcy.CallBack), new DelegateValue(DelegateFactory.com_tencent_pandora_NetProxcy_CallBack));
        }

        public static Delegate System_Reflection_MemberFilter(LuaFunction func)
        {
            <System_Reflection_MemberFilter>c__AnonStorey36 storey = new <System_Reflection_MemberFilter>c__AnonStorey36 {
                func = func
            };
            return new MemberFilter(storey.<>m__C);
        }

        public static Delegate System_Reflection_TypeFilter(LuaFunction func)
        {
            <System_Reflection_TypeFilter>c__AnonStorey37 storey = new <System_Reflection_TypeFilter>c__AnonStorey37 {
                func = func
            };
            return new TypeFilter(storey.<>m__D);
        }

        public static Delegate UnityEngine_Events_UnityAction(LuaFunction func)
        {
            <UnityEngine_Events_UnityAction>c__AnonStorey35 storey = new <UnityEngine_Events_UnityAction>c__AnonStorey35 {
                func = func
            };
            return new UnityAction(storey.<>m__B);
        }

        [CompilerGenerated]
        private sealed class <Action_AssetBundle>c__AnonStorey39
        {
            internal LuaFunction func;

            internal void <>m__F(AssetBundle param0)
            {
                int oldTop = this.func.BeginPCall();
                LuaScriptMgr.Push(this.func.GetLuaState(), param0);
                this.func.PCall(oldTop, 1);
                this.func.EndPCall(oldTop);
            }
        }

        [CompilerGenerated]
        private sealed class <Action_bool>c__AnonStorey38
        {
            internal LuaFunction func;

            internal void <>m__E(bool param0)
            {
                int oldTop = this.func.BeginPCall();
                LuaScriptMgr.Push(this.func.GetLuaState(), param0);
                this.func.PCall(oldTop, 1);
                this.func.EndPCall(oldTop);
            }
        }

        [CompilerGenerated]
        private sealed class <Action_GameObject>c__AnonStorey33
        {
            internal LuaFunction func;

            internal void <>m__9(GameObject param0)
            {
                int oldTop = this.func.BeginPCall();
                LuaScriptMgr.Push(this.func.GetLuaState(), param0);
                this.func.PCall(oldTop, 1);
                this.func.EndPCall(oldTop);
            }
        }

        [CompilerGenerated]
        private sealed class <Action_WWW_string>c__AnonStorey3A
        {
            internal LuaFunction func;

            internal void <>m__10(WWW param0, string param1)
            {
                int oldTop = this.func.BeginPCall();
                IntPtr luaState = this.func.GetLuaState();
                LuaScriptMgr.PushObject(luaState, param0);
                LuaScriptMgr.Push(luaState, param1);
                this.func.PCall(oldTop, 2);
                this.func.EndPCall(oldTop);
            }
        }

        [CompilerGenerated]
        private sealed class <Action>c__AnonStorey34
        {
            internal LuaFunction func;

            internal void <>m__A()
            {
                this.func.Call();
            }
        }

        [CompilerGenerated]
        private sealed class <Application_LogCallback>c__AnonStorey3B
        {
            internal LuaFunction func;

            internal void <>m__11(string param0, string param1, LogType param2)
            {
                int oldTop = this.func.BeginPCall();
                IntPtr luaState = this.func.GetLuaState();
                LuaScriptMgr.Push(luaState, param0);
                LuaScriptMgr.Push(luaState, param1);
                LuaScriptMgr.Push(luaState, param2);
                this.func.PCall(oldTop, 3);
                this.func.EndPCall(oldTop);
            }
        }

        [CompilerGenerated]
        private sealed class <com_tencent_pandora_GetNewsImage_Callback>c__AnonStorey3D
        {
            internal LuaFunction func;

            internal void <>m__13(string param0, Texture2D param1)
            {
                int oldTop = this.func.BeginPCall();
                IntPtr luaState = this.func.GetLuaState();
                LuaScriptMgr.Push(luaState, param0);
                LuaScriptMgr.Push(luaState, param1);
                this.func.PCall(oldTop, 2);
                this.func.EndPCall(oldTop);
            }
        }

        [CompilerGenerated]
        private sealed class <com_tencent_pandora_GetNewsImage_callbackFuc>c__AnonStorey3C
        {
            internal LuaFunction func;

            internal void <>m__12(string param0, GameObject param1)
            {
                int oldTop = this.func.BeginPCall();
                IntPtr luaState = this.func.GetLuaState();
                LuaScriptMgr.Push(luaState, param0);
                LuaScriptMgr.Push(luaState, param1);
                this.func.PCall(oldTop, 2);
                this.func.EndPCall(oldTop);
            }
        }

        [CompilerGenerated]
        private sealed class <com_tencent_pandora_NetProxcy_CallBack>c__AnonStorey3E
        {
            internal LuaFunction func;

            internal void <>m__14(string param0, int param1, int param2)
            {
                int oldTop = this.func.BeginPCall();
                IntPtr luaState = this.func.GetLuaState();
                LuaScriptMgr.Push(luaState, param0);
                LuaScriptMgr.Push(luaState, param1);
                LuaScriptMgr.Push(luaState, param2);
                this.func.PCall(oldTop, 3);
                this.func.EndPCall(oldTop);
            }
        }

        [CompilerGenerated]
        private sealed class <System_Reflection_MemberFilter>c__AnonStorey36
        {
            internal LuaFunction func;

            internal bool <>m__C(MemberInfo param0, object param1)
            {
                int oldTop = this.func.BeginPCall();
                IntPtr luaState = this.func.GetLuaState();
                LuaScriptMgr.PushObject(luaState, param0);
                LuaScriptMgr.PushVarObject(luaState, param1);
                this.func.PCall(oldTop, 2);
                object[] objArray = this.func.PopValues(oldTop);
                this.func.EndPCall(oldTop);
                return (bool) objArray[0];
            }
        }

        [CompilerGenerated]
        private sealed class <System_Reflection_TypeFilter>c__AnonStorey37
        {
            internal LuaFunction func;

            internal bool <>m__D(System.Type param0, object param1)
            {
                int oldTop = this.func.BeginPCall();
                IntPtr luaState = this.func.GetLuaState();
                LuaScriptMgr.Push(luaState, param0);
                LuaScriptMgr.PushVarObject(luaState, param1);
                this.func.PCall(oldTop, 2);
                object[] objArray = this.func.PopValues(oldTop);
                this.func.EndPCall(oldTop);
                return (bool) objArray[0];
            }
        }

        [CompilerGenerated]
        private sealed class <UnityEngine_Events_UnityAction>c__AnonStorey35
        {
            internal LuaFunction func;

            internal void <>m__B()
            {
                this.func.Call();
            }
        }

        private delegate Delegate DelegateValue(LuaFunction func);
    }
}

