namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class LuaBehaviour : View
    {
        private List<LuaFunction> buttons = new List<LuaFunction>();
        public static bool initialize;
        public bool isUseCor;
        public List<LuaFunction> timers = new List<LuaFunction>();

        public void AddTimeFunc(LuaFunction func)
        {
            this.timers.Add(func);
        }

        public void AddUGUIClick(GameObject go, LuaFunction luafunc)
        {
            <AddUGUIClick>c__AnonStorey2D storeyd = new <AddUGUIClick>c__AnonStorey2D {
                luafunc = luafunc,
                go = go,
                <>f__this = this
            };
            try
            {
                if (storeyd.go != null)
                {
                    storeyd.go.GetComponent<Button>().onClick.RemoveAllListeners();
                    storeyd.go.GetComponent<Button>().onClick.AddListener(new UnityAction(storeyd.<>m__2));
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
            }
        }

        protected void Awake()
        {
            object[] args = new object[] { base.gameObject };
            this.CallMethod("Awake", args);
        }

        public object[] CallMethod(string func, params object[] args)
        {
            try
            {
                if (!initialize)
                {
                    return null;
                }
                return Util.CallMethod(base.name, func, args);
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
                return null;
            }
        }

        public void ClearClick()
        {
            try
            {
                for (int i = 0; i < this.buttons.Count; i++)
                {
                    if (this.buttons[i] != null)
                    {
                        this.buttons[i].Dispose();
                        this.buttons[i] = null;
                    }
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
            }
        }

        private void FixedUpdate()
        {
            try
            {
                if (((base.LuaManager != null) && initialize) && this.isUseCor)
                {
                    base.LuaManager.FixedUpdate();
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
            }
        }

        private void LateUpdate()
        {
            try
            {
                if (((base.LuaManager != null) && initialize) && this.isUseCor)
                {
                    base.LuaManager.LateUpate();
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
            }
        }

        protected void OnClick()
        {
            this.CallMethod("OnClick", new object[0]);
        }

        protected void OnClickEvent(GameObject go)
        {
            object[] args = new object[] { go };
            this.CallMethod("OnClick", args);
        }

        protected void OnDestroy()
        {
            try
            {
                this.ClearClick();
                Util.ClearMemory();
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
            }
        }

        protected void Start()
        {
            if ((base.LuaManager != null) && initialize)
            {
                LuaState lua = base.LuaManager.lua;
                lua[base.name + ".transform"] = base.transform;
                lua[base.name + ".gameObject"] = base.gameObject;
            }
            this.CallMethod("Start", new object[0]);
        }

        private void Update()
        {
            try
            {
                if (((base.LuaManager != null) && initialize) && this.isUseCor)
                {
                    base.LuaManager.Update();
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
            }
        }

        [CompilerGenerated]
        private sealed class <AddUGUIClick>c__AnonStorey2D
        {
            internal LuaBehaviour <>f__this;
            internal GameObject go;
            internal LuaFunction luafunc;

            internal void <>m__2()
            {
                object[] args = new object[] { this.go };
                this.luafunc.Call(args);
                this.<>f__this.buttons.Add(this.luafunc);
            }
        }
    }
}

