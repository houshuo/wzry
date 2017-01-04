namespace com.tencent.pandora
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class PanelManager : View
    {
        private Dictionary<string, bool> bundleLock = new Dictionary<string, bool>();
        private Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();
        public Dictionary<string, GameObject> pages = new Dictionary<string, GameObject>();
        private Transform parent;

        public void CloseAllPanel()
        {
            try
            {
                List<string> list = new List<string>(this.pages.Keys);
                foreach (string str in list)
                {
                    string str2 = str;
                    str2 = str2.Substring(0, str2.Length - 5);
                    Util.CallMethod(str2 + "Ctrl", "Close", new object[0]);
                    com.tencent.pandora.Logger.d("close:" + str2);
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace.ToString());
            }
        }

        public void ClosePanel(string name)
        {
            try
            {
                name = name + "Panel";
                if (this.pages.ContainsKey(name) && (this.pages[name] != null))
                {
                    UnityEngine.Object.Destroy(this.pages[name]);
                    if (this.bundles[name] != null)
                    {
                        this.bundles[name].Unload(true);
                    }
                    this.bundleLock[name] = false;
                    this.pages.Remove(name);
                    if (this.pages.Count <= 0)
                    {
                        base.ResManager.startTimer = true;
                    }
                }
                Resources.UnloadUnusedAssets();
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace.ToString());
            }
        }

        public void CreatePanel(string name, LuaFunction func = null)
        {
            <CreatePanel>c__AnonStorey2E storeye = new <CreatePanel>c__AnonStorey2E {
                name = name,
                func = func,
                <>f__this = this
            };
            try
            {
                base.ResManager.startTimer = false;
                if (Pandora.GetPanelParent() != null)
                {
                    com.tencent.pandora.Logger.d("Pandora.GetPanelParent() != null");
                    this.parent = Pandora.GetPanelParent().transform;
                }
                else
                {
                    com.tencent.pandora.Logger.d("Pandora.GetInstance().goParent == null ,parent == UI Root");
                }
                Action<AssetBundle> reFunc = new Action<AssetBundle>(storeye.<>m__3);
                storeye.name = storeye.name + "Panel";
                if ((this.bundleLock != null) && this.bundleLock.ContainsKey(storeye.name))
                {
                    if (!this.bundleLock[storeye.name])
                    {
                        this.bundleLock[storeye.name] = true;
                        base.ResManager.LoadBundleRes(storeye.name, reFunc);
                    }
                    else
                    {
                        com.tencent.pandora.Logger.d("please wait moment to click");
                    }
                }
                else
                {
                    this.bundleLock[storeye.name] = true;
                    base.ResManager.LoadBundleRes(storeye.name, reFunc);
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace.ToString());
            }
        }

        [DebuggerHidden]
        private IEnumerator StartCreatePanel(string name, AssetBundle bundle, LuaFunction func = null)
        {
            return new <StartCreatePanel>c__Iterator0 { bundle = bundle, name = name, func = func, <$>bundle = bundle, <$>name = name, <$>func = func, <>f__this = this };
        }

        private Transform Parent
        {
            get
            {
                if (this.parent == null)
                {
                    GameObject obj2 = GameObject.Find("UI Root");
                    if (obj2 != null)
                    {
                        this.parent = obj2.transform;
                    }
                }
                return this.parent;
            }
        }

        [CompilerGenerated]
        private sealed class <CreatePanel>c__AnonStorey2E
        {
            internal PanelManager <>f__this;
            internal LuaFunction func;
            internal string name;

            internal void <>m__3(AssetBundle ab)
            {
                if (ab == null)
                {
                    com.tencent.pandora.Logger.LogNetError(0x3ec, "load panel " + this.name + "failed ,call lua function OnCreate with null parameter");
                    com.tencent.pandora.Logger.d("load panel " + this.name + "failed ,call lua function OnCreate with null parameter");
                }
                else
                {
                    AssetBundle bundle = ab;
                    this.<>f__this.bundles[this.name] = ab;
                    this.<>f__this.StartCoroutine(this.<>f__this.StartCreatePanel(this.name, bundle, this.func));
                    com.tencent.pandora.Logger.d("PanelManager: Lua call me , CreatePanel .." + this.name);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <StartCreatePanel>c__Iterator0 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal AssetBundle <$>bundle;
            internal LuaFunction <$>func;
            internal string <$>name;
            internal PanelManager <>f__this;
            internal Exception <ex>__1;
            internal Exception <ex>__3;
            internal Exception <ex>__4;
            internal GameObject <go>__2;
            internal GameObject <prefab>__0;
            internal AssetBundle bundle;
            internal LuaFunction func;
            internal string name;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<prefab>__0 = null;
                        try
                        {
                            this.<prefab>__0 = Util.LoadAsset(this.bundle, this.name);
                        }
                        catch (Exception exception)
                        {
                            this.<ex>__1 = exception;
                            com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, this.<ex>__1.Message + this.<ex>__1.StackTrace.ToString());
                        }
                        this.$current = new WaitForEndOfFrame();
                        this.$PC = 1;
                        goto Label_0226;

                    case 1:
                        this.<go>__2 = null;
                        try
                        {
                            if (this.<prefab>__0 != null)
                            {
                                this.<go>__2 = UnityEngine.Object.Instantiate(this.<prefab>__0) as GameObject;
                                this.<go>__2.name = this.name;
                                this.<go>__2.transform.parent = this.<>f__this.Parent;
                                this.<go>__2.transform.localScale = Vector3.one;
                                this.<go>__2.transform.localPosition = Vector3.zero;
                                this.<go>__2.AddComponent<LuaBehaviour>();
                            }
                        }
                        catch (Exception exception2)
                        {
                            this.<ex>__3 = exception2;
                            com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, this.<ex>__3.Message + this.<ex>__3.StackTrace);
                        }
                        this.$current = new WaitForEndOfFrame();
                        this.$PC = 2;
                        goto Label_0226;

                    case 2:
                        try
                        {
                            if (this.<go>__2 != null)
                            {
                                this.<>f__this.pages[this.name] = this.<go>__2;
                            }
                            if (this.func != null)
                            {
                                object[] args = new object[] { this.<go>__2 };
                                this.func.Call(args);
                            }
                            com.tencent.pandora.Logger.LogCommInfo("open " + this.name + " success");
                        }
                        catch (Exception exception3)
                        {
                            this.<ex>__4 = exception3;
                            com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, this.<ex>__4.Message + "," + this.<ex>__4.StackTrace);
                        }
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_0226:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

