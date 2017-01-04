namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class GameManager : LuaBehaviour
    {
        private List<string> downloadFiles = new List<string>();
        private static bool iIsCallInitLua;
        public static bool isInitUlua;

        private void FixedUpdate()
        {
            if (!Pandora.NotDoUpdate && ((base.LuaManager != null) && LuaBehaviour.initialize))
            {
                base.LuaManager.FixedUpdate();
            }
        }

        public void Init()
        {
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
            this.OnResourceInited();
        }

        private void LateUpdate()
        {
            if (!Pandora.NotDoUpdate && ((base.LuaManager != null) && LuaBehaviour.initialize))
            {
                base.LuaManager.LateUpate();
            }
        }

        private void OnDestroy()
        {
            if (base.LuaManager != null)
            {
                base.LuaManager.Destroy();
            }
            com.tencent.pandora.Logger.d("~GameManager was destroyed");
        }

        public void OnResourceInited()
        {
            try
            {
                com.tencent.pandora.Logger.d(" LuaManager.Start()");
                base.LuaManager.Start();
                base.LuaManager.DoString("GameManager");
                LuaBehaviour.initialize = true;
                foreach (object obj2 in base.CallMethod("LuaScriptPanel", new object[0]))
                {
                    string str = obj2.ToString().Trim();
                    if (!string.IsNullOrEmpty(str))
                    {
                        str = str + "Panel";
                        base.LuaManager.DoString(str);
                    }
                }
                base.CallMethod("OnInitOK", new object[0]);
                com.tencent.pandora.Logger.d("init lua success");
                isInitUlua = true;
                NotificationCenter.DefaultCenter().PostNotification(this, "OnLuaFinish");
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.d("init lua error : " + exception.Message);
            }
        }

        private void Update()
        {
            if (!Pandora.NotDoUpdate)
            {
                if (!iIsCallInitLua && ResourceManager.isLuaBundleReady)
                {
                    iIsCallInitLua = true;
                    this.Init();
                }
                if ((base.LuaManager != null) && LuaBehaviour.initialize)
                {
                    base.LuaManager.Update();
                }
            }
        }
    }
}

