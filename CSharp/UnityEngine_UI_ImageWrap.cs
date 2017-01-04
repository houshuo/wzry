using com.tencent.pandora;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UnityEngine_UI_ImageWrap
{
    private static System.Type classType = typeof(Image);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateUnityEngine_UI_Image(IntPtr L)
    {
        LuaDLL.luaL_error(L, "UnityEngine.UI.Image class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CalculateLayoutInputHorizontal(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((Image) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Image")).CalculateLayoutInputHorizontal();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CalculateLayoutInputVertical(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((Image) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Image")).CalculateLayoutInputVertical();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_eventAlphaThreshold(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name eventAlphaThreshold");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index eventAlphaThreshold on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.eventAlphaThreshold);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_fillAmount(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name fillAmount");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index fillAmount on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.fillAmount);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_fillCenter(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name fillCenter");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index fillCenter on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.fillCenter);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_fillClockwise(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name fillClockwise");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index fillClockwise on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.fillClockwise);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_fillMethod(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name fillMethod");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index fillMethod on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.fillMethod);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_fillOrigin(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name fillOrigin");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index fillOrigin on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.fillOrigin);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_flexibleHeight(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name flexibleHeight");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index flexibleHeight on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.flexibleHeight);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_flexibleWidth(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name flexibleWidth");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index flexibleWidth on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.flexibleWidth);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_hasBorder(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name hasBorder");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index hasBorder on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.hasBorder);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_layoutPriority(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name layoutPriority");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index layoutPriority on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.layoutPriority);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_mainTexture(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name mainTexture");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index mainTexture on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.mainTexture);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_minHeight(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name minHeight");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index minHeight on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.minHeight);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_minWidth(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name minWidth");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index minWidth on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.minWidth);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_overrideSprite(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name overrideSprite");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index overrideSprite on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.overrideSprite);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_pixelsPerUnit(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name pixelsPerUnit");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index pixelsPerUnit on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.pixelsPerUnit);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_preferredHeight(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name preferredHeight");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index preferredHeight on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.preferredHeight);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_preferredWidth(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name preferredWidth");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index preferredWidth on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.preferredWidth);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_preserveAspect(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name preserveAspect");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index preserveAspect on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.preserveAspect);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_sprite(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name sprite");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index sprite on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.sprite);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_type(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name type");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index type on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.type);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IsRaycastLocationValid(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        Image image = (Image) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Image");
        Vector2 screenPoint = LuaScriptMgr.GetVector2(L, 2);
        Camera eventCamera = (Camera) LuaScriptMgr.GetUnityObject(L, 3, typeof(Camera));
        bool b = image.IsRaycastLocationValid(screenPoint, eventCamera);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Lua_Eq(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        UnityEngine.Object luaObject = LuaScriptMgr.GetLuaObject(L, 1) as UnityEngine.Object;
        UnityEngine.Object obj3 = LuaScriptMgr.GetLuaObject(L, 2) as UnityEngine.Object;
        bool b = luaObject == obj3;
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnAfterDeserialize(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((Image) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Image")).OnAfterDeserialize();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnBeforeSerialize(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((Image) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Image")).OnBeforeSerialize();
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("OnBeforeSerialize", new LuaCSFunction(UnityEngine_UI_ImageWrap.OnBeforeSerialize)), new LuaMethod("OnAfterDeserialize", new LuaCSFunction(UnityEngine_UI_ImageWrap.OnAfterDeserialize)), new LuaMethod("SetNativeSize", new LuaCSFunction(UnityEngine_UI_ImageWrap.SetNativeSize)), new LuaMethod("CalculateLayoutInputHorizontal", new LuaCSFunction(UnityEngine_UI_ImageWrap.CalculateLayoutInputHorizontal)), new LuaMethod("CalculateLayoutInputVertical", new LuaCSFunction(UnityEngine_UI_ImageWrap.CalculateLayoutInputVertical)), new LuaMethod("IsRaycastLocationValid", new LuaCSFunction(UnityEngine_UI_ImageWrap.IsRaycastLocationValid)), new LuaMethod("New", new LuaCSFunction(UnityEngine_UI_ImageWrap._CreateUnityEngine_UI_Image)), new LuaMethod("GetClassType", new LuaCSFunction(UnityEngine_UI_ImageWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(UnityEngine_UI_ImageWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { 
            new LuaField("sprite", new LuaCSFunction(UnityEngine_UI_ImageWrap.get_sprite), new LuaCSFunction(UnityEngine_UI_ImageWrap.set_sprite)), new LuaField("overrideSprite", new LuaCSFunction(UnityEngine_UI_ImageWrap.get_overrideSprite), new LuaCSFunction(UnityEngine_UI_ImageWrap.set_overrideSprite)), new LuaField("type", new LuaCSFunction(UnityEngine_UI_ImageWrap.get_type), new LuaCSFunction(UnityEngine_UI_ImageWrap.set_type)), new LuaField("preserveAspect", new LuaCSFunction(UnityEngine_UI_ImageWrap.get_preserveAspect), new LuaCSFunction(UnityEngine_UI_ImageWrap.set_preserveAspect)), new LuaField("fillCenter", new LuaCSFunction(UnityEngine_UI_ImageWrap.get_fillCenter), new LuaCSFunction(UnityEngine_UI_ImageWrap.set_fillCenter)), new LuaField("fillMethod", new LuaCSFunction(UnityEngine_UI_ImageWrap.get_fillMethod), new LuaCSFunction(UnityEngine_UI_ImageWrap.set_fillMethod)), new LuaField("fillAmount", new LuaCSFunction(UnityEngine_UI_ImageWrap.get_fillAmount), new LuaCSFunction(UnityEngine_UI_ImageWrap.set_fillAmount)), new LuaField("fillClockwise", new LuaCSFunction(UnityEngine_UI_ImageWrap.get_fillClockwise), new LuaCSFunction(UnityEngine_UI_ImageWrap.set_fillClockwise)), new LuaField("fillOrigin", new LuaCSFunction(UnityEngine_UI_ImageWrap.get_fillOrigin), new LuaCSFunction(UnityEngine_UI_ImageWrap.set_fillOrigin)), new LuaField("eventAlphaThreshold", new LuaCSFunction(UnityEngine_UI_ImageWrap.get_eventAlphaThreshold), new LuaCSFunction(UnityEngine_UI_ImageWrap.set_eventAlphaThreshold)), new LuaField("mainTexture", new LuaCSFunction(UnityEngine_UI_ImageWrap.get_mainTexture), null), new LuaField("hasBorder", new LuaCSFunction(UnityEngine_UI_ImageWrap.get_hasBorder), null), new LuaField("pixelsPerUnit", new LuaCSFunction(UnityEngine_UI_ImageWrap.get_pixelsPerUnit), null), new LuaField("minWidth", new LuaCSFunction(UnityEngine_UI_ImageWrap.get_minWidth), null), new LuaField("preferredWidth", new LuaCSFunction(UnityEngine_UI_ImageWrap.get_preferredWidth), null), new LuaField("flexibleWidth", new LuaCSFunction(UnityEngine_UI_ImageWrap.get_flexibleWidth), null), 
            new LuaField("minHeight", new LuaCSFunction(UnityEngine_UI_ImageWrap.get_minHeight), null), new LuaField("preferredHeight", new LuaCSFunction(UnityEngine_UI_ImageWrap.get_preferredHeight), null), new LuaField("flexibleHeight", new LuaCSFunction(UnityEngine_UI_ImageWrap.get_flexibleHeight), null), new LuaField("layoutPriority", new LuaCSFunction(UnityEngine_UI_ImageWrap.get_layoutPriority), null)
         };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.UI.Image", typeof(Image), regs, fields, typeof(MaskableGraphic));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_eventAlphaThreshold(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name eventAlphaThreshold");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index eventAlphaThreshold on a nil value");
            }
        }
        luaObject.eventAlphaThreshold = (float) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_fillAmount(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name fillAmount");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index fillAmount on a nil value");
            }
        }
        luaObject.fillAmount = (float) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_fillCenter(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name fillCenter");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index fillCenter on a nil value");
            }
        }
        luaObject.fillCenter = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_fillClockwise(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name fillClockwise");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index fillClockwise on a nil value");
            }
        }
        luaObject.fillClockwise = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_fillMethod(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name fillMethod");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index fillMethod on a nil value");
            }
        }
        luaObject.fillMethod = (Image.FillMethod) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(Image.FillMethod)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_fillOrigin(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name fillOrigin");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index fillOrigin on a nil value");
            }
        }
        luaObject.fillOrigin = (int) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_overrideSprite(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name overrideSprite");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index overrideSprite on a nil value");
            }
        }
        luaObject.overrideSprite = (Sprite) LuaScriptMgr.GetUnityObject(L, 3, typeof(Sprite));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_preserveAspect(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name preserveAspect");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index preserveAspect on a nil value");
            }
        }
        luaObject.preserveAspect = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_sprite(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name sprite");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index sprite on a nil value");
            }
        }
        luaObject.sprite = (Sprite) LuaScriptMgr.GetUnityObject(L, 3, typeof(Sprite));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_type(IntPtr L)
    {
        Image luaObject = (Image) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name type");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index type on a nil value");
            }
        }
        luaObject.type = (Image.Type) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(Image.Type)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetNativeSize(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((Image) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Image")).SetNativeSize();
        return 0;
    }
}

