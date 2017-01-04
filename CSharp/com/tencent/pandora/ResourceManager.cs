namespace com.tencent.pandora
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class ResourceManager : View
    {
        public static string actId = string.Empty;
        public bool atlasUpdate;
        public bool blUIResUnLoad;
        public Dictionary<string, int> dicTryToLoad = new Dictionary<string, int>();
        private List<DownTaskRunner> finished = new List<DownTaskRunner>();
        public static int iLuaVer = 1;
        public AssetBundle init_atlas;
        public static bool isLuaBundleReady;
        public Dictionary<string, TextAsset> luaFiles = new Dictionary<string, TextAsset>();
        public Dictionary<string, UnityEngine.Object> mdictFont = new Dictionary<string, UnityEngine.Object>();
        public int needDownAtlas;
        public int needDownFont;
        private float passTimeNow;
        public float passTimeSpan = 4f;
        private List<DownTaskRunner> runnner = new List<DownTaskRunner>();
        public bool startTimer;
        private Queue<DownTask> task = new Queue<DownTask>();
        private int taskMaxCount = 5;
        private int times;
        public bool useSA = true;

        public void AssemplyUI(GameObject goParent = null)
        {
            GameObject obj2 = goParent;
            if ((obj2 == null) && (base.PanelMgr.pages.Count > 0))
            {
                foreach (GameObject obj3 in base.PanelMgr.pages.Values)
                {
                    if (obj3 != null)
                    {
                        obj2 = obj3;
                        break;
                    }
                }
            }
            if (this.mdictFont.Count == 0)
            {
                com.tencent.pandora.Logger.d("mdictFont.Count = 0");
            }
            else if (obj2 != null)
            {
                Text[] componentsInChildren = obj2.GetComponentsInChildren<Text>(true);
                if (componentsInChildren.Length > 0)
                {
                    com.tencent.pandora.Logger.d("Font for interface assignment");
                    for (int i = 0; i < componentsInChildren.Length; i++)
                    {
                        if (this.mdictFont["usefont"] != null)
                        {
                            componentsInChildren[i].font = this.mdictFont["usefont"] as Font;
                            componentsInChildren[i].text = componentsInChildren[i].text;
                        }
                    }
                }
            }
        }

        public void DownloadRes()
        {
            if ((this.runnner.Count < this.taskMaxCount) && (this.task.Count > 0))
            {
                this.runnner.Add(new DownTaskRunner(this.task.Dequeue()));
            }
            foreach (DownTaskRunner runner in this.runnner)
            {
                if (string.IsNullOrEmpty(runner.www.error))
                {
                    if (runner.www.isDone)
                    {
                        if (!string.IsNullOrEmpty(runner.www.error))
                        {
                            com.tencent.pandora.Logger.d("The Task DownLoad Failed : " + runner.www.url.ToString() + " : Error : " + runner.www.error);
                        }
                        else
                        {
                            com.tencent.pandora.Logger.d("The Task DownLoad OK : " + runner.www.url.ToString());
                        }
                        this.finished.Add(runner);
                        if (runner.task.onload != null)
                        {
                            runner.task.onload(runner.www, runner.task.tag);
                        }
                    }
                    else if (runner.task.ondownloading != null)
                    {
                        runner.task.ondownloading(runner.www, runner.task.tag);
                    }
                }
                else
                {
                    com.tencent.pandora.Logger.d("The Task DownLoad Failed : " + runner.www.url.ToString() + "  Error :  " + runner.www.error);
                    this.finished.Add(runner);
                }
            }
            foreach (DownTaskRunner runner2 in this.finished)
            {
                this.runnner.Remove(runner2);
            }
            this.finished.Clear();
        }

        public static string getPlatformDesc()
        {
            if ((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.WindowsPlayer))
            {
                return "pc";
            }
            if (Application.platform == RuntimePlatform.Android)
            {
                return "android";
            }
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return "mac";
            }
            return "iphone";
        }

        public string GetResFullName(string strFileName)
        {
            return (getPlatformDesc() + "_" + strFileName + ".assetbundle");
        }

        public string GetResPath(string strFileName)
        {
            try
            {
                strFileName = this.GetResFullName(strFileName);
                string str = Application.streamingAssetsPath + "/vercache/";
                string curHotUpdatePath = Configer.m_CurHotUpdatePath;
                if (Configer.IsUseLocalRes)
                {
                    curHotUpdatePath = string.Empty;
                }
                string path = curHotUpdatePath + "/" + strFileName;
                if (curHotUpdatePath != string.Empty)
                {
                    try
                    {
                        path = curHotUpdatePath + "/" + strFileName;
                        if (File.Exists(path))
                        {
                            com.tencent.pandora.Logger.d("path:" + path);
                            this.useSA = false;
                        }
                        else
                        {
                            this.useSA = true;
                            com.tencent.pandora.Logger.d("path not exists:" + path);
                        }
                    }
                    catch (Exception exception)
                    {
                        com.tencent.pandora.Logger.d("查找热更目录资源出错 ： " + exception.Message + " , path = " + curHotUpdatePath);
                        this.useSA = true;
                    }
                }
                if (this.useSA)
                {
                    com.tencent.pandora.Logger.d("use SA");
                    path = Application.streamingAssetsPath + "/vercache/" + strFileName;
                }
                if (this.useSA)
                {
                    path = path;
                }
                else
                {
                    path = "file://" + path;
                }
                return path;
            }
            catch (Exception exception2)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception2.Message + exception2.StackTrace.ToString());
                return string.Empty;
            }
        }

        public bool HasFont(string strFontName)
        {
            return this.mdictFont.ContainsKey(strFontName);
        }

        public void InitAtlas()
        {
            try
            {
                Action<AssetBundle> action = delegate (AssetBundle bundle) {
                    if (bundle != null)
                    {
                        this.init_atlas = bundle;
                    }
                };
                this.LoadBundleRes("_cfFirstAtlas", null);
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace.ToString());
            }
        }

        private void Load(string path, string tag, Action<WWW, string> onLoad, Action<WWW, string> onLoaddowning)
        {
            this.task.Enqueue(new DownTask(path, tag, onLoad, onLoaddowning));
        }

        public void LoadBundleRes(string filename, Action<AssetBundle> reFunc)
        {
            <LoadBundleRes>c__AnonStorey31 storey = new <LoadBundleRes>c__AnonStorey31 {
                reFunc = reFunc
            };
            Action<WWW, string> onLoad = new Action<WWW, string>(storey.<>m__7);
            this.LoadFromRemote(filename, filename, onLoad, null);
        }

        public void LoadFromRemote(string filename, string tag, Action<WWW, string> onLoad, Action<WWW, string> onLoaddowning)
        {
            string resPath = this.GetResPath(filename);
            this.Load(resPath, tag, onLoad, onLoaddowning);
        }

        public void LoadLua(string luaname, Dictionary<string, TextAsset> dics, Action<bool> func)
        {
            <LoadLua>c__AnonStorey30 storey = new <LoadLua>c__AnonStorey30 {
                dics = dics,
                func = func,
                luaname = luaname,
                <>f__this = this
            };
            Action<AssetBundle> reFunc = new Action<AssetBundle>(storey.<>m__6);
            this.LoadBundleRes(storey.luaname, reFunc);
        }

        public void LoadUIFont(LuaTable fontList)
        {
            if ((fontList != null) && (fontList.Count > 0))
            {
                this.needDownFont = fontList.Count;
                IEnumerator enumerator = fontList.Values.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        string current = (string) enumerator.Current;
                        if (!string.IsNullOrEmpty(current))
                        {
                            if (this.mdictFont != null)
                            {
                                try
                                {
                                    com.tencent.pandora.Logger.d("loading -> " + current);
                                    GameObject obj2 = Resources.Load(current) as GameObject;
                                    Font font = obj2.GetComponent<Text>().font;
                                    if (font != null)
                                    {
                                        this.mdictFont["usefont"] = font;
                                    }
                                    else
                                    {
                                        com.tencent.pandora.Logger.d("Get Font is null, path : " + current);
                                    }
                                }
                                catch (Exception exception)
                                {
                                    com.tencent.pandora.Logger.d("LoadUIFont error :" + exception.Message);
                                }
                            }
                            else
                            {
                                com.tencent.pandora.Logger.d("already ContainsKey :" + current + ", not need to load this");
                            }
                        }
                        else
                        {
                            com.tencent.pandora.Logger.d("warn: Received font path = ''");
                        }
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
            }
        }

        public void LuaInitToVm(string strLuaName)
        {
            try
            {
                if (strLuaName.Equals("lua_core"))
                {
                    try
                    {
                        base.LuaManager.Start();
                        LuaBehaviour.initialize = true;
                        com.tencent.pandora.Logger.d("-----------int core-----------------");
                    }
                    catch (Exception exception)
                    {
                        LuaBehaviour.initialize = false;
                        com.tencent.pandora.Logger.d("Error: LuaManager.Start() Fail :" + exception.Message);
                    }
                }
                else
                {
                    if (this.times <= 0)
                    {
                        Debug.Log("#################################################################执行GameManager");
                        base.LuaManager.DoString("GameManager");
                        this.times++;
                    }
                    base.LuaManager.DoString(strLuaName);
                    NetProxcy.SetLoadFinishLuaFile(this.GetResFullName(strLuaName));
                }
            }
            catch (Exception exception2)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception2.Message + exception2.StackTrace.ToString());
            }
        }

        public void OnUILuaLoaded(object obj)
        {
            try
            {
                <OnUILuaLoaded>c__AnonStorey2F storeyf = new <OnUILuaLoaded>c__AnonStorey2F {
                    <>f__this = this,
                    strLuaName = string.Empty
                };
                Notification notification = (Notification) obj;
                if (obj != null)
                {
                    storeyf.strLuaName = notification.data.ToString();
                }
                if (storeyf.strLuaName.Equals(string.Empty))
                {
                    com.tencent.pandora.Logger.d("ERRROR:通知了一个空加载");
                }
                else
                {
                    Dictionary<string, TextAsset> dics = new Dictionary<string, TextAsset>();
                    Action<bool> func = new Action<bool>(storeyf.<>m__5);
                    com.tencent.pandora.Logger.d("OnLuaBundleLoaded    Receive :" + storeyf.strLuaName);
                    this.LoadLua(storeyf.strLuaName, dics, func);
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace.ToString());
            }
        }

        private void Start()
        {
            NotificationCenter.DefaultCenter().AddObserver(this, "OnUILuaLoaded");
        }

        private void Update()
        {
            try
            {
                if (!Pandora.NotDoUpdate)
                {
                    this.DownloadRes();
                    NetProxcy.RefreshLuaLoadStatus();
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace.ToString());
            }
        }

        [CompilerGenerated]
        private sealed class <LoadBundleRes>c__AnonStorey31
        {
            internal Action<AssetBundle> reFunc;

            internal void <>m__7(WWW w, string tag)
            {
                if (string.IsNullOrEmpty(w.error))
                {
                    AssetBundle assetBundle = w.assetBundle;
                    if (this.reFunc != null)
                    {
                        if (assetBundle != null)
                        {
                            this.reFunc(assetBundle);
                        }
                        else
                        {
                            com.tencent.pandora.Logger.d("load BundleRes failed: bundle = null/ tag = " + tag + "/ path = " + w.url);
                            this.reFunc(null);
                        }
                    }
                }
                else
                {
                    com.tencent.pandora.Logger.LogNetError(0x3ec, "load failed:" + w.error + "/ tag = " + tag);
                    com.tencent.pandora.Logger.d("load BundleRes failed:" + w.error + "/ tag = " + tag + "/ path = " + w.url);
                    this.reFunc(null);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <LoadLua>c__AnonStorey30
        {
            internal ResourceManager <>f__this;
            internal Dictionary<string, TextAsset> dics;
            internal Action<bool> func;
            internal string luaname;

            internal void <>m__6(AssetBundle bundle)
            {
                if (bundle != null)
                {
                    UnityEngine.Object[] objArray = bundle.LoadAll();
                    if (objArray.Length > 0)
                    {
                        for (int i = 0; i < objArray.Length; i++)
                        {
                            if (this.dics != null)
                            {
                                char[] chArray1 = new char[] { '.' };
                                this.dics[objArray[i].name.Split(chArray1)[0]] = objArray[i] as TextAsset;
                            }
                            char[] separator = new char[] { '.' };
                            this.<>f__this.luaFiles[objArray[i].name.Split(separator)[0]] = objArray[i] as TextAsset;
                        }
                        bundle.Unload(false);
                        if (this.func != null)
                        {
                            this.func(true);
                        }
                    }
                    else
                    {
                        com.tencent.pandora.Logger.LogNetError(0x3ec, "Erroe: get lua.assetbundle files count = 0");
                        com.tencent.pandora.Logger.d("Erroe: get lua.assetbundle files count = 0, bundle name = " + this.luaname);
                        if (this.func != null)
                        {
                            this.func(false);
                        }
                    }
                }
                else
                {
                    com.tencent.pandora.Logger.d("Error: Load lua assetbundle failed：bundle = null");
                    if (this.func != null)
                    {
                        this.func(false);
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <OnUILuaLoaded>c__AnonStorey2F
        {
            internal ResourceManager <>f__this;
            internal string strLuaName;

            internal void <>m__5(bool result)
            {
                if (result)
                {
                    try
                    {
                        NetProxcy.SetLoadFinishLuaFile(this.<>f__this.GetResFullName(this.strLuaName));
                    }
                    catch (Exception exception)
                    {
                        com.tencent.pandora.Logger.d("Error: Init common Lua error :" + exception.Message);
                    }
                }
            }
        }

        public class DownTask
        {
            public Action<WWW, string> ondownloading;
            public Action<WWW, string> onload;
            public string path;
            public string tag;

            public DownTask(string path, string tag, Action<WWW, string> onload, Action<WWW, string> ondownloading)
            {
                this.path = path;
                this.tag = tag;
                this.onload = onload;
                this.ondownloading = ondownloading;
            }
        }

        public class DownTaskRunner
        {
            public ResourceManager.DownTask task;
            public WWW www;

            public DownTaskRunner(ResourceManager.DownTask task)
            {
                this.task = task;
                this.www = WWW.LoadFromCacheOrDownload(this.task.path, ResourceManager.iLuaVer);
                com.tencent.pandora.Logger.d(string.Concat(new object[] { "Sart Download Task From :", this.task.path, ",ver:", ResourceManager.iLuaVer }));
            }
        }
    }
}

